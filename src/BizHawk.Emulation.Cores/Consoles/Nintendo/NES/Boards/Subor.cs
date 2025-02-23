﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class Subor : NesBoardBase
	{
		private byte[] regs = new byte[4];
		private bool is167;

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER166":
					break;
				case "MAPPER167":
					is167 = true;
					break;
				default:
					return false;
			}

			return true;
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(regs), ref regs, false);
			ser.Sync(nameof(is167), ref is167);
		}

		public override void WritePrg(int addr, byte value)
		{
			regs[(addr >> 13) & 0x03] = value;
		}

		public override byte ReadPrg(int addr)
		{
			int basea, bank;
			basea = ((regs[0] ^ regs[1]) & 0x10) << 1;
			bank = (regs[2] ^ regs[3]) & 0x1f;

			if ((regs[1] & 0x08) > 0)
			{
				bank &= 0xFE;
				if (is167)
				{
					if (addr < 0x4000)
					{
						return Rom[((basea + bank + 1) * 0x4000) + (addr & 0x3FFF)];
					}
					else
					{
						return Rom[((basea + bank + 0) * 0x4000) + (addr & 0x3FFF)];
					}
				}
				else
				{
					if (addr < 0x4000)
					{
						return Rom[((basea + bank + 0) * 0x4000) + (addr & 0x3FFF)];
					}
					else
					{
						return Rom[((basea + bank + 1) * 0x4000) + (addr & 0x3FFF)];
					}
				}
			}
			else
			{
				if ((regs[1] & 0x04) > 0)
				{
					if (addr < 0x4000)
					{
						return Rom[(0x1F * 0x4000) + (addr & 0x3FFF)];
					}
					else
					{
						return Rom[((basea + bank) * 0x4000) + (addr & 0x3FFF)];
					}
				}
				else
				{
					if (addr < 0x4000)
					{
						return Rom[((basea + bank) * 0x4000) + (addr & 0x3FFF)];
					}
					else
					{
						if (is167)
						{
							return Rom[(0x20 * 0x4000) + (addr & 0x3FFF)];
						}
						else
						{
							return Rom[(0x07 * 0x4000) + (addr & 0x3FFF)];
						}
					}
				}
			}
		}
	}
}
