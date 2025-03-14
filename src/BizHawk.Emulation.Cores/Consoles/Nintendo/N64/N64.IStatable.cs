using System.IO;

using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Nintendo.N64.NativeApi;

namespace BizHawk.Emulation.Cores.Nintendo.N64
{
	public partial class N64 : IStatable
	{
		public bool AvoidRewind => false;

		public void SaveStateBinary(BinaryWriter writer)
		{
			byte[] data = SaveStatePrivateBuff;
			int bytes_used = api.SaveState(data);

			//RTC_Hijack return out if there's no data here as we changed api.SaveState to return -1 if it failed
			if (bytes_used == -1)
				return;

			writer.Write(bytes_used);
			writer.Write(data, 0, bytes_used);

			byte[] saveram = api.SaveSaveram();
			writer.Write(saveram);
			if (saveram.Length != mupen64plusApi.kSaveramSize)
			{
				throw new InvalidOperationException("Unexpected N64 SaveRam size");
			}

			// other variables
			writer.Write(IsLagFrame);
			writer.Write(LagCount);
			writer.Write(Frame);
		}

		public void LoadStateBinary(BinaryReader reader)
		{
			int length = reader.ReadInt32();
			if (length < 16788288)
			{
				//RTC_HIJACK: Add message (RTC: Try toggling the expansion pack setting)
				throw new SavestateSizeMismatchException("Wrong N64 savestate size (RTC: Try toggling the expansion pack setting)");
			}

			reader.Read(SaveStatePrivateBuff, 0, length);
			byte[] data = SaveStatePrivateBuff;

			api.LoadState(data);

			reader.Read(SaveStatePrivateBuff, 0, mupen64plusApi.kSaveramSize);
			api.LoadSaveram(SaveStatePrivateBuff);

			// other variables
			IsLagFrame = reader.ReadBoolean();
			LagCount = reader.ReadInt32();
			Frame = reader.ReadInt32();
		}

		private readonly byte[] SaveStatePrivateBuff = new byte[16788288 + 1024];
	}
}
