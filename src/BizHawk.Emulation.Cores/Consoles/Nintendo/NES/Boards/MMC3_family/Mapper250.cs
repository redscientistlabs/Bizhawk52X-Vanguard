﻿namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	// Time Diver Avenger (Unl)
	// MMC3 with slightly different write scheme
	// presumably the board contains an MMC3 clone with some unique edge case behavior; unknown
	internal sealed class Mapper250 : MMC3Board_Base
	{
		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER250":
					break;
				default:
					return false;
			}
			BaseSetup();
			return true;
		}

		public override void WritePrg(int addr, byte value)
		{
			base.WritePrg(addr & 0x6000 | addr >> 10 & 1, (byte)(addr & 0xff));
		}
	}
}
