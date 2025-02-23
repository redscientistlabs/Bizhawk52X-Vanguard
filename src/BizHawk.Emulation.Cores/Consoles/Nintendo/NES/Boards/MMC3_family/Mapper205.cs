﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	internal sealed class Mapper205 : MMC3Board_Base
	{
		//Mapper 205 info: http://wiki.nesdev.com/w/index.php/INES_Mapper_205

		private int block;

		public override bool Configure(EDetectionOrigin origin)
		{
			//analyze board type
			switch (Cart.BoardType)
			{
				case "MAPPER205":
					break;
				default:
					return false;
			}

			block = 0;

			BaseSetup();
			return true;
		}

		public override void SyncState(Serializer ser)
		{
			ser.Sync("mode", ref block);
			base.SyncState(ser);
		}

		public override void WriteWram(int addr, byte value)
		{
			block = value & 0x03;
		}

		public override byte ReadPrg(int addr)
		{
			int bank_8k = Get_PRGBank_8K(addr);
			bank_8k &= prg_mask;

			switch (block)
			{
				case 0:
					bank_8k &= 0x1F;
					bank_8k |= 0x00;
					break;
				case 1:
					bank_8k &= 0x1F;
					bank_8k |= 0x10;
					break;
				case 2:
					bank_8k &= 0x0F;
					bank_8k |= 0x20;
					break;
				case 3:
					bank_8k &= 0x0F;
					bank_8k |= 0x30;
					break;
			}

			addr = (bank_8k << 13) | (addr & 0x1FFF);
			return Rom[addr];
		}

		private int MapCHR2(int addr)
		{
			int bank_1k = Get_CHRBank_1K(addr);
			bank_1k &= chr_mask;
			switch (block)
			{
				case 0:
					bank_1k &= 0xFF;
					bank_1k |= 0x00;
					break;
				case 1:
					bank_1k &= 0xFF;
					bank_1k |= 0x80;
					break;
				case 2:
					bank_1k &= 0x7F;
					bank_1k |= 0x100;
					break;
				case 3:
					bank_1k &= 0x7F;
					bank_1k |= 0x180;
					break;
			}
			addr = (bank_1k << 10) | (addr & 0x3FF);
			return addr;
		}

		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
			{
				addr = MapCHR2(addr);
				if (Vrom != null)
					return Vrom[addr + extra_vrom];
				else return Vram[addr];
			}
			else return base.ReadPpu(addr);
		}

		public override void NesSoftReset()
		{
			block = 0;
			base.NesSoftReset();
		}
	}
}
