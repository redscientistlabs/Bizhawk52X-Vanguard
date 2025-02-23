﻿namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class Mapper055 : NesBoardBase
	{
		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER055": // Nestopia calls this mapper 55, I know of no dumps with the designation though
				case "UNIF_BTL-MARIO1-MALEE2":
					break;
				default:
					return false;
			}

			Wram = new byte[0x800];
			SetMirrorType(EMirrorType.Vertical);
			return true;
		}

		public override byte ReadWram(int addr)
		{
			if (addr < 0x1000)
			{
				return Rom[0x8000 + (addr & 0x7FF)];
			}

			return Wram[(addr & 0x7FF)];
		}

		public override void WriteWram(int addr, byte value)
		{
			if (addr >= 0x1000)
			{
				Wram[(addr & 0x7FF)] = value;
			}
		}
	}
}
