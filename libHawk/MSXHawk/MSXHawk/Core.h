#ifndef CORE_H
#define CORE_H

#include <iostream>
#include <cstdint>
#include <iomanip>
#include <string>
#include <cstring>

#include "Z80A.h"
#include "AY_3_8910.h"
#include "SCC.h"
#include "TMS9918A.h"
#include "Memory.h"

namespace MSXHawk
{
	class MSXCore
	{
	public:
		MSXCore() 
		{
			MemMap.cpu_pntr = &cpu;
			MemMap.vdp_pntr = &vdp;
			MemMap.psg_pntr = &psg;
			MemMap.SCC_1_pntr = &SCC_1;
			MemMap.SCC_2_pntr = &SCC_2;
			cpu.mem_ctrl = &MemMap;
			vdp.IRQ_PTR = &cpu.FlagI;
			vdp.SHOW_BG = vdp.SHOW_SPRITES = true;
			sl_case = 0;
			SCC_1.page_pntr = &MemMap.SCC_1_page[0];
			SCC_2.page_pntr = &MemMap.SCC_2_page[0];

			SCC_1.Reset();
			SCC_2.Reset();
		};

		TMS9918A vdp;
		Z80A cpu;
		AY_3_8910 psg;
		SCC SCC_1;
		SCC SCC_2;
		MemoryManager MemMap;

		uint8_t sl_case = 0;
		bool new_sample = false;

		// combine audio from all sources
		uint32_t sampleclock;
		uint32_t num_samples;
		int32_t samples[9000] = {};

		void Load_BIOS(uint8_t* bios, uint8_t* basic)
		{
			MemMap.Load_BIOS(bios, basic);
		}

		void Load_ROM(uint8_t* ext_rom_1, uint32_t ext_rom_size_1, uint32_t ext_rom_mapper_1, uint8_t* ext_rom_2, uint32_t ext_rom_size_2, uint32_t ext_rom_mapper_2)
		{
			MemMap.Load_ROM(ext_rom_1, ext_rom_size_1, ext_rom_mapper_1, ext_rom_2, ext_rom_size_2, ext_rom_mapper_2);
		}

		bool FrameAdvance(uint8_t controller_1, uint8_t controller_2, uint8_t* kb_rows_ptr, bool render, bool rendersound)
		{
			//MemMap.Mem_text_1.resize(2);
			//MemMap.Mem_text_1 = " ";
			
			if ((MemMap.psg_pntr->Register[0xF] & 0x40) > 0)
			{
				MemMap.psg_pntr->Register[0xE] = controller_2;
			}
			else 
			{
				MemMap.psg_pntr->Register[0xE] = controller_1;
			}
			
			MemMap.controller_byte_1 = controller_1;
			MemMap.controller_byte_2 = controller_2;
			MemMap.kb_rows = kb_rows_ptr;
			MemMap.start_pressed = (controller_1 & 0x80) > 0;
			MemMap.lagged = true;

			uint32_t scanlinesPerFrame = 262;
			vdp.SpriteLimit = true;

			num_samples = 0;
			sampleclock = 0;

			for (uint32_t i = 0; i < scanlinesPerFrame; i++)
			{
				vdp.ScanLine = i;

				vdp.RenderScanline(i);

				if (vdp.ScanLine == 192)
				{
					vdp.InterruptPendingSet(true);

					if (vdp.EnableInterrupts()) { cpu.FlagI = true; }						
				}

				switch (sl_case) 
				{
				case 0:
					for (int i = 0; i < 14; i++) 
					{
						cpu.ExecuteOne(16);
						sampleclock+=16;
						new_sample |= psg.generate_sound();
						new_sample |= SCC_1.generate_sound(16);
						//new_sample |= SCC_2.generate_sound();
						if (new_sample) { Add_Audio_Sample(); }
					}
					cpu.ExecuteOne(4);
					sampleclock += 4;
					sl_case = 1;
					break;

				case 1:
					cpu.ExecuteOne(12);
					sampleclock += 12;
					new_sample |= psg.generate_sound();
					new_sample |= SCC_1.generate_sound(12);
					//new_sample |= SCC_2.generate_sound();
					if (new_sample) { Add_Audio_Sample(); }
					
					for (int i = 0; i < 13; i++)
					{
						cpu.ExecuteOne(16);
						sampleclock += 16;
						new_sample |= psg.generate_sound();
						new_sample |= SCC_1.generate_sound(16);
						//new_sample |= SCC_2.generate_sound();
						if (new_sample) { Add_Audio_Sample(); }
					}
					cpu.ExecuteOne(8);
					sampleclock += 8;
					sl_case = 2;
					break;

				case 2:
					cpu.ExecuteOne(8);
					sampleclock += 8;
					new_sample |= psg.generate_sound();
					new_sample |= SCC_1.generate_sound(8);
					//new_sample |= SCC_2.generate_sound();
					if (new_sample) { Add_Audio_Sample(); }

					for (int i = 0; i < 13; i++)
					{
						cpu.ExecuteOne(16);
						sampleclock += 16;
						new_sample |= psg.generate_sound();
						new_sample |= SCC_1.generate_sound(16);
						//new_sample |= SCC_2.generate_sound();
						if (new_sample) { Add_Audio_Sample(); }
					}
					cpu.ExecuteOne(12);
					sampleclock += 12;
					sl_case = 3;
					break;
				case 3:
					cpu.ExecuteOne(4);
					sampleclock += 4;
					new_sample |= psg.generate_sound();
					new_sample |= SCC_1.generate_sound(4);
					//new_sample |= SCC_2.generate_sound();
					if (new_sample) { Add_Audio_Sample(); }

					for (int i = 0; i < 14; i++)
					{
						cpu.ExecuteOne(16);
						sampleclock += 16;
						new_sample |= psg.generate_sound();
						new_sample |= SCC_1.generate_sound(16);
						//new_sample |= SCC_2.generate_sound();
						if (new_sample) { Add_Audio_Sample(); }
					}
					sl_case = 0;
					break;
				}
			}

			return MemMap.lagged;
		}

		void Add_Audio_Sample() 
		{
			if (num_samples < 4500)
			{
				samples[num_samples * 2] = sampleclock;
				samples[num_samples * 2 + 1] = psg.current_sample - psg.old_sample;
				samples[num_samples * 2 + 1] += SCC_1.current_sample - SCC_1.old_sample;
				//samples[num_samples * 2 + 1] = SCC_2.current_sample - SCC_2.old_sample;
				num_samples++;
				psg.old_sample = psg.current_sample;
				SCC_1.old_sample = SCC_1.current_sample;
				//SCC_2.old_sample = SCC_2.current_sample;

				new_sample = false;
			}
		}

		void GetVideo(uint32_t* dest) 
		{
			uint32_t* src = vdp.FrameBuffer;
			uint32_t* dst = dest;

			std::memcpy(dst, src, sizeof (uint32_t) * 256 * 192);
		}

		uint32_t GetAudio(int32_t* dest, int32_t* n_samp) 
		{
			int32_t* src = samples;
			int32_t* dst = dest;

			std::memcpy(dst, src, sizeof (int32_t) * num_samples * 2);
			n_samp[0] = num_samples;

			return sampleclock;
		}

		int GetMessageLength() 
		{
			return (int)(MemMap.Mem_text_1.length() + 1);
		}

		// the copy length l must be supplied ahead of time from GetMessageLength
		void GetMessage(char* h, int l)
		{
			std::memcpy(h, MemMap.Mem_text_1.c_str(), l);
		}

		#pragma region State Save / Load

		void SaveState(uint8_t* saver)
		{
			saver = vdp.SaveState(saver);
			saver = cpu.SaveState(saver);
			saver = psg.SaveState(saver);
			saver = SCC_1.SaveState(saver);
			saver = SCC_2.SaveState(saver);
			saver = MemMap.SaveState(saver);

			*saver = (uint8_t)(new_sample ? 1 : 0); saver++;
			*saver = sl_case; saver++;
		}

		void LoadState(uint8_t* loader)
		{
			loader = vdp.LoadState(loader);
			loader = cpu.LoadState(loader);
			loader = psg.LoadState(loader);
			loader = SCC_1.LoadState(loader);
			loader = SCC_2.LoadState(loader);
			loader = MemMap.LoadState(loader);

			new_sample = *loader == 1; loader++;
			sl_case = *loader; loader++;
		}

		#pragma endregion

		#pragma region Memory Domain Functions

		uint8_t GetSysBus(uint32_t addr)
		{
			cpu.bank_num = cpu.bank_offset = addr & 0xFFFF;
			cpu.bank_offset &= cpu.low_mask;
			cpu.bank_num = (cpu.bank_num >> cpu.bank_shift)& cpu.high_mask;

			return cpu.MemoryMap[cpu.bank_num][cpu.bank_offset];
		}

		uint8_t GetVRAM(uint32_t addr) 
		{
			return vdp.VRAM[addr & 0x3FFF];
		}

		uint8_t GetRAM(uint32_t addr)
		{
			return MemMap.ram[addr & 0xFFFF];
		}

		#pragma endregion

		#pragma region Tracer

		void SetTraceCallback(void (*callback)(int))
		{
			cpu.TraceCallback = callback;
		}

		int GetHeaderLength()
		{
			return 105 + 1;
		}

		int GetDisasmLength()
		{
			return 48 + 1;
		}

		int GetRegStringLength()
		{
			return 86 + 1;
		}

		void GetHeader(char* h, int l)
		{
			std::memcpy(h, cpu.TraceHeader, l);
		}

		// the copy length l must be supplied ahead of time from GetRegStrngLength
		void GetRegisterState(char* r, int t, int l)
		{
			if (t == 0)
			{
				std::memcpy(r, cpu.CPURegisterState().c_str(), l);
			}
			else
			{
				std::memcpy(r, cpu.No_Reg, l);
			}
		}

		// the copy length l must be supplied ahead of time from GetDisasmLength
		void GetDisassembly(char* d, int t, int l)
		{
			if (t == 0)
			{
				std::memcpy(d, cpu.CPUDisassembly().c_str(), l);
			}
			else if (t == 1)
			{
				std::memcpy(d, cpu.NMI_event, l);
			}
			else
			{
				std::memcpy(d, cpu.IRQ_event, l);
			}
		}

		#pragma endregion		
	};
}

#endif
