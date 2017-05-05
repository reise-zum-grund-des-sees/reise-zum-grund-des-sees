using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ReiseZumGrundDesSees
{
	class InputManager
	{
		public InputEventArgs Update()
		{
			InputEventList _eventList = 0;

			KeyboardState _keyboardState = Keyboard.GetState();

			if (_keyboardState.IsKeyDown(Keys.W)) _eventList |= InputEventList.MoveForwards;
			if (_keyboardState.IsKeyDown(Keys.A)) _eventList |= InputEventList.MoveLeft;
			if (_keyboardState.IsKeyDown(Keys.S)) _eventList |= InputEventList.MoveBackwards;
			if (_keyboardState.IsKeyDown(Keys.D)) _eventList |= InputEventList.MoveRight;

			MouseState _mouseState = Mouse.GetState();

			return new InputEventArgs(_eventList);
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