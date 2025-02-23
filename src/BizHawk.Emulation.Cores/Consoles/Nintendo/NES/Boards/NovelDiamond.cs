﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class NovelDiamond : NesBoardBase
	{
		private int prg;
		private int chr;

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER054": // ??
				case "UNIF_BMC-NovelDiamond9999999in1": // works
					break;
				default:
					return false;
			}
			AssertPrg(128);
			AssertChr(64);
			SetMirrorType(Cart.PadH, Cart.PadV);
			return true;
		}

		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
				return Vrom[addr | chr << 13];
			return base.ReadPpu(addr);
		}

		public override byte ReadPrg(int addr)
		{
			return Rom[addr | prg << 15];
		}

		public override void WritePrg(int addr, byte value)
		{
			prg = addr & 3;
			chr = addr & 7;
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(prg), ref prg);
			ser.Sync(nameof(chr), ref chr);
		}
	}
}
