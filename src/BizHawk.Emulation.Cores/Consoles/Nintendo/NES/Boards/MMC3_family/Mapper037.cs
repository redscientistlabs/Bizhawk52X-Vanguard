﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	// mmc3 multi, PAL, "Super Mario Bros. / Tetris / Nintendo World Cup"
	internal sealed class Mapper037 : MMC3Board_Base
	{
		private int exreg;

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER037":
				case "PAL-ZZ":
					break;
				default:
					return false;
			}
			AssertPrg(256);
			AssertChr(256);
			BaseSetup();
			//mmc3.MMC3Type = ??
			exreg = 0;
			return true;
		}

		public override void SyncState(Serializer ser)
		{
			ser.Sync(nameof(exreg), ref exreg);
			base.SyncState(ser);
		}

		public override void WriteWram(int addr, byte value)
		{
			if (!mmc3.wram_enable || mmc3.wram_write_protect)
				return;
			exreg = value & 7;
			mmc3.Sync(); // unneeded?
		}

		protected override int Get_CHRBank_1K(int addr)
		{
			return base.Get_CHRBank_1K(addr) | (exreg << 5 & 0x80);
		}

		protected override int Get_PRGBank_8K(int addr)
		{
			return (exreg << 2 & 0x10) | ((exreg & 3) == 3 ? 8 : 0) | (base.Get_PRGBank_8K(addr) & (exreg << 1 | 7));
		}
	}
}
