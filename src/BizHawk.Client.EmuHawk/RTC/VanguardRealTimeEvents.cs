using RTCV.CorruptCore;

namespace RTCV.BizhawkVanguard
{
	public class VanguardRealTimeEvents : RTCV.CorruptCore.IRealTime
	{
		public bool SupportsRewind { get; set; } = true;
		public bool SupportsForwarding { get; set; } = true;
		public bool SupportsFastForwarding { get; set; } = true;

		public object GetDisplayForm
		{
			get
			{
				return GlobalWin.MainForm;
			}
		}

		public bool OverrideBackgroundInput { get; set; } = false;

		public event EventHandler<RealTimeEventArgs> StepHandler;
		public event EventHandler GameLoaded;
		public event EventHandler GameClosed;

		public void ON_STEP(bool _isForwarding, bool _isRewinding, bool _isFastForwarding)
		{
			StepHandler?.Invoke(this, new RealTimeEventArgs()
			{
				isForwarding = _isForwarding,
				isRewinding = _isRewinding,
				isFastForwarding = _isFastForwarding,
			});

		}

		public void LOAD_GAME() => GameLoaded?.Invoke(this, EventArgs.Empty);
		public void GAME_CLOSED() => GameClosed?.Invoke(this, EventArgs.Empty);
	}
}
