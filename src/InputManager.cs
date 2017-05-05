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
		private Point PreviousMousePosition;
		public InputEventArgs Update()
		{
			InputEventList _eventList = 0;

			KeyboardState _keyboardState = Keyboard.GetState();

			if (_keyboardState.IsKeyDown(Keys.W)) _eventList |= InputEventList.MoveForwards;
			if (_keyboardState.IsKeyDown(Keys.A)) _eventList |= InputEventList.MoveLeft;
			if (_keyboardState.IsKeyDown(Keys.S)) _eventList |= InputEventList.MoveBackwards;
			if (_keyboardState.IsKeyDown(Keys.D)) _eventList |= InputEventList.MoveRight;

			MouseState _mouseState = Mouse.GetState();

			if (_mouseState.LeftButton == ButtonState.Pressed) _eventList |= InputEventList.MouseLeftClick;
			if (_mouseState.RightButton == ButtonState.Pressed) _eventList |= InputEventList.MouseRightClick;
			if (_mouseState.MiddleButton == ButtonState.Pressed) _eventList |= InputEventList.MouseMiddleClick;

			InputEventArgs _args = new InputEventArgs(_eventList,
				new Point(_mouseState.X, _mouseState.Y),
				new Point(_mouseState.X - PreviousMousePosition.X, _mouseState.Y - PreviousMousePosition.Y));

			PreviousMousePosition.X = _mouseState.X;
			PreviousMousePosition.Y = _mouseState.Y;

			return _args;
		}
	}

	struct InputEventArgs
	{
		public readonly InputEventList Events;
		public readonly Point MousePosition;
		public readonly Point MouseMovement;

		public InputEventArgs(InputEventList _events, Point _mousePos, Point _mouseMov)
		{
			Events = _events;
			MousePosition = _mousePos;
			MouseMovement = _mouseMov;
		}
	}

	[Flags]
	public enum InputEventList
	{
		MoveForwards = 1,
		MoveLeft = 2,
		MoveRight = 4,
		MoveBackwards = 8,

		MouseLeftClick = 0xF0,
		MouseRightClick = 0xF1,
		MouseMiddleClick = 0xF2
	}
}