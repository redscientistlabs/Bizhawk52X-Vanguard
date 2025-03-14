﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using BizHawk.Client.EmuHawk.RTC;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Nintendo.N64;
using RTCV.CorruptCore;
using RTCV.NetCore;
using RTCV.NetCore.Commands;
using RTCV.Vanguard;
using static BizHawk.Emulation.Cores.Computers.Amiga.LibUAE.FrameInfo;

namespace RTCV.BizhawkVanguard
{
	public static class Hooks
	{
		//Instead of writing code inside bizhawk, hooks are placed inside of it so will be easier
		//to upgrade BizHawk when they'll release a new version.

		// Here are the keywords for searching hooks and fixes: //RTC_HIJACK

		public static bool disableRTC;
		public static bool isNormalAdvance = false;

		//private static Guid? loadGameToken = null;
		//private static Guid? loadSavestateToken = null;

		public static System.Diagnostics.Stopwatch watch = null;

		public static volatile bool BIZHAWK_ALLOWED_DOUBLECLICK_FULLSCREEN = true;

		public static void CPU_STEP(bool isRewinding, bool isFastForwarding, bool isBeforeStep = false)
		{
			try
			{
				if (disableRTC || ClassicLinker.mainForm.Emulator is NullEmulator)
					return;

				bool runBefore = (bool) (AllSpec.CorruptCoreSpec?[RTCSPEC.STEP_RUNBEFORE.ToString()] ?? false);

				//Return out if it's being called from before the step and we're not on frame 0. If we're on frame 0, then we go as normal
				//If we can't get runbefore, just assume we don't want to run before
				if (isBeforeStep && runBefore == false)
					return;
				if (runBefore)
				{
					AllSpec.CorruptCoreSpec.Update(RTCSPEC.STEP_RUNBEFORE.ToString(), false);
				}

				isNormalAdvance = !(isRewinding || isFastForwarding);

				bool isForward = (!isRewinding && !isFastForwarding);

				// Unique step hooks
				if (isForward)
					STEP_FORWARD();
				else if (isRewinding)
					STEP_REWIND();
				else if (isFastForwarding)
					STEP_FASTFORWARD();

				//Any step hook for corruption
				STEP_CORRUPT(isRewinding, isFastForwarding);
				VanguardCore.RTE_API.ON_STEP(isForward, isRewinding, isFastForwarding);
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();
				MessageBox.Show("Clearing all step blastunits due to an exception within Core_Step().");
				LocalNetCoreRouter.Route(Endpoints.UI, Basic.ErrorDisableAutoCorrupt, false);
				StepActions.ClearStepBlastUnits();
			}
		}

		private static void STEP_FORWARD() //errors trapped by CPU_STEP
		{
			if (disableRTC) return;
		}

		private static void STEP_REWIND() //errors trapped by CPU_STEP
		{
			if (disableRTC) return;

			if (StepActions.ClearStepActionsOnRewind)
				StepActions.ClearStepBlastUnits();
		}

		private static void STEP_FASTFORWARD() //errors trapped by CPU_STEP
		{
			if (disableRTC) return;
		}

		private static void STEP_CORRUPT(bool _isRewinding, bool _isFastForwarding) //errors trapped by CPU_STEP
		{
			if (disableRTC) return;

			bool executeActions = !_isRewinding; //keep active unit executing when forwarding and fast forwarding
			bool performStep = (!_isRewinding && !_isFastForwarding); //don't corrupt when altering time

			RtcClock.StepCorrupt(executeActions, performStep);

		}

		public static void SHOW_CONSOLE(bool show)
		{
			if (disableRTC) return;

			if (show)
				Common.ConsoleHelper.ShowConsole();
			else
				Common.ConsoleHelper.HideConsole();
		}

		public static void CREATE_VMD_FROM_SELECTED_HEXEDITOR(string domain, List<long> allAddresses, int wordSize)
		{
			int allAddrCount = allAddresses.Count;
			if (wordSize > 1) //fills the gap caused by address spacing
				for (int addrPos = 0; addrPos < allAddrCount; addrPos++)
					for (int addedCount = 1; addedCount < wordSize; addedCount++)
					{
						long newAddr = allAddresses[addrPos] + addedCount;
						allAddresses.Add(newAddr);
					}

			var ordered = allAddresses.OrderBy(it => it).ToArray();
			bool contiguous = true;
			long? lastAddress = null;
			int i = 0;

			foreach (long item in ordered)
			{
				if (lastAddress != null) //not the first one
					if (i != (ordered.Length - 1)) //not the last one
						if (item != lastAddress.Value + 1) //checks expected address
							contiguous = false;

				lastAddress = item;
				i++;
			}

			string ToHexString(long n)
			{
				return $"{n:X}";
			}

			string text;
			if (contiguous)
			{
				text = $"{ToHexString(ordered[0])}-{ToHexString(ordered[ordered.Length - 1])}";
			}
			else
			{
				text = string.Join("\n", ordered.Select(it => ToHexString(it)));
			}

			VanguardCore.CreateVmdText(domain, text);

		}


		public static void MAIN_BIZHAWK(string[] args)
		{
			//MessageBox.Show("ATTACH DEBUGGER NOW");

			if (!System.Environment.Is64BitOperatingSystem)
			{
				MessageBox.Show("32-bit operating system detected. Bizhawk requires 64-bit to run. Program will shut down");
				Application.Exit();
			}

			try
			{
				VanguardCore.args = args;

				disableRTC = VanguardCore.args.Contains("-DISABLERTC");

				//VanguardCore.attached = true;
				VanguardCore.attached = VanguardCore.args.Contains("-ATTACHED");

				//BizHawk.Client.EmuHawk.LogConsole.ReleaseConsole();
				RTCV.Common.Logging.StartLogging(VanguardCore.logPath);
				if (args.Contains("-CONSOLE"))
				{
					Common.ConsoleHelper.ShowConsole();
				}
				else
				{
					Common.ConsoleHelper.HideConsole();
				}
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();
			}
		}

		public static void MAINFORM_FORM_LOAD_END()
		{
			try
			{
				if (disableRTC) return;

				VanguardCore.Start();
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();
			}
		}



		public static void MAINFORM_RESIZEEND()
		{
			try
			{
				if (disableRTC) return;

				VanguardCore.SaveBizhawkWindowState();
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();
			}
		}

		public static void MAINFORM_CLOSING()
		{

			if (disableRTC) return;

			//Todo
			//RTC_UICore.CloseAllRtcForms();

		}


		public static void LOAD_GAME_BEGIN()
		{
			try
			{
				if (disableRTC) return;

				isNormalAdvance = false;

				StepActions.ClearStepBlastUnits();
				RtcClock.ResetCount();
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex) == DialogResult.Abort)
					throw new AbortEverythingException();
			}
		}

		private static string lastGameName = "";

		public static void LOAD_GAME_DONE()
		{
			try
			{
				if (disableRTC) return;

				if (AllSpec.UISpec == null)
				{
					CLOSE_GAME();
					GlobalWin.MainForm.CloseRom();
					MessageBox.Show("It appears you haven't connected to StandaloneRTC. Please make sure that the RTC is running and not just Bizhawk.\nIf you have an antivirus, it might be blocking the RTC from launching.\n\nIf you keep getting this message, poke the RTC devs for help (Discord is in the launcher).", "RTC Not Connected");
					return;
				}

				//Glitch Harvester warning for archives

				string uppercaseFilename = GlobalWin.MainForm.CurrentlyOpenRom.ToUpper();
				if (uppercaseFilename.Contains(".ZIP") || uppercaseFilename.Contains(".7Z"))
				{
					MessageBox.Show($"The selected file {Path.GetFileName(uppercaseFilename.Split('|')[0])} is an archive.\nThe RTC does not support archived rom files. Please extract the file then try again.");
					CLOSE_GAME(true);
					return;
				}

				//Load Game vars into RTC_Core
				PathEntry pathEntry = Global.Config.PathEntries[Global.Game.System, "Savestates"] ??
				Global.Config.PathEntries[Global.Game.System, "Base"];

				//Check the config file for if we should reload the game when a savestate is loaded
				bool reload_on_savestate = true;
				var config = VanguardConfigReader.configFile.RELOAD_ON_SAVESTATE;
				switch (BIZHAWK_GET_CURRENTLYLOADEDSYSTEMNAME().ToUpper())
				{
					case "PCENGINE":
						reload_on_savestate = config.PCEngine;

						break;

					case "PLAYSTATION":
						reload_on_savestate = config.Playstation;

						break;

					case "SATURN":

						reload_on_savestate = config.Saturn;

						break;

					case "JAGUAR":
						reload_on_savestate = config.Jaguar;

						break;

					case "NDS":
						reload_on_savestate = config.NDS;

						break;

					case "3DS":
						reload_on_savestate = config.N3DS;

						break;

				}


				//prepare memory domains in advance on bizhawk side
				bool domainsChanged = RefreshDomains(false);

				PartialSpec gameDone = new PartialSpec("VanguardSpec");
				gameDone[VSPEC.SYSTEM] = BIZHAWK_GET_CURRENTLYLOADEDSYSTEMNAME().ToUpper();
				gameDone[VSPEC.GAMENAME] = BIZHAWK_GET_FILESYSTEMGAMENAME();
				gameDone[VSPEC.SYSTEMPREFIX] = BIZHAWK_GET_SAVESTATEPREFIX();
				gameDone[VSPEC.SYSTEMCORE] = BIZHAWK_GET_SYSTEMCORENAME(GlobalWin.MainForm.Game.System);
				gameDone[VSPEC.SYNCSETTINGS] = BIZHAWK_GETSET_SYNCSETTINGS;
				gameDone[VSPEC.OPENROMFILENAME] = GlobalWin.MainForm.CurrentlyOpenRom;
				gameDone[VSPEC.MEMORYDOMAINS_BLACKLISTEDDOMAINS] = VanguardCore.GetBlacklistedDomains(BIZHAWK_GET_CURRENTLYLOADEDSYSTEMNAME().ToUpper());
				gameDone[VSPEC.MEMORYDOMAINS_INTERFACES] = GetInterfaces();
				gameDone[VSPEC.CORE_DISKBASED] = isCurrentCoreDiskBased();
				gameDone[VSPEC.RELOAD_ON_SAVESTATE] = reload_on_savestate;
				AllSpec.VanguardSpec.Update(gameDone);

				//This is local. If the domains changed it propgates over netcore
				LocalNetCoreRouter.Route(Endpoints.CorruptCore, Remote.EventDomainsUpdated, domainsChanged, true);

				if (VanguardCore.GameName != lastGameName)
				{
					LocalNetCoreRouter.Route(Endpoints.UI, Basic.ResetGameProtectionIfRunning, true);
				}
				lastGameName = VanguardCore.GameName;

				RtcCore.InvokeLoadGameDone();
				VanguardCore.RTE_API.LOAD_GAME();
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex) == DialogResult.Abort)
					throw new AbortEverythingException();
			}
		}

		public static void LOAD_GAME_FAILED()
		{
			if (disableRTC) return;

		}

		private static bool CLOSE_GAME_loop_flag = false;

		public static bool AllowCaptureRewindState = true;

		public static void CLOSE_GAME(bool loadDefault = false)
		{
			try
			{
				if (disableRTC) return;

				if (CLOSE_GAME_loop_flag)
					return;

				CLOSE_GAME_loop_flag = true;

				//RTC_Core.AutoCorrupt = false;

				StepActions.ClearStepBlastUnits();

				MemoryDomains.Clear();

				VanguardCore.OpenRomFilename = null;

				if (loadDefault)
					VanguardCore.LoadDefaultRom();

				//RTC_RPC.SendToKillSwitch("UNFREEZE");

				CLOSE_GAME_loop_flag = false;

				RtcCore.InvokeGameClosed();
				VanguardCore.RTE_API.GAME_CLOSED();

			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex) == DialogResult.Abort)
					throw new AbortEverythingException();
			}
		}

		public static void LOAD_SAVESTATE_BEGIN()
		{
			if (disableRTC) return;

		}

		public static void LOAD_SAVESTATE_END()
		{
			if (disableRTC) return;


		}

		public static bool IsAllowedBackgroundInputForm()
		{
			if (disableRTC) return false;

			return VanguardCore.RTE_API.OverrideBackgroundInput || VanguardConnector.IsUIForm() ||
				(Form.ActiveForm is HexEditor && Global.ClassicConfig.HexEditorAllowBackgroundInput);

		}

		public static Bitmap BIZHAWK_GET_SCREENSHOT()
		{
			try
			{
				return GlobalWin.MainForm.MakeScreenshotImage().ToSysdrawingBitmap();
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return null;
			}
		}



		public static string BIZHAWK_GET_FILESYSTEMGAMENAME()
		{
			try
			{
				//This returns the filename of the currently loaded game before extension

				PathEntry pathEntry = GlobalWin.MainForm.Config.PathEntries[GlobalWin.MainForm.Game.System, "Savestates"] ??
				GlobalWin.MainForm.Config.PathEntries[GlobalWin.MainForm.Game.System, "Base"];

				return PathManager.FilesystemSafeName(GlobalWin.MainForm.Game);
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return null;
			}
		}

		public static string BIZHAWK_GET_CURRENTLYLOADEDSYSTEMNAME()
		{
			try
			{
				//returns the currently loaded core's name

				return GlobalWin.MainForm.Game.System;
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return null;
			}
		}

		public static string BIZHAWK_GET_CURRENTLYOPENEDROM()
		{
			try
			{
				//This returns the filename of the currently opened rom

				return GlobalWin.MainForm.CurrentlyOpenRom;
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return null;
			}
		}

		public static bool BIZHAWK_ISNULLEMULATORCORE()
		{
			try
			{
				//This checks if the currently loaded emulator core is the Null emulator

				return ClassicLinker.mainForm.Emulator is NullEmulator;
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex) == DialogResult.Abort)
					throw new AbortEverythingException();

				return false;
			}
		}

		public static bool BIZHAWK_ISMAINFORMVISIBLE()
		{
			try
			{
				return GlobalWin.MainForm.Visible;
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return false;
			}
		}

		public static void BIZHAWK_LOADROM(string RomFile)
		{
			try
			{
				string currentOpenRom = "";
				if ((string) AllSpec.VanguardSpec[VSPEC.OPENROMFILENAME] != "")
					currentOpenRom = (string) AllSpec.VanguardSpec[VSPEC.OPENROMFILENAME];

				bool reload_rom = true;
				if ((AllSpec.VanguardSpec[VSPEC.RELOAD_ON_SAVESTATE]) != null)
					reload_rom = (bool) AllSpec.VanguardSpec[VSPEC.RELOAD_ON_SAVESTATE];

				// Only send the command if it should always be reloaded (e.g. the ROM needs to be uncorrupted),
				// or if we need to load a new rom (since some systems take longer to load every time)
				if (reload_rom || currentOpenRom != RomFile)
				{
					var lra = new LoadRomArgs(new OpenAdvanced_OpenRom { Path = RomFile });
					GlobalWin.MainForm.LoadRom(RomFile, lra);
				}
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return;
			}
		}

		public static void BIZHAWK_OPEN_HEXEDITOR()
		{
			try
			{
				GlobalWin.Tools.Load<HexEditor>();
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return;
			}
		}
		public static void BIZHAWK_OPEN_HEXEDITOR_ADDRESS(MemoryDomainProxy mdp, long address)
		{
			try
			{
				if (mdp?.MD == null)
					return;
				GlobalWin.Tools.Load<HexEditor>();
				GlobalWin.Tools.HexEditor.SetMemoryDomain(((VanguardImplementation.BizhawkMemoryDomain) (mdp.MD)).MD.ToString());
				GlobalWin.Tools.HexEditor.GoToAddress(address);
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return;
			}
		}

		public static Size BIZHAWK_GETSET_MAINFORMSIZE
		{
			get
			{
				return GlobalWin.MainForm.Size;
			}
			set
			{
				GlobalWin.MainForm.Size = value;
			}
		}

		public static Point BIZHAWK_GETSET_MAINFORMLOCATION
		{
			get
			{
				return GlobalWin.MainForm.Location;
			}
			set
			{
				GlobalWin.MainForm.Location = value;
			}
		}


		public static void BIZHAWK_STARTSOUND()
		{
			if (GlobalWin.MainForm != null) { GlobalWin.Sound.StartSound(); }
		}

		public static void BIZHAWK_STOPSOUND()
		{
			if (GlobalWin.MainForm != null) { GlobalWin.Sound.StopSound(); }
		}

		public static void BIZHAWK_MAINFORM_CLOSE()
		{
			GlobalWin.MainForm?.Close();
		}

		public static void BIZHAWK_MAINFORM_FOCUS()
		{
			GlobalWin.MainForm?.Activate();
		}

		public static void BIZHAWK_MAINFORM_SAVECONFIG()
		{
			GlobalWin.MainForm?.SaveConfig();
		}

		public static string BIZHAWK_GET_SAVESTATEPREFIX()
		{
			return PathManager.FilesystemSafeName(GlobalWin.MainForm.Game);
		}

		public static void BIZHAWK_LOADSTATE(string path)
		{
			GlobalWin.MainForm.LoadState(path, Path.GetFileName(path), false);
		}

		public static void BIZHAWK_SAVESTATE(string path, string quickSlotName)
		{
			GlobalWin.MainForm.SaveState(path, quickSlotName, false);
		}


		public static void BIZHAWK_OSDMESSAGE(string message)
		{
			GlobalWin.OSD.AddMessage(message);
		}

		public static void BIZHAWK_SET_SYSTEMCORE(string systemName, string systemCore)
		{
			try
			{
				switch (systemName.ToUpper())
				{
					case "GAMEBOY":
						Global.CoreConfig.SGB_UseBsnes = false;
						Global.CoreConfig.GB_UseGBHawk = systemCore == "gbhawk";

						break;

					case "NES":
						Global.CoreConfig.NES_InQuickNES = systemCore == "quicknes";
						break;

					case "SNES":

						if (systemCore == "bsnes_SGB")
						{
							Global.CoreConfig.SGB_UseBsnes = true;
						}
						else
							Global.CoreConfig.SNES_InSnes9x = systemCore == "snes9x";

						break;

					case "GBA":
						break;

				}
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return;
			}
		}

		public static string BIZHAWK_GET_SYSTEMCORENAME(string systemName)
		{
			try
			{
				switch (systemName.ToUpper())
				{
					case "GAMEBOY":

						if (Global.CoreConfig.GB_UseGBHawk)
							return "gbhawk";
						else
							return "gambatte";

					case "GBC":
						if (Global.CoreConfig.GB_UseGBHawk)
							return "gbhawk";
						else
							return "gambatte";

					case "NES":
						return (Global.CoreConfig.NES_InQuickNES ? "quicknes" : "neshawk");

					case "SNES":

						if (MemoryDomains.MemoryInterfaces.ContainsKey("SGB WRAM"))
							return "bsnes_SGB";

						return (Global.CoreConfig.SNES_InSnes9x ? "snes9x" : "bsnes");

					case "GBA":
						return "mgba";

					case "N64":

						N64SyncSettings ss = Global.Config.GetCoreSyncSettings<N64, N64SyncSettings>()
											 ?? new N64SyncSettings();

						return $"{ss.VideoPlugin}/{ss.Rsp}/{ss.Core}/{(ss.DisableExpansionSlot ? "NoExp" : "Exp")}";

					case "3DS":
						return "3DS";

					default:
						break;
				}

				return systemName;
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex) == DialogResult.Abort)
					throw new AbortEverythingException();

				return null;
			}
		}

		public static string BIZHAWK_GETSET_SYNCSETTINGS
		{
			get
			{
				try
				{
					SettingsAdapter settable = new SettingsAdapter(Global.Emulator, () => true,
						_ => { }, () => true, _ => { });
					if (settable.HasSyncSettings)
					{
						string ss = ConfigService.SaveWithType(settable.GetSyncSettings());
						return ss;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}

				return null;
			}
			set
			{
				try
				{
					SettingsAdapter settable = new SettingsAdapter(Global.Emulator, () => true,
						_ => { }, () => true, _ => { });
					if (settable.HasSyncSettings)
					{
						settable.PutCoreSyncSettings(ConfigService.LoadWithType(value));
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
		}

		public static void BIZHAWK_STARTRECORDAV(string videowritername, string filename, bool unattended)
		{
			GlobalWin.MainForm.RecordAvBase(videowritername, filename, unattended);
		}

		public static void BIZHAWK_STOPRECORDAV()
		{
			GlobalWin.MainForm.StopAv();
		}

		public class MemoryDomainRTCInterface
		{
			[RequiredService]
			public IMemoryDomains MemoryDomains { get; set; }

			[RequiredService]
			private IEmulator Emulator { get; set; }
		}

		public static volatile MemoryDomainRTCInterface MDRI = new MemoryDomainRTCInterface();

		public static bool RefreshDomains(bool updateSpecs = true)
		{
			try
			{
				//Compare the old to the new. If the name and sizes are all the same, don't push that there were changes.
				//We need to compare like this because the domains can change from syncsettings.
				//We only check name and size as those are the only things that can change on the fly
				var oldInterfaces = VanguardCore.MemoryInterfacees;
				var newInterfaces = GetInterfaces();
				bool domainsChanged = oldInterfaces.Length != newInterfaces.Length;

				//Bruteforce it since domains can change inconsistently
				for (int i = 0; i < oldInterfaces.Length; i++)
				{
					if (domainsChanged)
						break;
					if (oldInterfaces[i].Name != newInterfaces[i].Name)
						domainsChanged = true;
					if (oldInterfaces[i].Size != newInterfaces[i].Size)
						domainsChanged = true;
				}

				//We gotta push this no matter what since it's new underlying objects
				if (updateSpecs)
				{
					AllSpec.VanguardSpec.Update(VSPEC.MEMORYDOMAINS_INTERFACES, newInterfaces);
					LocalNetCoreRouter.Route(Endpoints.CorruptCore, Remote.EventDomainsUpdated, domainsChanged, true);
				}
				return domainsChanged;
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex) == DialogResult.Abort)
					throw new AbortEverythingException();

				return false;
			}
		}

		public static MemoryDomainProxy[] GetInterfaces()
		{
			try
			{
				Console.WriteLine($" getInterfaces()");

				List<MemoryDomainProxy> interfaces = new List<MemoryDomainProxy>();

				if (Global.Emulator?.ServiceProvider == null || GlobalWin.MainForm.Emulator is NullEmulator)
					return new MemoryDomainProxy[] { };

				//not sure if this is stiull needed
				ServiceInjector.UpdateServices(Global.Emulator.ServiceProvider, MDRI);

				foreach (MemoryDomain _domain in GlobalWin.MainForm.Emulator.AsMemoryDomains())
					if (!_domain.Name.Contains("Waterbox")) //Waterbox domains are banned
						interfaces.Add(new MemoryDomainProxy(new VanguardImplementation.BizhawkMemoryDomain(_domain)));


				return interfaces.ToArray();
			}
			catch (Exception ex)
			{
				if (VanguardCore.ShowErrorDialog(ex, true) == DialogResult.Abort)
					throw new AbortEverythingException();

				return new MemoryDomainProxy[] { };
			}

		}

		private static bool isCurrentCoreDiskBased()
		{
			return Global.Emulator.HasDriveLight() && Global.Emulator.AsDriveLight().DriveLightEnabled;
		}


	}
}
