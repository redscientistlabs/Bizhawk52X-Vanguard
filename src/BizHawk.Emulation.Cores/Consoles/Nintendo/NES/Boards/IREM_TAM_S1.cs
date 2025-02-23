﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	//iNES Mapper 97
	//Kaiketsu Yanchamaru (Kid Niki 1)

	internal sealed class IREM_TAM_S1 : NesBoardBase
	{
		private int prg_bank_mask_16k;
		private byte prg_bank_16k;
		private byte[] prg_banks_16k = new byte[2];

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER097":
					break;
				case "IREM-TAM-S1":
					break;
				default:
					return false;
			}
			SetMirrorType(Cart.PadH, Cart.PadV);
			prg_bank_mask_16k = (Cart.PrgSize / 16) - 1;
			prg_banks_16k[0] = 0xFF;
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
			prg_banks_16k[1] = prg_bank_16k;
		}

		public override void WritePrg(int addr, byte value)
		{
			prg_bank_16k = (byte)(value & 15);
			SyncPRG();

			int m = value >> 6;
			switch (m)
			{
				case 0:
					SetMirrorType(EMirrorType.OneScreenA);
					break;
				case 1:
					SetMirrorType(EMirrorType.Horizontal);
					break;
				case 2:
					SetMirrorType(EMirrorType.Vertical);
					break;
				case 3:
					SetMirrorType(EMirrorType.OneScreenB);
					break;
			}
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
