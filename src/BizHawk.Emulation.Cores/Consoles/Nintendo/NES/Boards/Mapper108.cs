﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	// Meikyuu Jiin Dababa (FDS Conversion)
	internal sealed class Mapper108 : NesBoardBase
	{
		private int prg;

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER108":
					break;
				default:
					return false;
			}
			AssertPrg(128);
			AssertChr(0);
			Cart.VramSize = 8;
			AssertWram(0);
			SetMirrorType(Cart.PadH, Cart.PadV);

			return true;
		}

		public override void WritePrg(int addr, byte value)
		{
			if (addr < 0xFFF
				|| addr >= 0x7000) // hack ported from FCEUX to support Bubble Bobble (FDS Conversion, Kaiser Hacked) (Unl) [p1][!]
			{
				prg = value & 15;
			}
		}

		public override byte ReadPrg(int addr)
		{
			return Rom[addr | 0x18000];
		}

		public override byte ReadWram(int addr)
		{
			return Rom[addr | prg << 13];
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(prg), ref prg);
		}
	}
}
