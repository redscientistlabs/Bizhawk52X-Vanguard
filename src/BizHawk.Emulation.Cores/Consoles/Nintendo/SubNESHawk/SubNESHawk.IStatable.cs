﻿using System.IO;

using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.SubNESHawk
{
	public partial class SubNESHawk : IStatable
	{
		private readonly IStatable _nesStatable;

		public bool AvoidRewind => _nesStatable.AvoidRewind;

		public void SaveStateBinary(BinaryWriter bw)
		{
			_nesStatable.SaveStateBinary(bw);
			// other variables
			SyncState(new Serializer(bw));
		}

		public void LoadStateBinary(BinaryReader br)
		{
			_nesStatable.LoadStateBinary(br);
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
			ser.Sync(nameof(reset_cycle), ref reset_cycle);
			ser.Sync(nameof(reset_cycle_int), ref reset_cycle_int);
		}
	}
}
