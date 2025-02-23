﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class UNIF_BMC_NTD_03 : NesBoardBase
	{
		private int latche;

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "UNIF_BMC-NTD-03":
					break;
				default:
					return false;
			}


			return true;
		}

		public override void SyncState(Serializer ser)
		{
			ser.Sync(nameof(latche), ref latche);
			base.SyncState(ser);
		}

		public override void WritePrg(int addr, byte value)
		{
			latche = addr & 65535;
			SetMirrorType(addr.Bit(10) ? EMirrorType.Horizontal : EMirrorType.Vertical);
		}

		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
			{
				int bank = ((latche & 0x0300) >> 5) | (latche & 7);
				return Vrom[(bank << 13) + addr];
			}

			return base.ReadPpu(addr);
		}

		public override byte ReadPrg(int addr)
		{
			int prg = ((latche >> 10) & 0x1E);

			if (latche.Bit(7))
			{
				int bank = prg | ((latche >> 6) & 1);
				return Rom[(bank << 14) + (addr & 0x3FFF)];
			}
			else
			{
				int bank = prg >> 1;
				return Rom[( bank << 15) + addr];
			}
		}
	}
}
