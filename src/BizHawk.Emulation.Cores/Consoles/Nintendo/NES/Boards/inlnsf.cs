﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class INLNSF : NesBoardBase
	{

		// config
		private int prg_bank_mask_4k;

		// state
		private int[] prg = new int[8];

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(prg), ref prg, true);
		}

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER031":
					AssertChr(0, 8);
					if(Cart.ChrSize == 0)
						Cart.VramSize = 8;
					break;
				case "MAPPER0031-00":
					AssertVram(8);
					break;
				default:
					return false;
			}
			SetMirrorType(CalculateMirrorType(Cart.PadH, Cart.PadV));
			AssertPrg(16, 32, 64, 128, 256, 512, 1024);
			Cart.WramSize = 0;
			prg_bank_mask_4k = Cart.PrgSize / 4 - 1;
			prg[7] = prg_bank_mask_4k;
			return true;
		}

		public override void WriteExp(int addr, byte value)
		{
			if (addr >= 0x1000)
				prg[addr & 0x07] = value & prg_bank_mask_4k;
			else
				base.WriteExp(addr, value);
		}

		public override byte ReadPrg(int addr)
		{
			return Rom[prg[(addr & 0x7000) >> 12] << 12 | addr & 0x0fff];
		}
	}
}
