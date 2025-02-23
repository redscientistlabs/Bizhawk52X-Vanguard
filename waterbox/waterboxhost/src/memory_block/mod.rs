mod pageblock;
pub mod pal;
mod tripguard;
mod tests;

use std::sync::MutexGuard;
use std::ops::DerefMut;
use pageblock::PageBlock;
use crate::*;
use crate::syscall_defs::*;
use itertools::Itertools;
use std::sync::atomic::AtomicU32;
use crate::bin;

/// Tracks one lock for each 4GB memory area
mod lock_list {
	use lazy_static::lazy_static;
	use std::collections::HashMap;
	use std::sync::Mutex;
	use super::MemoryBlockRef;

	lazy_static! {
		static ref LOCK_LIST: Mutex<HashMap<u32, Box<Mutex<Option<MemoryBlockRef>>>>> = Mutex::new(HashMap::new());
	}

	unsafe fn extend<T>(o: &T) -> &'static T {
		std::mem::transmute::<&T, &'static T>(o)
	}
	/// adds a lock if it does not exist; no effect if it already does.
	pub fn maybe_add(lock_index: u32) {
		let map = &mut LOCK_LIST.lock().unwrap();
		map.entry(lock_index).or_insert_with(|| Box::new(Mutex::new(None)));	
	}
	/// Gets the lock for a particular index.
	pub fn get(lock_index: u32) -> &'static Mutex<Option<MemoryBlockRef>> {
		let map = &mut LOCK_LIST.lock().unwrap();
		unsafe {
			extend(map.get(&lock_index).unwrap())
		}
	}
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum Protection {
	None,
	R,
	RW,
	RX,
	RWX,
	RWStack
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
enum PageAllocation {
	/// not in use by the guest
	Free,
	/// in use by the guest system, with a particular allocation status
	Allocated(Protection),
}
impl PageAllocation {
	pub fn writable(&self) -> bool {
		use PageAllocation::*;
		match self {
			Allocated(a) => match a {
				Protection::RW | Protection::RWX | Protection::RWStack => true,
				_ => false
			}
			_ => false,
		}
	}
	pub fn readable(&self) -> bool {
		use PageAllocation::*;
		match self {
			Allocated(Protection::None) => false,
			Free => false,
			_ => true,
		}
	}
}

/// Stores information about the original data content of a memory area, before it got dirty
#[derive(Debug)]
enum Snapshot {
	None,
	ZeroFilled,
	Data(PageBlock),
}

/// Information about a single page of memory
#[derive(Debug)]
struct Page {
	pub status: PageAllocation,
	/// if true, the page has changed from its original state
	pub dirty: bool,
	pub snapshot: Snapshot,
	/// If true, the page content is not stored in states (but status still is).
	pub invisible: bool,
}
impl Page {
	pub fn new() -> Page {
		Page {
			status: PageAllocation::Free,
			dirty: false,
			snapshot: Snapshot::ZeroFilled,
			invisible: false,
		}
	}
	/// Take a snapshot if one is not yet stored
	/// unsafe: caller must ensure pages are mapped and addr is correct
	/// Does not check dirty or invisible
	pub unsafe fn maybe_snapshot(&mut self, addr: usize) {
		if match self.snapshot { Snapshot:: None => true, _ => false } {
			let mut snapshot = PageBlock::new();
			let src = std::slice::from_raw_parts(addr as *const u8, PAGESIZE);
			let dst = snapshot.slice_mut();
			dst.copy_from_slice(src);
			self.snapshot = Snapshot::Data(snapshot);	
		}
	}
	/// Compute the appropriate native protection value given this page's current status
	pub fn native_prot(&self) -> Protection {
		match self.status {
			#[cfg(windows)]
			PageAllocation::Allocated(Protection::RWStack) if self.dirty => Protection::RW,
			PageAllocation::Allocated(Protection::RW) if !self.dirty => Protection::R,
			PageAllocation::Allocated(Protection::RWX) if !self.dirty => Protection::RX,
			#[cfg(unix)]
			PageAllocation::Allocated(Protection::RWStack) => if self.dirty { Protection::RW } else { Protection::R },
			PageAllocation::Allocated(x) => x,
			PageAllocation::Free => Protection::None,
		}
	}
}

/// Used internally to talk about regions of memory together with their allocation status
struct PageRange<'a> {
	pub start: usize,
	pub mirror_start: usize,
	pub swapped_in: bool,
	pub pages: &'a mut [Page]
}
impl<'a> PageRange<'a> {
	pub fn addr(&self) -> AddressRange {
		AddressRange {
			start: self.start,
			size: self.pages.len() << PAGESHIFT
		}
	}
	pub fn mirror_addr(&self) -> AddressRange {
		AddressRange {
			start: self.mirror_start,
			size: self.pages.len() << PAGESHIFT
		}		
	}
	pub fn split_at_size(self, size: usize) -> (PageRange<'a>, PageRange<'a>) {
		let (sl, sr) = self.pages.split_at_mut(size >> PAGESHIFT);
		(
			PageRange {
				start: self.start,
				mirror_start: self.mirror_start,
				swapped_in: self.swapped_in,
				pages: sl
			},
			PageRange {
				start: self.start + size,
				mirror_start: self.mirror_start + size,
				swapped_in: self.swapped_in,
				pages: sr
			}
		)
	}
	pub fn iter(&self) -> std::slice::Iter<Page> {
		self.pages.iter()
	}
	pub fn iter_mut(&mut self) -> std::slice::IterMut<Page> {
		self.pages.iter_mut()
	}
	pub fn iter_with_addr(&self) -> impl Iterator<Item = (AddressRange, &Page)> {
		let mut start = self.start;
		self.pages.iter().map(move |p| {
			let page_start = start;
			start += PAGESIZE;
			(AddressRange { start: page_start, size: PAGESIZE}, p)
		})
	}
	pub fn iter_mut_with_addr(&mut self) -> impl Iterator<Item = (AddressRange, &mut Page)> {
		let mut start = self.start;
		self.pages.iter_mut().map(move |p| {
			let page_start = start;
			start += PAGESIZE;
			(AddressRange { start: page_start, size: PAGESIZE}, p)
		})
	}
	pub fn iter_mut_with_mirror_addr(&mut self) -> impl Iterator<Item = (AddressRange, &mut Page)> {
		let mut start = self.mirror_start;
		self.pages.iter_mut().map(move |p| {
			let page_start = start;
			start += PAGESIZE;
			(AddressRange { start: page_start, size: PAGESIZE}, p)
		})
	}
	/// fuse two adjacent ranges.  panics if they do not exactly touch
	pub fn fuse(left: Self, right: Self) -> PageRange<'a> {
		unsafe {
			let lp = left.pages.as_mut_ptr();
			let rp = right.pages.as_mut_ptr();
			assert_eq!(lp.add(left.pages.len()), rp);
			PageRange {
				start: left.start,
				mirror_start: left.mirror_start,
				swapped_in: left.swapped_in,
				pages: std::slice::from_raw_parts_mut(lp, left.pages.len() + right.pages.len())
			}
		}
	}
}

static NEXT_DEBUG_ID: AtomicU32 = AtomicU32::new(0);

#[derive(Debug)]
pub struct MemoryBlock {
	pages: Vec<Page>,
	addr: AddressRange,
	/// An always-visible second mirror of the address space with RW permissions
	/// Writes here will not trip dirty detection!
	mirror: AddressRange,
	sealed: bool,
	hash: Vec<u8>,

	lock_index: u32,
	handle: pal::Handle,

	debug_id: u32,

	/// The lock indicating that this is active, as viewed by the outside world
	active_guard: Option<BlockGuard>,

	/// Whether or not this is currently swapped in.  When ALWAYS_EVICT_BLOCKS is off,
	/// swapping out is done lazily so this might be true even when active_guard is not
	swapped_in: bool,
}

type BlockGuard = MutexGuard<'static, Option<MemoryBlockRef>>;

impl MemoryBlock {
	pub fn new(addr: AddressRange) -> Box<MemoryBlock> {
		if addr.start != align_down(addr.start) || addr.size != align_down(addr.size) {
			panic!("Addresses and sizes must be aligned!");
		}
		if addr.start >> 32 != (addr.end() - 1) >> 32 {
			panic!("MemoryBlock must fit into a single 4G region!");
		}

		let npage = addr.size >> PAGESHIFT;
		let mut pages = Vec::new();
		pages.reserve_exact(npage);
		for _ in 0..npage {
			pages.push(Page::new());
		}
		#[cfg(feature = "no-dirty-detection")]
		for p in pages.iter_mut() {
			p.dirty = true;
		}

		let handle = pal::open_handle(addr.size).unwrap();
		let lock_index = (addr.start >> 32) as u32;
		// add the lock_index stuff now, so we won't have to check for it later on activate / drop
		lock_list::maybe_add(lock_index);
		let mirror = pal::map_handle(&handle, AddressRange { start: 0, size: addr.size }).unwrap();
		unsafe { pal::protect(mirror, Protection::RW).unwrap(); }

		let debug_id = NEXT_DEBUG_ID.fetch_add(1, std::sync::atomic::Ordering::Relaxed);
		let res = Box::new(MemoryBlock {
			pages,
			addr,
			mirror,
			sealed: false,
			hash: Vec::new(),

			lock_index,
			handle,

			debug_id,
			active_guard: None,
			swapped_in: false,
		});
		println!("MemoryBlock created for address {:x}:{:x} with mirror {:x}:{:x}", addr.start, addr.end(), mirror.start, mirror.end());
		// res.trace("new");
		res
	}

	pub fn trace(&self, name: &str) {
		let ptr = unsafe { std::mem::transmute::<&Self, usize>(self) };
		let tid = unsafe { std::mem::transmute::<std::thread::ThreadId, u64>(std::thread::current().id()) };
		eprintln!("{}#{} {} [{}] thr{}",
			name, self.debug_id, ptr, self.lock_index, tid)
	}

	fn has_active_lock(&self) -> bool {
		match self.active_guard {
			Some(_) => true,
			None => false
		}
	}

	/// lock memory region and potentially swap this block into memory
	pub fn activate(&mut self) {
		unsafe {
			// self.trace("activate");
			if self.has_active_lock() {
				return
			}

			let area = lock_list::get(self.lock_index);
			let mut guard = area.lock().unwrap();

			let other_opt = guard.deref_mut();
			match *other_opt {
				Some(MemoryBlockRef(other)) => {
					if other != self {
						assert!(!(*other).has_active_lock());
						(*other).swapout();
						self.swapin();
						*other_opt = Some(MemoryBlockRef(self));
					}
				},
				None => {
					self.swapin();
					*other_opt = Some(MemoryBlockRef(self));	
				}
			}

			self.active_guard = Some(guard);
		}
	}
	/// unlock memory region, and potentially swap this block out of memory
	#[allow(unused_variables)] // unused stuff in release mode only
	#[allow(unused_mut)]
	pub fn deactivate(&mut self) {
		// self.trace("deactivate");
		if !self.has_active_lock() {
			return
		}
		let mut guard = std::mem::replace(&mut self.active_guard, None).unwrap();

		unsafe {
			if ALWAYS_EVICT_BLOCKS {
				// in debug mode, forcibly evict to catch dangling pointers
				let other_opt = guard.deref_mut();
				match *other_opt {
					Some(MemoryBlockRef(other)) => {
						if other != self {
							panic!();
						}
						self.swapout();
						*other_opt = None;
					},
					None => {
						panic!()
					}
				}
			}
		}
	}

	unsafe fn swapin(&mut self) {
		// self.trace("swapin");
		pal::map_handle(&self.handle, self.addr).unwrap();
		tripguard::register(self);
		self.swapped_in = true;
		self.refresh_all_protections();
	}
	unsafe fn swapout(&mut self) {
		// self.trace("swapout");
		self.get_stack_dirty();
		self.swapped_in = false;
		pal::unmap_handle(self.addr).unwrap();
		tripguard::unregister(self);
	}

	fn page_range(&mut self) -> PageRange {
		PageRange {
			start: self.addr.start,
			mirror_start: self.mirror.start,
			swapped_in: self.swapped_in,
			pages: &mut self.pages[..],
		}
	}

	fn validate_range(&mut self, addr: AddressRange) -> Result<PageRange, SyscallError> {
		if addr.start < self.addr.start
			|| addr.end() > self.addr.end()
			|| addr.size == 0
			|| addr.start != align_down(addr.start)
			|| addr.size != align_down(addr.size) {
			Err(EINVAL)
		} else {
			let offset = addr.start - self.addr.start;
			let pstart = offset >> PAGESHIFT;
			let psize = (addr.size) >> PAGESHIFT;
			Ok(PageRange {
				start: addr.start,
				mirror_start: self.mirror.start + offset,
				swapped_in: self.swapped_in,
				pages: &mut self.pages[pstart..pstart + psize]
			})
		}
	}

	/// Refresh the correct protections in underlying host RAM on a page range.  Use after
	/// temporary pal::protect(...) modifications, or to apply the effect of a dirty/prot change on the page
	fn refresh_protections(range: &PageRange) {
		if !range.swapped_in {
			return
		}
		struct Chunk {
			addr: AddressRange,
			prot: Protection,
		}
		let mut start = range.start;
		let chunks = range.iter()
			.map(|p| {
				let cstart = start;
				start += PAGESIZE;
				Chunk {
					addr: AddressRange { start: cstart, size: PAGESIZE },
					prot: p.native_prot(),
				}
			})
			.coalesce(|x, y| if x.prot == y.prot {
				Ok(Chunk {
					addr: AddressRange { start: x.addr.start, size: x.addr.size + y.addr.size },
					prot: x.prot,
				})
			} else {
				Err((x, y))
			});

		for c in chunks {
			unsafe {
				pal::protect(c.addr, c.prot).unwrap();
			}
		}
	}

	fn refresh_all_protections(&mut self) {
		MemoryBlock::refresh_protections(&self.page_range())
	}

	/// Applies new protections to a pagerange, including special RWStack handling on Windows
	fn set_protections(range: &mut PageRange, status: PageAllocation) {
		for p in range.iter_mut() {
			p.status = status;
		}
		MemoryBlock::refresh_protections(&range);
		#[cfg(windows)]
		if status == PageAllocation::Allocated(Protection::RWStack) {
			// have to precapture snapshots here
			for (maddr, p) in range.iter_mut_with_mirror_addr() {
				unsafe {
					p.maybe_snapshot(maddr.start)
				}
			}
		}
	}

	/// Updates knowledge on RWStack tripped areas.  Must be called before those areas change allocation type, or are swapped out.
	/// noop on linux
	fn get_stack_dirty(&mut self) {
		#[cfg(windows)]
		unsafe {
			if !self.swapped_in {
				return
			}
			let mut start = self.addr.start;
			let mut pindex = 0;
			while start < self.addr.end() {
				if !self.pages[pindex].dirty && self.pages[pindex].status == PageAllocation::Allocated(Protection::RWStack) {
					let mut res = pal::get_stack_dirty(start).unwrap();
					while res.size > 0 && start < self.addr.end() {
						if res.dirty && self.pages[pindex].status == PageAllocation::Allocated(Protection::RWStack) {
							self.pages[pindex].dirty = true;
						}
						res.size -= PAGESIZE;
						start += PAGESIZE;
						pindex += 1;
					}
				} else {
					start += PAGESIZE;
					pindex += 1;
				}
			}
		}
	}

	pub fn page_len(&self) -> usize {
		self.pages.len()
	}

	pub fn page_info(&self, index: usize) -> u8 {
		let p = &self.pages[index];
		let mut res = match p.status {
			PageAllocation::Free => 0,
			PageAllocation::Allocated(prot) => match prot {
				Protection::None => 0x20,
				Protection::R => 1,
				Protection::RW => 3,
				Protection::RX => 5,
				Protection::RWX => 7,
				Protection::RWStack => 0x13,
			}
		};
		if p.dirty {
			res |= 0x80;
		}
		if p.invisible {
			res |= 0x40;
		}
		res
	}
}

impl Drop for MemoryBlock {
	fn drop(&mut self) {
		// self.trace("drop");
		self.deactivate();

		let area = lock_list::get(self.lock_index);
		let mut guard = area.lock().unwrap();
		let other_opt = guard.deref_mut();
		match *other_opt {
			Some(MemoryBlockRef(other)) => {
				if other == self {
					unsafe { self.swapout(); }
					*other_opt = None;
				}
			},
			None => ()
		}
		unsafe { let _ = pal::unmap_handle(self.mirror); }
		let h = std::mem::replace(&mut self.handle, pal::bad());
		unsafe { let _ = pal::close_handle(h); }
	}
}

impl MemoryBlock {
	/// Looks for some free pages inside an arena
	fn find_free_pages<'a>(arena: &'a mut PageRange<'a>, npages: usize) -> Result<PageRange<'a>, SyscallError> {
		let swapped_in = arena.swapped_in;
		struct Chunk<'a> {
			range: PageRange<'a>,
			free: bool,
		}
		let disp = arena.mirror_start.wrapping_sub(arena.start);
		let range = arena.iter_mut_with_addr()
			.map(|(a, p)| Chunk {
				free: p.status == PageAllocation::Free,
				range: PageRange {
					start: a.start,
					mirror_start: a.start.wrapping_add(disp),
					swapped_in,
					pages: std::slice::from_mut(p),
				},
			})
			.coalesce(|x, y| {
				if x.free == y.free {
					Ok(Chunk {
						free: x.free,
						range: PageRange::fuse(x.range, y.range)
					})
				} else {
					Err((x, y))
				}
			})
			.filter(|c| c.free && c.range.pages.len() >= npages)
			.map(|c| c.range)
			.sorted_by(|x, y| x.pages.len().cmp(&y.pages.len()))
			.next();
		match range {
			Some(r) => {
				if r.pages.len() == npages {
					Ok(r)
				} else {
					Ok(PageRange {
						start: r.start,
						mirror_start: r.mirror_start,
						swapped_in,
						pages: &mut r.pages[0..npages],
					})
				}
			},
			None => Err(ENOMEM)
		}
	}

	/// implements a subset of mmap(2) for anonymous, movable address mappings
	fn mmap_movable(&mut self, size: usize, prot: Protection, arena_addr: AddressRange) -> Result<usize, SyscallError> {
		if size != align_down(size) {
			return Err(EINVAL)
		}
		let mut arena = self.validate_range(arena_addr).unwrap();
		match MemoryBlock::find_free_pages(&mut arena, size >> PAGESHIFT) {
			Ok(mut range) => {
				MemoryBlock::set_protections(&mut range, PageAllocation::Allocated(prot));
				Ok(range.start)		
			},
			Err(e) => Err(e),
		}
	}

	/// implements a subset of mmap(2) for anonymous, fixed address mappings
	pub fn mmap_fixed(&mut self, addr: AddressRange, prot: Protection, no_replace: bool) -> SyscallResult {
		let mut range = self.validate_range(addr)?;
		if no_replace && range.iter().any(|p| p.status != PageAllocation::Free) {
			return Err(EEXIST)
		}
		MemoryBlock::set_protections(&mut range, PageAllocation::Allocated(prot));
		Ok(())
	}

	/// implements a subset of mremap(2) when MREMAP_MAYMOVE is not set, and MREMAP_FIXED is not
	fn mremap_nomove(&mut self, addr: AddressRange, new_size: usize) -> SyscallResult {
		self.get_stack_dirty();
		if new_size > addr.size {
			let full_addr = AddressRange { start: addr.start, size: new_size };
			let range = self.validate_range(full_addr)?;
			let (old_range, mut new_range) = range.split_at_size(addr.size);
			if old_range.iter().any(|p| p.status == PageAllocation::Free) {
				return Err(EINVAL)
			}
			if new_range.iter().any(|p| p.status != PageAllocation::Free) {
				return Err(EEXIST)
			}
			MemoryBlock::set_protections(&mut new_range, old_range.pages[0].status);
			Ok(())
		} else {
			let range = self.validate_range(addr)?;
			if range.iter().any(|p| p.status == PageAllocation::Free) {
				return Err(EINVAL)
			}
			self.munmap_impl(AddressRange { start: addr.start + new_size, size: addr.size - new_size }, false)
		}
	}

	/// implements a subset of mremap(2) when MREMAP_MAYMOVE is set, and MREMAP_FIXED is not
	fn mremap_maymove(&mut self, addr: AddressRange, new_size: usize, arena_addr: AddressRange) -> Result<usize, SyscallError> {
		// This could be a lot more clever, but it's a difficult problem and doesn't come up often.
		// So I use a "simple" solution here.
		self.get_stack_dirty();
		if new_size != align_down(new_size) {
			return Err(EINVAL)
		}

		// save a copy of src, and unmap
		let mut src = self.validate_range(addr)?;
		if src.iter().any(|p| p.status == PageAllocation::Free) {
			return Err(EINVAL)
		}

		let src_maddr = src.mirror_addr();
		let mut old_status = Vec::new();
		old_status.reserve_exact(src.pages.len());
		let mut old_data = vec![0u8; src_maddr.size];
		for p in src.iter() {
			old_status.push(p.status);
		}
		unsafe {
			old_data.copy_from_slice(src_maddr.slice());
		}
		MemoryBlock::free_pages_impl(&mut src, false);

		// find new location to map to, and copy into there
		let mut arena = self.validate_range(arena_addr).unwrap();
		let mut dest = match MemoryBlock::find_free_pages(&mut arena, new_size >> PAGESHIFT) {
			Ok(r) => r,
			Err(_) => {
				// woops! reallocate at the old address.
				// Or just panic because that probably won't happen
				panic!("Failure in realloc")
			},
		};
		let nbcopy = std::cmp::min(addr.size, new_size);
		let npcopy = nbcopy >> PAGESHIFT;
		unsafe {
			dest.mirror_addr().slice_mut()[0..nbcopy].copy_from_slice(&old_data[0..nbcopy]);
		}
		for (status, pdst) in old_status.iter().zip(dest.iter_mut()) {
			pdst.status = *status;
			// this is conservative; there are situations where dirty might be false,
			// but we're unlikely to see them with real world realloc usage
			pdst.dirty = true;
		}
		for pdst in dest.pages[npcopy..].iter_mut() {
			pdst.status = old_status[0];
		}
		MemoryBlock::refresh_protections(&dest);
		Ok(dest.start)
	}

	/// implements a subset of mprotect(2)
	pub fn mprotect(&mut self, addr: AddressRange, prot: Protection) -> SyscallResult {
		self.get_stack_dirty();
		let mut range = self.validate_range(addr)?;
		if range.iter().any(|p| p.status == PageAllocation::Free) {
			return Err(ENOMEM)
		}
		MemoryBlock::set_protections(&mut range, PageAllocation::Allocated(prot));
		Ok(())
	}

	/// implements a subset of munmap(2)
	pub fn munmap(&mut self, addr: AddressRange) -> SyscallResult {
		self.munmap_impl(addr, false)
	}

	pub fn mmap(&mut self, addr: AddressRange, prot: Protection, arena_addr: AddressRange, no_replace: bool) -> Result<usize, SyscallError> {
		if addr.size == 0 {
			return Err(EINVAL)
		}
		if addr.start == 0 {
			self.mmap_movable(addr.size, prot, arena_addr)
		} else {
			self.mmap_fixed(addr, prot, no_replace)?;
			Ok(addr.start)
		}
	}

	pub fn mremap(&mut self, addr: AddressRange, new_size: usize, arena_addr: AddressRange) -> Result<usize, SyscallError> {
		if addr.size == 0 || new_size == 0 {
			return Err(EINVAL)
		}
		if addr.start == 0 {
			self.mremap_maymove(addr, new_size, arena_addr)
		} else {
			self.mremap_nomove(addr, new_size)?;
			Ok(addr.start)
		}
	}

	/// release pages, assuming the range has been fully validated already
	fn free_pages_impl(range: &mut PageRange, advise_only: bool) {
		unsafe {
			// We do not save the current state of unmapped pages, and if they are later remapped,
			// the expectation is that they will start out as zero filled.  Accordingly, the most
			// sensible way to do this is to zero them now.
			// Since this will mutate the current memory, if they have no snapshot stored we must store one now.
			for (maddr, p) in range.iter_mut_with_mirror_addr() {
				p.maybe_snapshot(maddr.start);
			}
			range.mirror_addr().zero();

			// simple state size optimization: we can undirty pages in this case depending on the initial state
			#[cfg(not(feature = "no-dirty-detection"))]
			for p in range.iter_mut() {
				p.dirty = !p.invisible && match p.snapshot {
					Snapshot::ZeroFilled => false,
					_ => true
				};
			}
		}
		if advise_only {
			MemoryBlock::refresh_protections(range);
		} else {
			MemoryBlock::set_protections(range, PageAllocation::Free);
		}
	}

	/// munmap or MADV_DONTNEED
	fn munmap_impl(&mut self, addr: AddressRange, advise_only: bool) -> SyscallResult {
		self.get_stack_dirty();
		let mut range = self.validate_range(addr)?;
		if range.iter().any(|p| p.status == PageAllocation::Free) {
			return Err(EINVAL)
		}
		MemoryBlock::free_pages_impl(&mut range, advise_only);
		Ok(())
	}
	/// Marks an address range as invisible.  Its page content will not be saved in states (but
	/// their allocation status still will be.)  Cannot be revoked.  Must be done before sealing.
	/// The pages need not be currently mapped; they will always be invisible regardless of that.
	/// !!Not actually saved in states, as is assumed to be unchanging for a particular layout.!!
	pub fn mark_invisible(&mut self, addr: AddressRange) -> SyscallResult {
		// The limitations on this method are mostly because we want to not need a snapshot or dirty
		// tracking for invisible pages.  But if we didn't have one and later the pages became visible,
		// we'd need one and wouldn't be able to reconstruct one.
		assert!(!self.sealed);
		let mut range = self.validate_range(addr)?;
		for p in range.iter_mut() {
			p.dirty = true;
			p.invisible = true;
		}
		MemoryBlock::refresh_protections(&range);
		Ok(())
	}

	/// implements a subset of madvise(2)
	pub fn madvise_dontneed(&mut self, addr: AddressRange) -> SyscallResult {
		self.munmap_impl(addr, true)
	}

	pub fn seal(&mut self) -> anyhow::Result<()> {
		if self.sealed {
			return Err(anyhow!("Already sealed!"))
		}
		self.get_stack_dirty();

		for (maddr, p) in self.page_range().iter_mut_with_mirror_addr() {
			if p.dirty && !p.invisible {
				p.dirty = false;
				p.snapshot = Snapshot::None;

				// Just as we needed to precapture RWStack snapshots when allocating on Windows, we also do when sealing
				#[cfg(windows)]
				if p.status == PageAllocation::Allocated(Protection::RWStack) {
					unsafe {
						p.maybe_snapshot(maddr.start)
					}
				}
			}
		}

		#[cfg(feature = "no-dirty-detection")]
		unsafe {
			for (a, p) in self.page_range().iter_mut_with_mirror_addr() {
				p.dirty = true;
				p.maybe_snapshot(a.start);
			}
		}

		self.refresh_all_protections();
		self.sealed = true;

		use sha2::{Sha256, Digest};
		self.hash = {
			let mut hasher = Sha256::new();
			bin::write(&mut hasher, &self.addr).unwrap();

			#[cfg(not(feature = "no-dirty-detection"))]
			{
				for p in self.pages.iter() {
					match &p.snapshot {
						Snapshot::None => bin::writeval(&mut hasher, 1).unwrap(),
						Snapshot::ZeroFilled => bin::writeval(&mut hasher, 2).unwrap(),
						Snapshot::Data(d) => { hasher.write(d.slice()).unwrap(); },
					}
				}
			}

			hasher.finalize()[..].to_owned()
		};

		Ok(())
	}

	/// Helper to copy bytes into guest memory, to guest address `start`
	pub fn copy_from_external(&mut self, src: &[u8], start: usize) -> SyscallResult {
		{
			let addr = AddressRange {
				start,
				size: src.len()
			};
			let mut range = self.validate_range(addr.align_expand())?;
			for p in range.iter_mut() {
				p.dirty = true;
			}
		}
		let dest = AddressRange {
			start: start - self.addr.start + self.mirror.start,
			size: src.len()
		};
		unsafe { dest.slice_mut().copy_from_slice(src) }
		Ok(())
	}
}

const MAGIC: &str = "ActivatedMemoryBlock";

impl IStateable for MemoryBlock {
	fn save_state(&mut self, stream: &mut dyn Write) -> anyhow::Result<()> {
		if !self.sealed {
			return Err(anyhow!("Must seal first"))
		}

		bin::write_magic(stream, MAGIC)?;
		bin::write_hash(stream, &self.hash[..])?;
		self.get_stack_dirty();
		self.addr.save_state(stream)?;

		unsafe {
			let mut statii = Vec::new();
			let mut dirtii = Vec::new();
			statii.reserve_exact(self.pages.len());
			dirtii.reserve_exact(self.pages.len());
			for p in self.pages.iter() {
				statii.push(p.status);
				dirtii.push(p.dirty);
			}
			stream.write_all(std::mem::transmute(&statii[..]))?;
			stream.write_all(std::mem::transmute(&dirtii[..]))?;
		}

		for (paddr, p) in self.page_range().iter_mut_with_mirror_addr() {
			// bin::write(stream, &p.status)?;
			if !p.invisible {
				// bin::write(stream, &p.dirty)?;
				if p.dirty {
					unsafe {
						stream.write_all(paddr.slice())?;
					}
				}
			}
		}
		Ok(())
	}

	fn load_state(&mut self, stream: &mut dyn Read) -> anyhow::Result<()> {
		if !self.sealed {
			return Err(anyhow!("Must seal first"))
		}

		bin::verify_magic(stream, MAGIC)?;
		match bin::verify_hash(stream, &self.hash[..]) {
			Ok(_) => (),
			Err(_) => eprintln!("Unexpected MemoryBlock hash mismatch."),
		}
		self.get_stack_dirty();
		{
			let mut addr = AddressRange { start:0, size: 0 };
			addr.load_state(stream)?;
			if addr != self.addr {
				return Err(anyhow!("Bad state data (addr) for ActivatedMemoryBlock"))
			}
		}

		unsafe {
			let mut statii = vec![PageAllocation::Free; self.pages.len()];
			let mut dirtii = vec![false; self.pages.len()];
			stream.read_exact(std::mem::transmute(&mut statii[..]))?;
			stream.read_exact(std::mem::transmute(&mut dirtii[..]))?;

			let mut index = 0usize;
			for (paddr, p) in self.page_range().iter_mut_with_mirror_addr() {
				let status = statii[index];
				// let status = bin::readval::<PageAllocation>(stream)?;
				if !p.invisible {
					let dirty = dirtii[index];
					// let dirty = bin::readval::<bool>(stream)?;
					match (p.dirty, dirty) {
						(false, false) => (),
						(false, true) => {
							p.maybe_snapshot(paddr.start);
							stream.read_exact(paddr.slice_mut())?;
						},
						(true, false) => {
							match &p.snapshot {
								Snapshot::ZeroFilled => paddr.zero(),
								Snapshot::Data(b) => {
									std::ptr::copy_nonoverlapping(b.as_ptr(), paddr.start as *mut u8, PAGESIZE)
								},
								Snapshot::None => panic!("Missing snapshot for dirty region"),
							}
						}
						(true, true) => {
							stream.read_exact(paddr.slice_mut())?;
						}
					}
					p.dirty = dirty;
				}
				p.status = status;
				index += 1;
			}

			self.refresh_all_protections();
		}
		Ok(())
	}
}

impl PartialEq for MemoryBlock {
	fn eq(&self, other: &MemoryBlock) -> bool {
		self as *const MemoryBlock == other as *const MemoryBlock
	}
}
impl Eq for MemoryBlock {}

#[derive(Debug)]
pub struct MemoryBlockRef(*mut MemoryBlock);
unsafe impl Send for MemoryBlockRef {}
