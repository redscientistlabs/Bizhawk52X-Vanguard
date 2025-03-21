using System.Collections.Generic;
using System.Linq;

using BizHawk.Common;
using BizHawk.Common.ReflectionExtensions;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.ColecoVision
{
	public class ColecoVisionControllerDeck
	{
		public ColecoVisionControllerDeck(string controller1Name, string controller2Name)
		{
			Port1 = ControllerCtors.TryGetValue(controller1Name, out var ctor1)
				? ctor1(1)
				: throw new InvalidOperationException($"Invalid controller type: {controller1Name}");
			Port2 = ControllerCtors.TryGetValue(controller2Name, out var ctor2)
				? ctor2(2)
				: throw new InvalidOperationException($"Invalid controller type: {controller2Name}");

			Definition = new("ColecoVision Basic Controller")
			{
				BoolButtons = Port1.Definition.BoolButtons
					.Concat(Port2.Definition.BoolButtons)
					.Concat(new[]
					{
						"Power", "Reset"
					})
					.ToList()
			};

			foreach (var kvp in Port1.Definition.Axes) Definition.Axes.Add(kvp);
			foreach (var kvp in Port2.Definition.Axes) Definition.Axes.Add(kvp);

			Definition.MakeImmutable();
		}

		public float wheel1;
		public float wheel2;

		public float temp_wheel1;
		public float temp_wheel2;

		public byte ReadPort1(IController c, bool leftMode, bool updateWheel)
		{
			wheel1 = Port1.UpdateWheel(c);

			return Port1.Read(c, leftMode, updateWheel, temp_wheel1);
		}

		public byte ReadPort2(IController c, bool leftMode, bool updateWheel)
		{
			wheel2 = Port2.UpdateWheel(c);

			return Port2.Read(c, leftMode, updateWheel, temp_wheel2);
		}

		public ControllerDefinition Definition { get; }

		public void SyncState(Serializer ser)
		{
			ser.BeginSection(nameof(Port1));
			Port1.SyncState(ser);
			ser.Sync(nameof(temp_wheel1), ref temp_wheel1);
			ser.EndSection();

			ser.BeginSection(nameof(Port2));
			ser.Sync(nameof(temp_wheel2), ref temp_wheel2);
			Port2.SyncState(ser);
			ser.EndSection();
		}

		public IPort Port1 { get; }
		public IPort Port2 { get; }

		private static IReadOnlyDictionary<string, Func<int, IPort>> _controllerCtors;

		public static IReadOnlyDictionary<string, Func<int, IPort>> ControllerCtors => _controllerCtors
			??= new Dictionary<string, Func<int, IPort>>
			{
				[typeof(UnpluggedController).DisplayName()] = portNum => new UnpluggedController(portNum),
				[typeof(StandardController).DisplayName()] = portNum => new StandardController(portNum),
				[typeof(ColecoTurboController).DisplayName()] = portNum => new ColecoTurboController(portNum),
				[typeof(ColecoSuperActionController).DisplayName()] = portNum => new ColecoSuperActionController(portNum)
			};

		public static string DefaultControllerName => typeof(StandardController).DisplayName();
	}

}
