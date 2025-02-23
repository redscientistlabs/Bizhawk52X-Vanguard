﻿using System.IO;

using BizHawk.Common;

namespace BizHawk.Emulation.Cores.ColecoVision
{
	public partial class ColecoVision
	{
		private void SyncState(Serializer ser)
		{
			byte[] core = null;
			if (ser.IsWriter)
			{
				using var ms = new MemoryStream();
				ms.Close();
				core = ms.ToArray();
			}

			ser.BeginSection("Coleco");
			_cpu.SyncState(ser);
			_vdp.SyncState(ser);
			ControllerDeck.SyncState(ser);
			PSG.SyncState(ser);
			SGM_sound.SyncState(ser);
			ser.Sync("UseSGM", ref use_SGM);
			ser.Sync(nameof(is_MC), ref is_MC);
			ser.Sync(nameof(MC_bank), ref MC_bank);
			ser.Sync("EnableSGMhigh", ref enable_SGM_high);
			ser.Sync("EnableSGMlow", ref enable_SGM_low);
			ser.Sync("Port_0x53", ref port_0x53);
			ser.Sync("Port_0x7F", ref port_0x7F);
			ser.Sync("RAM", ref _ram, false);
			ser.Sync(nameof(SGM_high_RAM), ref SGM_high_RAM, false);
			ser.Sync(nameof(SGM_low_RAM), ref SGM_low_RAM, false);
			ser.Sync("Frame", ref _frame);
			ser.Sync("LagCount", ref _lagCount);
			ser.Sync("IsLag", ref _isLag);

			ser.EndSection();

			if (ser.IsReader)
			{
				SyncAllByteArrayDomains();
			}
		}
	}
}
