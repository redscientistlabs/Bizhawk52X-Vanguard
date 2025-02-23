﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class Mapper106 : NesBoardBase
	{
		private byte[] regs = new byte[16];

		private int prg_bank_mask_8k;
		private int chr_bank_mask_1k;

		private bool IRQa;
		private int IRQCount;

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER106":
					break;
				default:
					return false;
			}

			regs[0x8] = 0xFF;
			regs[0x9] = 0xFF;
			regs[0xA] = 0xFF;
			regs[0xB] = 0xFF;

			prg_bank_mask_8k = Cart.PrgSize / 8 - 1;
			chr_bank_mask_1k = Cart.ChrSize / 1 - 1;

			return true;
		}

		public override void SyncState(Serializer ser)
		{
			ser.Sync(nameof(regs), ref regs, false);
			ser.Sync(nameof(IRQa), ref IRQa);
			ser.Sync(nameof(IRQCount), ref IRQCount);
			base.SyncState(ser);
		}

		public override void WritePrg(int addr, byte value)
		{
			int a = addr & 0xF;
			switch(a)
			{
				default:
					regs[a] = value;
					SetMirror();
					break;

				// TODO: IRQs
				case 0xD:
					IRQa = false;
					IRQCount = 0;
					IrqSignal = false;
					break;
				case 0xE:
					IRQCount = (IRQCount & 0xFF00) | value;
					break;
				case 0XF:
					IRQCount = (IRQCount & 0x00FF) | (value << 8);
					IRQa = true;
					break;
			}
		}

		private void SetMirror()
		{
			SetMirrorType(regs[0xC].Bit(0) ? EMirrorType.Horizontal : EMirrorType.Vertical);
		}

		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
			{
				int bank = 0;
				if (addr < 0x400)
				{
					bank = regs[0] & 0xFE;
				}
				else if (addr < 0x800)
				{
					bank = regs[1] | 1;
				}
				else if (addr < 0xC00)
				{
					bank = regs[2] & 0xFE;
				}
				else if (addr < 0x1000)
				{
					bank = regs[3] | 1;
				}

				else if (addr < 0x1400)
				{
					bank = regs[4];
				}
				else if (addr < 0x1800)
				{
					bank = regs[5];
				}
				else if (addr < 0x1C00)
				{
					bank = regs[6];
				}
				else
				{
					bank = regs[7];
				}

				bank &= chr_bank_mask_1k;
				return Vrom[(bank << 10) + (addr & 0x3FF)];
			}

			return base.ReadPpu(addr);
		}

		public override byte ReadPrg(int addr)
		{
			int index = ((addr >> 13) & 3) + 8;
			int bank = regs[index] & prg_bank_mask_8k;
			return Rom[(bank << 13) + (addr & 0x1FFF)];
		}

		public override void ClockCpu()
		{
			IrqHook(1);
		}

		private void IrqHook(int a)
		{
			if (IRQa)
			{
				IRQCount += a;
				if (IRQCount > 0x10000)
				{
					IrqSignal = true;
					IRQa = false;
				}
			}
		}
	}
}
