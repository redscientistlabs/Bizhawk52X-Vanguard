﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class Mapper015 : NesBoardBase
	{
		//configuration
		private int prg_bank_mask_8k;

		//state
		private byte[] prg_banks_8k = new byte[4];

		public override bool Configure(EDetectionOrigin origin)
		{
			//configure
			switch (Cart.BoardType)
			{
				case "MAPPER015":
					break;
				default:
					return false;
			}
			prg_bank_mask_8k = (Cart.PrgSize / 8) - 1;

			// not a maskable size (BF=10111111)
			// so just set mask to FF and hope for the best
			if (Cart.PrgSize==192)
			{
				prg_bank_mask_8k = 0xFF;
			}

			prg_banks_8k[0] = 0;
			prg_banks_8k[1] = 1;
			prg_banks_8k[2] = 2;
			prg_banks_8k[3] = 3;
			ApplyMemoryMapMask(prg_bank_mask_8k, prg_banks_8k);

			SetMirrorType(EMirrorType.Horizontal);

			return true;
		}

		public override byte ReadPrg(int addr)
		{
			addr = ApplyMemoryMap(13, prg_banks_8k, addr);
			return Rom[addr];
		}

		public override void WritePrg(int addr, byte value)
		{
			int mode = addr & 3;
			int prg_high = value & 0x3F;
			bool prg_low = value.Bit(7);
			int prg_low_val = 0;
			if (mode==2)
				prg_low_val = prg_low ? 1 : 0;
			bool mirror = value.Bit(6);
			SetMirrorType(mirror ? EMirrorType.Horizontal : EMirrorType.Vertical);

			switch(mode)
			{
				case 0:
					prg_banks_8k[0] = (byte)((prg_high * 2 + 0) ^ prg_low_val);
					prg_banks_8k[1] = (byte)((prg_high * 2 + 1) ^ prg_low_val);
					prg_high |= 0x01;
					prg_banks_8k[2] = (byte)((prg_high * 2 + 0) ^ prg_low_val);
					prg_banks_8k[3] = (byte)((prg_high * 2 + 1) ^ prg_low_val);
					break;
				case 1:
					prg_banks_8k[0] = (byte)((prg_high*2+0) ^ prg_low_val);
					prg_banks_8k[1] = (byte)((prg_high*2+1) ^ prg_low_val);
					prg_banks_8k[2] = 0xFE;
					prg_banks_8k[3] = 0xFF;
					//maybe all 4?
					break;
				case 2:
					prg_banks_8k[0] = (byte)((prg_high * 2 + 0) ^ prg_low_val);
					prg_banks_8k[1] = (byte)((prg_high * 2 + 0) ^ prg_low_val);
					prg_banks_8k[2] = (byte)((prg_high * 2 + 0) ^ prg_low_val);
					prg_banks_8k[3] = (byte)((prg_high * 2 + 0) ^ prg_low_val);
					break;
				case 3:
					prg_banks_8k[0] = (byte)((prg_high * 2 + 0) ^ prg_low_val);
					prg_banks_8k[1] = (byte)((prg_high * 2 + 1) ^ prg_low_val);
					prg_banks_8k[2] = (byte)((prg_high * 2 + 0) ^ prg_low_val);
					prg_banks_8k[3] = (byte)((prg_high * 2 + 1) ^ prg_low_val);
					break;
			}

			ApplyMemoryMapMask(prg_bank_mask_8k, prg_banks_8k);
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(prg_banks_8k), ref prg_banks_8k, false);
		}
	}
}
