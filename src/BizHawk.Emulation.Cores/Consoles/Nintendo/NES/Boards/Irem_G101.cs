﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	//AKA mapper 032

	//Image Fight
	//Major League
	//Kaiketsu Yanchamaru 2

	internal sealed class Irem_G101 : NesBoardBase
	{
		//configuration
		private int prg_bank_mask, chr_bank_mask;
		private bool oneScreenHack;

		//state
		private byte[] prg_regs_8k = new byte[8];
		private byte[] chr_regs_1k = new byte[8];
		private int prg_mode, mirror_mode;

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(prg_regs_8k), ref prg_regs_8k, false);
			ser.Sync(nameof(chr_regs_1k), ref chr_regs_1k, false);
			ser.Sync(nameof(prg_mode), ref prg_mode);
			ser.Sync(nameof(mirror_mode), ref mirror_mode);
		}

		public override bool Configure(EDetectionOrigin origin)
		{
			//configure
			switch (Cart.BoardType)
			{
				case "MAPPER032":
					break;
				case "IREM-G101":
					if (Cart.Pcb == "UNK-IF-13")
					{
						//special case for major league
						oneScreenHack = true;
					}
					AssertPrg(128, 256); AssertChr(128); AssertWram(0, 8); AssertVram(0);
					break;
				default:
					return false;
			}

			prg_bank_mask = Cart.PrgSize / 8 - 1;
			chr_bank_mask = Cart.ChrSize - 1;

			prg_regs_8k[0] = 0x00;
			prg_regs_8k[1] = 0x01;
			prg_regs_8k[2] = 0xFE; //constant
			prg_regs_8k[3] = 0xFF; //constant
			prg_regs_8k[4] = 0xFE; //constant //** NOTE ** according to disch's doc this would be fixed to 0. but it needs to be this to work. someone should let him know.
			prg_regs_8k[5] = 0x01;
			prg_regs_8k[6] = 0x00;
			prg_regs_8k[7] = 0xFF; //constant

			SyncMirror();

			return true;
		}

		private void SyncMirror()
		{
			if (oneScreenHack)
				SetMirrorType(EMirrorType.OneScreenA);
			else
				if (mirror_mode == 0)
					SetMirrorType(EMirrorType.Vertical);
				else SetMirrorType(EMirrorType.Horizontal);
		}

		public override void WritePrg(int addr, byte value)
		{
			addr &= 0xF007;
			switch (addr)
			{
				//$8000-$8007:  [PPPP PPPP]    PRG Reg 0
				case 0x0000: case 0x0001: case 0x0002: case 0x0003:
				case 0x0004: case 0x0005: case 0x0006: case 0x0007:
					prg_regs_8k[0] = value;
					prg_regs_8k[4 + 2] = value;
					break;

				//$9000-$9007:  [.... ..PM]
				//P = PRG Mode
				//M = Mirroring (0=Vert, 1=Horz) **Ignore for Major League**
				case 0x1000: case 0x1001: case 0x1002: case 0x1003:
				case 0x1004: case 0x1005: case 0x1006: case 0x1007:
					prg_mode = (value >> 1) & 1;
					prg_mode <<= 2;
					mirror_mode = value & 1;
					SyncMirror();
					break;

				//$A000-$A007:  [PPPP PPPP]    PRG Reg 1
				case 0x2000: case 0x2001: case 0x2002: case 0x2003:
				case 0x2004: case 0x2005: case 0x2006: case 0x2007:
					prg_regs_8k[1] = value;
					prg_regs_8k[4 + 1] = value;
					break;

				//$B000-$B007:  [CCCC CCCC]    CHR Regs
				case 0x3000: case 0x3001: case 0x3002: case 0x3003:
				case 0x3004: case 0x3005: case 0x3006: case 0x3007:
					chr_regs_1k[addr - 0x3000] = value;
					break;
			}
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

		public override byte ReadPrg(int addr)
		{
			int bank_8k = addr >> 13;
			int ofs = addr & ((1 << 13) - 1);
			bank_8k += prg_mode;
			bank_8k = prg_regs_8k[bank_8k];
			bank_8k &= prg_bank_mask;
			addr = (bank_8k << 13) | ofs;
			return Rom[addr];
		}
	}
}