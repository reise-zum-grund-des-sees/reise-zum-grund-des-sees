using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
	class InputManager
	{
		public void Update()
		{
			throw new NotImplementedException();
		}
	}

	struct InputEventArgs
	{
		public readonly InputEventList Events;

		public InputEventArgs(InputEventList _events)
		{
			Events = _events;
		}
	}

	[Flags]
	public enum InputEventList
	{
		MoveForwards = 1,
		MoveLeft = 2,
		MoveRight = 4,
		MoveBackwards = 8
	}
}