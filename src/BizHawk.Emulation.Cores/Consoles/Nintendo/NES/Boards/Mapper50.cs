﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class Mapper50 : NesBoardBase
	{
		//http://wiki.nesdev.com/w/index.php/INES_Mapper_050

		private byte prg_bank;
		private int prg_bank_mask_8k;
		private bool irq_enable;
		private ushort irq_counter = 0;
		private bool irq_ready = false;
		private int ppu_cyclecount = 0;
		public override bool Configure(EDetectionOrigin origin)
		{
			//analyze board type
			switch (Cart.BoardType)
			{
				case "MAPPER050":
					break;
				default:
					return false;
			}
			prg_bank = 0;
			prg_bank_mask_8k = Cart.PrgSize / 8 - 1;
			SetMirrorType(EMirrorType.Vertical);
			return true;
		}

		public override void SyncState(Serializer ser)
		{
			ser.Sync(nameof(ppu_cyclecount), ref ppu_cyclecount);
			ser.Sync(nameof(prg_bank), ref prg_bank);
			ser.Sync(nameof(irq_enable), ref irq_enable);
			ser.Sync(nameof(irq_counter), ref irq_counter);
			ser.Sync(nameof(irq_ready), ref irq_ready);
			base.SyncState(ser);
		}

		public override void WriteExp(int addr, byte value)
		{
			addr &= 0x0120;
			if (addr == 0x0020)
			{
				prg_bank = (byte)(((value & 1) << 2) | ((value & 2) >> 1) | ((value & 4) >> 1) | (value & 8));
			}
			else if (addr == 0x0120)
			{
				irq_enable = value.Bit(0);
				if (!irq_enable)
				{
					SyncIRQ(irq_enable);
					irq_counter = 0;
				}
			}
		}

		public override byte ReadPrg(int addr)
		{
			if (addr < 0x2000)
			{
				return Rom[(0x08 * 0x2000) + (addr & 0x1FFF)];
			}
			else if (addr < 0x4000)
			{
				return Rom[(0x09 * 0x2000) + (addr & 0x1FFF)];
			}
			else if (addr < 0x6000)
			{
				int bank = (prg_bank & prg_bank_mask_8k);
				return Rom[(bank * 0x2000) + (addr & 0x1FFF)];
			}
			else
			{
				return Rom[(0x0B * 0x2000) + (addr & 0x1FFF)];
			}
		}

		public override byte ReadWram(int addr)
		{
			return Rom[(0x0F * 0x2000) + (addr & 0x1FFF)];
		}

		private void IRQ_Ready()
		{
			base.SyncIRQ(irq_ready);
		}

		public override void  ClockCpu()
		{
			if (irq_enable)
			{
				irq_counter += 1;
				if (irq_counter == 0x1000)
					SyncIRQ(true);
			}
		}

		/*
		public override void ClockPPU()
		{
			ppu_cyclecount++;
			if (ppu_cyclecount >= 3)
			{
				ppu_cyclecount = 0;
				IRQ_Tick();
				base.ClockPPU();
			}
		}*/
	}
}
