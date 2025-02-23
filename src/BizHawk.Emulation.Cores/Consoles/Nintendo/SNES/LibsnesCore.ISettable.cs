﻿using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.SNES
{
	public partial class LibsnesCore : ISettable<LibsnesCore.SnesSettings, LibsnesCore.SnesSyncSettings>
	{
		public SnesSettings GetSettings()
		{
			return _settings.Clone();
		}

		IBSNESForGfxDebugger.SettingsObj IBSNESForGfxDebugger.GetSettings()
			=> GetSettings();

		public SnesSyncSettings GetSyncSettings()
		{
			return _syncSettings.Clone();
		}

		public PutSettingsDirtyBits PutSettings(SnesSettings o)
		{
			bool refreshNeeded = o.Palette != _settings.Palette;
			_settings = o;
			if (refreshNeeded)
			{
				RefreshPalette();
			}

			return PutSettingsDirtyBits.None;
		}

		void IBSNESForGfxDebugger.PutSettings(IBSNESForGfxDebugger.SettingsObj s)
			=> PutSettings((SnesSettings) s);

		public PutSettingsDirtyBits PutSyncSettings(SnesSyncSettings o)
		{
			bool ret = o.LeftPort != _syncSettings.LeftPort
				|| o.RightPort != _syncSettings.RightPort
				|| o.LimitAnalogChangeSensitivity != _syncSettings.LimitAnalogChangeSensitivity
				|| o.RandomizedInitialState != _syncSettings.RandomizedInitialState;

			_syncSettings = o;
			return ret ? PutSettingsDirtyBits.RebootCore : PutSettingsDirtyBits.None;
		}

		private SnesSettings _settings;
		private SnesSyncSettings _syncSettings;

		public class SnesSettings : IBSNESForGfxDebugger.SettingsObj
		{
			public bool ShowBG1_0 { get; set; } = true;
			public bool ShowBG2_0 { get; set; } = true;
			public bool ShowBG3_0 { get; set; } = true;
			public bool ShowBG4_0 { get; set; } = true;
			public bool ShowBG1_1 { get; set; } = true;
			public bool ShowBG2_1 { get; set; } = true;
			public bool ShowBG3_1 { get; set; } = true;
			public bool ShowBG4_1 { get; set; } = true;
			public bool ShowOBJ_0 { get; set; } = true;
			public bool ShowOBJ_1 { get; set; } = true;
			public bool ShowOBJ_2 { get; set; } = true;
			public bool ShowOBJ_3 { get; set; } = true;

			public bool CropSGBFrame { get; set; } = false;
			public bool AlwaysDoubleSize { get; set; } = false;
			public string Palette { get; set; } = "BizHawk";

			public SnesSettings Clone()
			{
				return (SnesSettings)MemberwiseClone();
			}
		}

		public class SnesSyncSettings
		{
			public LibsnesControllerDeck.ControllerType LeftPort { get; set; } = LibsnesControllerDeck.ControllerType.Gamepad;
			public LibsnesControllerDeck.ControllerType RightPort { get; set; } = LibsnesControllerDeck.ControllerType.Unplugged;

			public bool LimitAnalogChangeSensitivity { get; set; } = true;

			public bool RandomizedInitialState { get; set; } = true;

			public SnesSyncSettings Clone()
			{
				return (SnesSyncSettings)MemberwiseClone();
			}
		}
	}
}
