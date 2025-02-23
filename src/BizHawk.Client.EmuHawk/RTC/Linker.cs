using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores;

namespace RTCV.BizhawkVanguard
{

	public static class ClassicLinker
	{

		public static Action<string> logCallback = null;
		public static IMainFormForApi mainForm = null;
		public static DisplayManagerBase displayManager = null;
		public static InputManager inputManager = null;
		public static IMovieSession movieSession = null;
		public static ToolManager toolManager = null;
		public static Config config = null;
		public static IEmulator emulator = null;
		public static IGameInfo game = null;

		//public IEmuClientApi EmuClient => (IEmuClientApi)Libraries[typeof(IEmuClientApi)];
	}

	public static class Global
	{
		public static class CoreConfig
		{

			public static bool NES_InQuickNES
			{
				get { return ClassicLinker.config.PreferredCores["NES"] == CoreNames.QuickNes; }
				set
				{
					if (value)
					{
						ClassicLinker.config.PreferredCores["NES"] = CoreNames.QuickNes;
					}
					else
					{
						ClassicLinker.config.PreferredCores["NES"] = CoreNames.NesHawk;
					}
				}
			}

			public static bool GB_UseGBHawk
			{
				get { return ClassicLinker.config.PreferredCores["GB"] == CoreNames.GbHawk; }
				set
				{
					if (value)
					{
						ClassicLinker.config.PreferredCores["GB"] = CoreNames.GbHawk;
						ClassicLinker.config.PreferredCores["GBC"] = CoreNames.GbHawk;
					}
					else
					{
						ClassicLinker.config.PreferredCores["GB"] = CoreNames.Gambatte;
						ClassicLinker.config.PreferredCores["GBC"] = CoreNames.Gambatte;
					}
				}
			}

			public static bool SGB_UseBsnes
			{
				get { return ClassicLinker.config.PreferredCores["SGB"] == CoreNames.Bsnes; }
				set
				{
					if (value)
					{
						ClassicLinker.config.PreferredCores["SGB"] = CoreNames.Bsnes;
					}
					else
					{
						ClassicLinker.config.PreferredCores["SGB"] = CoreNames.Snes9X;
					}
				}
			}

			public static bool SNES_InSnes9x
			{
				get { return ClassicLinker.config.PreferredCores["SNES"] == CoreNames.Snes9X; }
				set
				{
					if (value)
					{
						ClassicLinker.config.PreferredCores["SNES"] = CoreNames.Snes9X;
					}
					else
					{
						ClassicLinker.config.PreferredCores["SNES"] = CoreNames.Bsnes;
					}
				}
			}
			public static bool GBA_UsemGBA
			{
				get { return true; }
				set
				{
					ClassicLinker.config.PreferredCores["GBA"] = CoreNames.Mgba;
				}
			}
		}

		public static class ClassicConfig
		{
			public static bool HexEditorAllowBackgroundInput { get { return ClassicLinker.config.AcceptBackgroundInput; } }

		}
		public static Config Config { get { return ClassicLinker.config; } }
		public static IEmulator Emulator { get { return ClassicLinker.emulator; } }
		public static IGameInfo Game { get { return ClassicLinker.game; } }

	}

	public static class GlobalWin
	{
		public static Sound Sound { get { return ((MainForm) ClassicLinker.mainForm).Sound; } }
		public static ToolManager Tools { get { return ((MainForm) ClassicLinker.mainForm).Tools; } }
		public static OSDManager OSD { get { return ClassicLinker.displayManager.OSD; } }
		public static MainForm mainForm { get { return (MainForm) ClassicLinker.mainForm; } }
		public static MainForm MainForm { get { return (MainForm) ClassicLinker.mainForm; } }



	}

	public static class PathManager
	{

		public static string FilesystemSafeName(IGameInfo game)
		{
			return EmulatorExtensions.FilesystemSafeName(game);
		}

	}
}
