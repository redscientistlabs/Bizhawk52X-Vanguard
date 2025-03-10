﻿using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.Gameboy
{
	public partial class GambatteLink : IInputPollable
	{
		public int LagCount { get; set; }

		public bool IsLagFrame { get; set; }

		public IInputCallbackSystem InputCallbacks => _inputCallbacks;

		private readonly InputCallbackSystem _inputCallbacks = new InputCallbackSystem();
	}
}
