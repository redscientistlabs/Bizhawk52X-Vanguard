﻿namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class Farid_UNROM_8_in_1 : NesBoardBase
	{
		// http://forums.nesdev.com/viewtopic.php?f=9&t=11099

		// state
		private int c; // clock bit for the second 74'161
		private int e; // /load for second 74'161. guaranteed to be 0 on powerup
		private int prginner;
		private int prgouter; // guaranteed to be 0 on powerup

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "UNIF_FARID_UNROM_8-IN-1":
					AssertPrg(1024);
					AssertChr(0);
					break;
				default:
					return false;
			}

			Cart.VramSize = 8;
			SetMirrorType(Cart.PadH, Cart.PadV);
			return true;
		}

		public override void WritePrg(int addr, byte value)
		{
			prginner = value & 7;
			int newc = value >> 7;
			int newe = value >> 3 & 1;

			if (newc > c && e == 0) // latch e and outer
			{
				e = newe;
				prgouter = value >> 4 & 7;
			}
			c = newc;
		}

		public override byte ReadPrg(int addr)
		{
			int bnk = addr >= 0x4000 ? 7 : prginner;
			bnk |= prgouter << 3;
			return Rom[bnk << 14 | addr & 0x3fff];
		}

		public override void SyncState(BizHawk.Common.Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(c), ref c);
			ser.Sync(nameof(e), ref e);
			ser.Sync(nameof(prginner), ref prginner);
			ser.Sync(nameof(prgouter), ref prgouter);
		}

		public override void NesSoftReset()
		{
			e = 0;
			prgouter = 0;
		}
	}
}
