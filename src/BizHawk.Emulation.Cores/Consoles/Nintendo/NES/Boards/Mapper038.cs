﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	// Crime Busters (Brazil) (Unl)
	internal sealed class Mapper038 : NesBoardBase
	{
		//configuraton
		private int prg_mask, chr_mask;
		//state
		private int prg, chr;
		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER038":
				case "UNL-PCI556":
					break;
				default:
					return false;
			}
			AssertPrg(128);
			AssertChr(32);
			AssertVram(0);
			AssertWram(0);
			prg_mask = Cart.PrgSize / 32 - 1;
			chr_mask = Cart.ChrSize / 8 - 1;
			SetMirrorType(Cart.PadH, Cart.PadV);
			return true;
		}

		public override byte ReadPrg(int addr)
		{
			return Rom[addr + (prg << 15)];
		}

		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
			{
				return Vrom[addr + (chr << 13)];
			}
			else return base.ReadPpu(addr);
		}

		private void writereg(byte value)
		{
			prg = value & 3 & prg_mask;
			chr = (value >> 2) & 3 & chr_mask;
		}

		// the standard way to access this register is at 7000:7fff, but due to
		// hardware design, f000:ffff also works
		public override void WritePrg(int addr, byte value)
		{
			//if ((addr & 0x7000) == 0x7000)
			//	writereg(value);
		}

		public override void WriteWram(int addr, byte value)
		{
			if ((addr & 0x1000) == 0x1000)
				writereg(value);
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(chr), ref chr);
			ser.Sync(nameof(prg), ref prg);
		}
	}
}
