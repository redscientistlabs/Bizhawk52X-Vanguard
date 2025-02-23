﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	//game=shanghai ; chip=sunsoft-2 ; pcb=SUNSOFT-3R
	//this is confusing. see docs/sunsoft.txt
	internal sealed class Sunsoft2_Mapper93 : NesBoardBase
	{
		private int prg_bank_mask_16k;
		private byte prg_bank_16k;
		private byte[] prg_banks_16k = new byte[2];

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER093":
					break;
				case "SUNSOFT-2":
					if (Cart.Pcb != "SUNSOFT-3R") return false;
					break;
				case "SUNSOFT-1":
					if (Cart.Pcb != "SUNSOFT-4") return false;
					return false; // this has been moved to Sunsoft1_Alt
				default:
					return false;
			}

			SetMirrorType(Cart.PadH, Cart.PadV);
			prg_bank_mask_16k = (Cart.PrgSize / 16) - 1;
			prg_banks_16k[1] = 0xFF;
			return true;
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(prg_bank_mask_16k), ref prg_bank_mask_16k);
			ser.Sync(nameof(prg_bank_16k), ref prg_bank_16k);
			ser.Sync(nameof(prg_banks_16k), ref prg_banks_16k, false);
		}

		private void SyncPRG()
		{
			prg_banks_16k[0] = prg_bank_16k;
		}

		public override void WritePrg(int addr, byte value)
		{
			prg_bank_16k = (byte)((value >> 4) & 15);
			SyncPRG();

			// there is no mirroring control on this board; only a hardwired H\V
		}

		public override byte ReadPrg(int addr)
		{
			int bank_16k = addr >> 14;
			int ofs = addr & ((1 << 14) - 1);
			bank_16k = prg_banks_16k[bank_16k];
			bank_16k &= prg_bank_mask_16k;
			addr = (bank_16k << 14) | ofs;
			return Rom[addr];
		}
	}
}
