using System.IO;
using System.Windows.Forms;
using BizHawk.Common.PathExtensions;
using Newtonsoft.Json;

namespace BizHawk.Client.EmuHawk.RTC
{
	// Root object of config file
	public class ConfigRoot
	{
		public ReloadOnSaveStateConfig RELOAD_ON_SAVESTATE { get; set; }
	}

	// setting for if the game should be reloaded based on the system from config file
	public class ReloadOnSaveStateConfig
	{

		[JsonProperty("PC Engine")]
		public bool PCEngine { get; set; }
		public bool Playstation { get; set; }
		public bool Saturn { get; set; }
		public bool Jaguar { get; set; }
		public bool NDS { get; set; }
		[JsonProperty("3DS")]
		public bool N3DS { get; set; }


	}

	public class VanguardConfigReader
	{
		public static ConfigRoot configFile = GetConfigFile();

		// Read the config file and parse the data into the class
		public static ConfigRoot GetConfigFile()
		{
			string configErrMessage = "Vanguard could not find the target Emulator's config file at " + PathUtils.ExeDirectoryPath + ". Try reinstalling " +
									  "and launching Vanguard.\n\nIf you keep getting this message, poke " +
									  "the RTC devs for help (Discord is in the launcher).";

			if (!File.Exists(PathUtils.ExeDirectoryPath + "\\VanguardConfig.Json"))
			{
				MessageBox.Show(configErrMessage,
				"RTC Not Connected");

				Environment.Exit(-1);
				return null;
			}

			string config = File.ReadAllText(PathUtils.ExeDirectoryPath + "\\VanguardConfig.Json");
			ConfigRoot configFile = JsonConvert.DeserializeObject<ConfigRoot>(config);
			return configFile;
		}
	}
}
