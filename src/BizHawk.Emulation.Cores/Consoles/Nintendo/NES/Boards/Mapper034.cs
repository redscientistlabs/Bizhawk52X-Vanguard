using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class Mapper034 : NesBoardBase
	{
		// zombie board that tries to handle both bxrom and ave-nina at once

		//configuration
		private int prg_bank_mask_32k, chr_bank_mask_4k;

		//state
		private byte[] chr = new byte[2];
		private int prg;


		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER034": // 3-D Battles of World Runner, The (U) [b5].nes
					// TODO: No idea what to assert here
					break;
				default:
					return false;
			}

			Cart.WramSize = 8;
			prg_bank_mask_32k = Cart.PrgSize / 32 - 1;
			chr_bank_mask_4k = Cart.ChrSize / 4 - 1;

			SetMirrorType(Cart.PadH, Cart.PadV);
			chr[1] = 1;

			return true;
		}

		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
				return (Vrom ?? Vram)[addr & 0xfff | chr[addr >> 12] << 12];
			else
				return base.ReadPpu(addr);
		}

		public override byte ReadPrg(int addr)
		{
			return Rom[addr | prg << 15];
		}

		public override void WritePrg(int addr, byte value)
		{
			prg = value & prg_bank_mask_32k;
		}

		public override void WriteWram(int addr, byte value)
		{
			switch (addr)
			{
				case 0x1ffd:
					prg = value & prg_bank_mask_32k;
					break;
				case 0x1ffe:
					chr[0] = (byte)(value & chr_bank_mask_4k);
					break;
				case 0x1fff:
					chr[1] = (byte)(value & chr_bank_mask_4k);
					break;
				default:
					// on NINA, the regs sit on top of WRAM
					base.WriteWram(addr, value);
					break;
			}
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(prg), ref prg);
			ser.Sync(nameof(chr), ref chr, false);
		}
	}
}
