﻿using System;

namespace Jellyfish.Virtu
{
	internal abstract class Disk525
	{
		protected readonly byte[] Data;
		private readonly byte[] Original;
		public bool IsWriteProtected;

		protected Disk525(byte[] data, bool isWriteProtected)
		{
			Data = data;
			Original = (byte[])data.Clone();
			IsWriteProtected = isWriteProtected;
		}

		public virtual void Sync(IComponentSerializer ser)
		{
			ser.SyncDelta("DataDelta", Original, Data);
			ser.Sync(nameof(IsWriteProtected), ref IsWriteProtected);
		}

		public void DeltaUpdate(Action<byte[], byte[]> callback)
		{
			callback(Data, Original);
		}

		public static Disk525 CreateDisk(string name, byte[] data, bool isWriteProtected)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.EndsWith(".do", StringComparison.OrdinalIgnoreCase) ||
				name.EndsWith(".dsk", StringComparison.OrdinalIgnoreCase)) // assumes dos sector skew
			{
				return new DiskDsk(data, isWriteProtected, SectorSkew.Dos);
			}

			if (name.EndsWith(".nib", StringComparison.OrdinalIgnoreCase))
			{
				return new DiskNib(data, isWriteProtected);
			}

			if (name.EndsWith(".po", StringComparison.OrdinalIgnoreCase))
			{
				return new DiskDsk(data, isWriteProtected, SectorSkew.ProDos);
			}

			return null;
		}

		public abstract void ReadTrack(int number, int fraction, byte[] buffer);
		public abstract void WriteTrack(int number, int fraction, byte[] buffer);

		public const int SectorCount = 16;
		public const int SectorSize = 0x100;
		public const int TrackSize = 0x1A00;
	}
}
