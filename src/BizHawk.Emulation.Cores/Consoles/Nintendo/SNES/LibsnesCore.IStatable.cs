﻿using System.IO;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.SNES
{
	public partial class LibsnesCore : IStatable
	{
		public bool AvoidRewind => false;

		public void SaveStateBinary(BinaryWriter writer)
		{
			Api.SaveStateBinary(writer);
			writer.Write(IsLagFrame);
			writer.Write(LagCount);
			writer.Write(Frame);
		}

		public void LoadStateBinary(BinaryReader reader)
		{
			Api.LoadStateBinary(reader);
			IsLagFrame = reader.ReadBoolean();
			LagCount = reader.ReadInt32();
			Frame = reader.ReadInt32();
			Api.QUERY_set_cdl(_currCdl);
		}
	}
}
