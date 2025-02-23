using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	// aka NAMCOT-3446
	// just a mapper206 with a few lines changed;
	// but easiest described in code with a separate, independent class
	internal sealed class Mapper076 : NesBoardBase
	{
		// config
		private int chr_bank_mask_2k;

		private int prg_bank_mask_8k;
		// state
		private int[] prg = new int[4];
		private int[] chr = new int[4];
		private int port;

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "NAMCOT-3446": // Megami Tensei: Digital Devil Story
				case "MAPPER076":
					break;
				default:
					return false;
			}

			SetMirrorType(EMirrorType.Vertical);
			chr_bank_mask_2k = Cart.ChrSize / 2 - 1;
			prg_bank_mask_8k = Cart.PrgSize / 8 - 1;

			prg[3] = prg_bank_mask_8k;
			prg[2] = prg_bank_mask_8k - 1;

			Cart.WramSize = 0;

			return true;
		}

		public override void WritePrg(int addr, byte value)
		{
			switch (addr & 1)
			{
				case 0:
					port = value & 7;
					break;
				case 1:
					switch (port)
					{
						case 6: prg[0] = value & 63 & prg_bank_mask_8k; break;
						case 7: prg[1] = value & 63 & prg_bank_mask_8k; break;

						case 2: chr[0] = value & 63 & chr_bank_mask_2k; break;
						case 3: chr[1] = value & 63 & chr_bank_mask_2k; break;
						case 4: chr[2] = value & 63 & chr_bank_mask_2k; break;
						case 5: chr[3] = value & 63 & chr_bank_mask_2k; break;
					}
					break;
			}
		}

		public override byte ReadPrg(int addr)
		{
			return Rom[addr & 0x1fff | prg[addr >> 13] << 13];
		}

		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
				return Vrom[addr & 0x7ff | chr[addr >> 11] << 11];
			return base.ReadPpu(addr);
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(prg), ref prg, false);
			ser.Sync(nameof(chr), ref chr, false);
			ser.Sync(nameof(port), ref port);
		}
	}
}
