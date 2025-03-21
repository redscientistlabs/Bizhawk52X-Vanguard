﻿using BizHawk.Common;
using BizHawk.Common.NumberExtensions;

namespace BizHawk.Emulation.Cores.Nintendo.NES
{
	//iNES Mapper 92
	//Example Games:
	//Example Games:
	//--------------------------
	//Moero!! Pro Soccer
	//Moero!! Pro Yakyuu '88 - Ketteiban

	//Near Identical to Jaleco JF 17, except for a slight PRG setup

	internal sealed class JALECO_JF_19 : NesBoardBase
	{
		//configuration
		private int prg_bank_mask_16k;
		private int chr_bank_mask_8k;

		//state
		private int latch;
		private byte[] prg_banks_16k = new byte[2];
		private byte[] chr_banks_8k = new byte[1];

		public override bool Configure(EDetectionOrigin origin)
		{
			switch (Cart.BoardType)
			{
				case "MAPPER092":
					break;
				case "JALECO-JF-19":
					break;
				default:
					return false;
			}

			prg_bank_mask_16k = (Cart.PrgSize / 16) - 1;
			chr_bank_mask_8k = (Cart.ChrSize / 8) - 1;

			SetMirrorType(Cart.PadH, Cart.PadV);

			prg_banks_16k[0] = 0;
			chr_banks_8k[0] = 0;
			SyncMap();

			return true;
		}

		private void SyncMap()
		{
			ApplyMemoryMapMask(prg_bank_mask_16k, prg_banks_16k);
			ApplyMemoryMapMask(chr_bank_mask_8k, chr_banks_8k);
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync(nameof(latch), ref latch);
			ser.Sync(nameof(prg_banks_16k), ref prg_banks_16k, false);
			ser.Sync(nameof(chr_banks_8k), ref chr_banks_8k, false);
		}

		public override void WritePrg(int addr, byte value)
		{
			//Console.WriteLine("MAP {0:X4} = {1:X2}", addr, value);

			value = HandleNormalPRGConflict(addr, value);
			/*
			int command = value >> 6;
			switch (command)
			{
				case 0:
					if (latch == 1)
						chr_banks_8k[0] = (byte)(value & 0xF);
					else if (latch == 2)
						prg_banks_16k[1] = (byte)(value & 0xF);
					SyncMap();
					break;
				default:
					latch = command;
					break;
			}
			*/

			// the important change here is that the chr and prg bank latches get filled on the rising edge, not falling
			if (value.Bit(6) && !latch.Bit(6))
				chr_banks_8k[0] = (byte)(value & 0xF);
			if (value.Bit(7) && !latch.Bit(7))
				prg_banks_16k[1] = (byte)(value & 0xF);
			latch = value;
			SyncMap();
		}

		public override byte ReadPrg(int addr)
		{
			addr = ApplyMemoryMap(14, prg_banks_16k, addr);
			return Rom[addr];
		}

		public override byte ReadPpu(int addr)
		{
			if (addr < 0x2000)
			{
				addr = ApplyMemoryMap(13, chr_banks_8k, addr);
				return base.ReadPPUChr(addr);
			}
			else return base.ReadPpu(addr);
		}
	}
}
