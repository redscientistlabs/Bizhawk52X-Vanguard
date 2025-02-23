﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.PCEngine
{
	public sealed partial class PCEngine
	{
		private void SyncState(Serializer ser)
		{
			ser.BeginSection(nameof(PCEngine));
			Cpu.SyncState(ser);
			VCE.SyncState(ser);
			VDC1.SyncState(ser, 1);
			PSG.SyncState(ser);

			if (SuperGrafx)
			{
				VPC.SyncState(ser);
				VDC2.SyncState(ser, 2);
			}

			if (TurboCD)
			{
				ADPCM.SyncState(ser);
				CDAudio.SyncState(ser);
				SCSI.SyncState(ser);

				ser.Sync("CDRAM", ref CDRam, false);
				if (SuperRam != null)
				{
					ser.Sync("SuperRAM", ref SuperRam, false);
				}

				if (ArcadeCard)
				{
					ArcadeCardSyncState(ser);
				}
			}

			ser.Sync("RAM", ref Ram, false);
			ser.Sync(nameof(IOBuffer), ref IOBuffer);
			ser.Sync(nameof(CdIoPorts), ref CdIoPorts, false);
			ser.Sync(nameof(BramLocked), ref BramLocked);

			ser.Sync("Frame", ref _frame);
			ser.Sync("Lag", ref _lagCount);
			ser.Sync("IsLag", ref _isLag);
			if (Cpu.ReadMemory21 == ReadMemorySF2)
			{
				ser.Sync(nameof(SF2MapperLatch), ref SF2MapperLatch);
			}

			if (PopulousRAM != null)
			{
				ser.Sync(nameof(PopulousRAM), ref PopulousRAM, false);
			}

			if (BRAM != null)
			{
				ser.Sync(nameof(BRAM), ref BRAM, false);
			}

			ser.EndSection();

			if (ser.IsReader)
			{
				SyncAllByteArrayDomains();
			}
		}
	}
}
