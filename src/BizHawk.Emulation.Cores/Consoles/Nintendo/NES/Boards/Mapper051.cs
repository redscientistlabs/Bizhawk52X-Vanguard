﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class Mapper051 : NesBoardBase
	{
		private int _bank;
		private int _mode = 2;

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER051":
					break;
				default:
					return false;
			}

			SetMirrorType(Cart.PadH, Cart.PadV);
			return true;
		}

		public override void NesSoftReset()
		{
			_bank = 0;
			_mode = 2;
			base.NesSoftReset();
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync("bank", ref _bank);
			ser.Sync("mode", ref _mode);
		}

		public override byte ReadWram(int addr)
		{
			int prgBank8k;
			if ((_mode & 0x02) > 0)
			{
				prgBank8k = ((_bank & 7) << 2) | 0x23;
			}
			else
			{
				prgBank8k = ((_bank & 4) << 2) | 0x2F;
			}

			return Rom[(prgBank8k * 0x2000) + addr];
		}

		public override byte ReadPrg(int addr)
		{
			int prgBank16k_8;
			int prgBank16k_C;

			int prgBank;

			if ((_mode & 0x02) > 0)
			{
				prgBank16k_8 = (_bank << 1) | 0;
				prgBank16k_C = (_bank << 1) | 1;

				prgBank = _bank << 1;
			}
			else
			{
				prgBank16k_8 = (_bank << 1) | (_mode >> 4);
				prgBank16k_C = ((_bank & 0xC) << 1) | 7;
			}

			if (addr < 0x4000)
			{
				return Rom[(prgBank16k_8 * 0x4000) + (addr & 0x3FFF)];
			}
			else
			{
				return Rom[(prgBank16k_C * 0x4000) + (addr & 0x3FFF)];
			}
		}

		public override void WriteWram(int addr, byte value)
		{
			if (addr < 0x2000)
			{
				_mode = value & 0x012;
				SyncMirroring();
			}
			else
			{
				base.WriteWram(addr, value);
			}
		}

		public override void WritePrg(int addr, byte value)
		{
			_bank = value & 0x0F;
			if ((addr & 0x4000) > 0)
			{
				_mode = (_mode & 0x02) | (value & 0x10);
			}

			SyncMirroring();
		}

		private void SyncMirroring()
		{
			if (_mode == 0x12)
			{
				SetMirrorType(EMirrorType.Horizontal);
			}
			else
			{
				SetMirrorType(EMirrorType.Vertical);
			}
		}
	}
}
