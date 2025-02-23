﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	//AKA mapper 184
	//Sunsoft-1 chips, EXCEPT for fantasy zone.
	//this is confusing. see docs/sunsoft.txt
	internal sealed class Sunsoft1 : NesBoardBase
	{
		private int chr_mask;
		private int left_piece = 0;
		private int right_piece = 3;

		public override bool Configure(EDetectionOrigin origin)
		{
			//configure
			switch (Cart.BoardType)
			{
				case "MAPPER184":
					break;

				case "SUNSOFT-1":
					//this will catch fantasy zone, which isn't emulated the same as the other SUNSOFT-1 boards
					if (Cart.Pcb == "SUNSOFT-4")
						return false;
					break;
				default:
					return false;
			}
			chr_mask = (Cart.ChrSize / 4) - 1;
			SetMirrorType(Cart.PadH, Cart.PadV);
			return true;
		}



		public override byte ReadPpu(int addr)
		{

			if (addr < 0x1000)
			{
				return Vrom[(addr & 0xFFF) + (left_piece * 0x1000)];
			}
			else if (addr < 0x2000)
			{
				return Vrom[(addr & 0xFFF) + (right_piece * 0x1000)];
			}

			return base.ReadPpu(addr);
		}

		public override void WriteWram(int addr, byte value)
		{
			left_piece = value & 7 & chr_mask;
			// the bank at ppu $1000 has only 2 selection bits.  the high bit is frozen to 1
			// this doesn't matter in practice because the only game on this board with 32K CHR,
			// wing of madoola, writes a '1' to the unread bit.
			right_piece = (value >> 4 & 3 | 4) & chr_mask;
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(left_piece), ref left_piece);
			ser.Sync(nameof(right_piece), ref right_piece);
		}
	}
}
