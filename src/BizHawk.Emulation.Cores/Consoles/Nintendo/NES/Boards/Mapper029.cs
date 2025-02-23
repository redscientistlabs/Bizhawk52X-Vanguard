﻿namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	// what is this?
	internal sealed class Mapper029 : NesBoardBase
	{
		private int prg;
		private int chr;
		private int prg_bank_mask_16k;

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "Mapper029":
				case "UNIF_JERKFACE":
				case "UNIF_RET-CUFROM":
					break;
				default:
					return false;
			}
			SetMirrorType(EMirrorType.Vertical);
			AssertChr(0);
			AssertPrg(32, 64, 128, 256, 512, 1024);
			Cart.WramSize = 8;
			Cart.VramSize = 32;
			prg_bank_mask_16k = Cart.PrgSize / 16 - 1;
			return true;
		}

		public override void WritePrg(int addr, byte value)
		{
			chr = value & 3;
			prg = (value >> 2) & prg_bank_mask_16k;
		}

		public override byte ReadPrg(int addr)
		{
			int bank = addr >= 0x4000 ? prg_bank_mask_16k : prg;
			return Rom[bank << 14 | addr & 0x3fff];
		}
		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
				return Vram[addr | chr << 13];
			else
				return base.ReadPpu(addr);
		}
		public override void WritePpu(int addr, byte value)
		{
			if (addr < 0x2000)
				Vram[addr | chr << 13] = value;
			else
				base.WritePpu(addr, value);
		}
	}
}
