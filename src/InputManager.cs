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
            InputEventList _eventList = InputEventList.None;

            KeyboardState _keyboardState = Keyboard.GetState();

            if (_keyboardState.IsKeyDown(Keys.W)) _eventList |= InputEventList.MoveForwards;
            if (_keyboardState.IsKeyDown(Keys.A)) _eventList |= InputEventList.MoveLeft;
            if (_keyboardState.IsKeyDown(Keys.S)) _eventList |= InputEventList.MoveBackwards;
            if (_keyboardState.IsKeyDown(Keys.D)) _eventList |= InputEventList.MoveRight;
			if (_keyboardState.IsKeyDown(Keys.Space)) _eventList |= InputEventList.Jump;
            if (_keyboardState.IsKeyDown(Keys.LeftShift)) _eventList |= InputEventList.Sprint;

            MouseState _mouseState = Mouse.GetState();

            if (_mouseState.LeftButton == ButtonState.Pressed) _eventList |= InputEventList.MouseLeftClick;
            if (_mouseState.RightButton == ButtonState.Pressed) _eventList |= InputEventList.MouseRightClick;
            if (_mouseState.MiddleButton == ButtonState.Pressed) _eventList |= InputEventList.MouseMiddleClick;
            
            InputEventArgs _args = new InputEventArgs(_eventList,
                new Point(_mouseState.X, _mouseState.Y),             
                new Point(_mouseState.X - PreviousMousePosition.X, _mouseState.Y - PreviousMousePosition.Y));

           // Mouse.SetPosition(800 / 2, 480 / 2); //Mouse in die Mitte des Bildschirms einfangen, schlecht zum debuggen
            _mouseState = Mouse.GetState();
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

        public override string ToString() => $"Event: { Events }\r\nMousePosition: { MousePosition }\r\nMouseMovement: { MouseMovement }";
    }

    [Flags]
    public enum InputEventList
    {
        None = 0,

        MoveForwards = 0x01,
        MoveLeft = 0x02,
        MoveRight = 0x04,
        MoveBackwards = 0x08,

        MouseLeftClick = 0x10,
        MouseRightClick = 0x20,
        MouseMiddleClick = 0x40,

        Jump = 0x80,
        Sprint = 0x100
    }
}