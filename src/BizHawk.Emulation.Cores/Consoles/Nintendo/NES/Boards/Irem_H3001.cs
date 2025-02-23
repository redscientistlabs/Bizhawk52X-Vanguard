﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	//AKA mapper 65

	//Daiku no Gen San 2
	//Spartan X 2

	//NOTE - fceux support for this mapper has some kind of -4 cpu cycle delay built into the timer. not sure yet whether we need that

	internal sealed class Irem_H3001 : NesBoardBase
	{
		//configuration
		private int prg_bank_mask, chr_bank_mask;

		//state
		private byte[] prg_regs_8k = new byte[4];
		private byte[] chr_regs_1k = new byte[8];
		private bool irq_counter_enabled, irq_asserted;
		private ushort irq_counter, irq_reload;
		private int clock_counter;

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(prg_regs_8k), ref prg_regs_8k, false);
			ser.Sync(nameof(chr_regs_1k), ref chr_regs_1k, false);
			ser.Sync(nameof(irq_counter_enabled), ref irq_counter_enabled);
			ser.Sync(nameof(irq_asserted), ref irq_asserted);
			ser.Sync(nameof(irq_counter), ref irq_counter);
			ser.Sync(nameof(irq_reload), ref irq_reload);
			ser.Sync(nameof(clock_counter), ref clock_counter);
			SyncIRQ();
		}

		public override bool Configure(EDetectionOrigin origin)
		{
			//configure
			switch (Cart.BoardType)
			{
				case "MAPPER065":
					break;
				case "IREM-H3001":
					AssertPrg(128, 256); AssertChr(128, 256); AssertVram(0); AssertWram(0);
					break;
				default:
					return false;
			}

			prg_bank_mask = Cart.PrgSize / 8 - 1;
			chr_bank_mask = Cart.ChrSize - 1;

			prg_regs_8k[0] = 0x00;
			prg_regs_8k[1] = 0x01;
			prg_regs_8k[2] = 0xFE;
			prg_regs_8k[3] = 0xFF; //constant
			return true;
		}

		/*
		public override void ClockPPU()
		{
			clock_counter++;
			if (clock_counter == 3)
			{
				ClockCPU();
				clock_counter = 0;
			}
		}*/

		public override void ClockCpu()
		{
			if (irq_counter == 0) return;
			if (!irq_counter_enabled) return;
			irq_counter--;
			if (irq_counter != 0) return;
			irq_asserted = true;
			SyncIRQ();
		}

		private void SyncIRQ()
		{
			IrqSignal = irq_asserted;
		}

		public override byte ReadPrg(int addr)
		{
			int bank_8k = addr >> 13;
			int ofs = addr & ((1 << 13) - 1);
			bank_8k = prg_regs_8k[bank_8k];
			bank_8k &= prg_bank_mask;
			addr = (bank_8k << 13) | ofs;
			return Rom[addr];
		}

		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
			{
				int bank_1k = addr >> 10;
				int ofs = addr & ((1 << 10) - 1);
				bank_1k = chr_regs_1k[bank_1k];
				bank_1k &= chr_bank_mask;
				addr = (bank_1k << 10) | ofs;
				return Vrom[addr];
			}
			else
				return base.ReadPpu(addr);
		}

		public override void WritePrg(int addr, byte value)
		{
			switch (addr)
			{
				case 0x0000: //$8000:  PRG Reg 0 (8k @ $8000)
					prg_regs_8k[0] = value;
					break;
				case 0x2000: //$A000:  PRG Reg 1 (8k @ $A000)
					prg_regs_8k[1] = value;
					break;
				case 0x4000: //$C000:  PRG Reg 2 (8k @ $C000)
					prg_regs_8k[2] = value;
					break;

				case 0x1001: //$9001:  [M... ....]  Mirroring
					if ((value & 0x80) == 0) SetMirrorType(EMirrorType.Vertical);
					else SetMirrorType(EMirrorType.Horizontal);
					break;
				case 0x1003: //$9003:  [E... ....]  IRQ Enable (0=disabled, 1=enabled)
					irq_counter_enabled = (value & 0x80) != 0;
					irq_asserted = false;
					SyncIRQ();
					break;
				case 0x1004: //$9004:  [.... ....]  Reload IRQ counter
					irq_counter = irq_reload;
					irq_asserted = false;
					SyncIRQ();
					break;
				case 0x1005: //$9005:  [IIII IIII]  High 8 bits of IRQ Reload value
					irq_reload = (ushort)((irq_reload & 0x00FF) | (value << 8));
					break;
				case 0x1006: //$9006:  [IIII IIII]  Low 8 bits of IRQ Reload value
					irq_reload = (ushort)((irq_reload & 0xFF00) | (value));
					break;

				//$B000-$B007:  CHR regs
				case 0x3000: case 0x3001: case 0x3002: case 0x3003:
				case 0x3004: case 0x3005: case 0x3006: case 0x3007:
					chr_regs_1k[addr - 0x3000] = value;
					break;
			}
		}
	}
}