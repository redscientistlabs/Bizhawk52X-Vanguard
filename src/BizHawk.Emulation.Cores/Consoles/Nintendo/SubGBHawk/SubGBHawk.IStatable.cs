﻿using System.IO;

using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.SubGBHawk
{
	public partial class SubGBHawk : IStatable
	{
		private readonly IStatable _GBStatable;

		public bool AvoidRewind => _GBStatable.AvoidRewind;

		public void SaveStateBinary(BinaryWriter bw)
		{
			_GBStatable.SaveStateBinary(bw);
			// other variables
			SyncState(new Serializer(bw));
		}

		public void LoadStateBinary(BinaryReader br)
		{
			_GBStatable.LoadStateBinary(br);
			// other variables
			SyncState(new Serializer(br));
		}

		private void SyncState(Serializer ser)
		{
			ser.Sync("Lag", ref _lagCount);
			ser.Sync("Frame", ref _frame);
			ser.Sync("IsLag", ref _isLag);
			ser.Sync(nameof(pass_a_frame), ref pass_a_frame);
			ser.Sync(nameof(reset_frame), ref reset_frame);
			ser.Sync(nameof(pass_new_input), ref pass_new_input);
			ser.Sync(nameof(current_cycle), ref current_cycle);
			ser.Sync(nameof(frame_cycle), ref frame_cycle);
			ser.Sync(nameof(input_frame_length), ref input_frame_length);
			ser.Sync(nameof(input_frame_length_int), ref input_frame_length_int);
			ser.Sync(nameof(_cycleCount), ref _cycleCount);
			ser.Sync(nameof(_GBCore.audio.master_audio_clock), ref _GBCore.audio.master_audio_clock);
		}
	}
}
