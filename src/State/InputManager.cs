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
        private bool leftMouseDown = false, rightMouseDown = false, middleMouseDown = false;

#if LINUX
        private bool ignoreMouseBecauseLinuxImplementationIsShit = false;

        private const int RESET_MOUSE_DIFFERENCE = 200;
        private const int MAGIC_MOUSE_PROBABLY_GOT_RESET_DIFFERENCE = 100;
#endif

        public InputEventArgs Update(GameFlags _flags, Rectangle _windowBounds)
        {
            InputEventList _eventList = InputEventList.None;

            KeyboardState _keyboardState = Keyboard.GetState();

            if (_keyboardState.IsKeyDown(Keys.W)) _eventList |= InputEventList.MoveForwards;
            if (_keyboardState.IsKeyDown(Keys.A)) _eventList |= InputEventList.MoveLeft;
            if (_keyboardState.IsKeyDown(Keys.S)) _eventList |= InputEventList.MoveBackwards;
            if (_keyboardState.IsKeyDown(Keys.D)) _eventList |= InputEventList.MoveRight;
            if (_keyboardState.IsKeyDown(Keys.E)) _eventList |= InputEventList.Interact;

            if (_keyboardState.IsKeyDown(Keys.Q)) _eventList |= InputEventList.MoveUp;
            if (_keyboardState.IsKeyDown(Keys.E)) _eventList |= InputEventList.MoveDown;

            if (_keyboardState.IsKeyDown(Keys.Space)) _eventList |= InputEventList.Jump;
            if (_keyboardState.IsKeyDown(Keys.LeftShift)) _eventList |= InputEventList.Sprint;
            if (_keyboardState.IsKeyDown(Keys.B)) _eventList |= InputEventList.Delete;
            if (_keyboardState.IsKeyDown(Keys.Q)) _eventList |= InputEventList.Return;

            if (_keyboardState.IsKeyDown(Keys.D1))
            {
                _eventList |= InputEventList.LeichterBlock;
                _eventList |= InputEventList.PlaceWater1;
            }
            if (_keyboardState.IsKeyDown(Keys.D2))
            {
                _eventList |= InputEventList.MittelschwererBlock;
                _eventList |= InputEventList.PlaceWater2;
            }
            if (_keyboardState.IsKeyDown(Keys.D3))
            {
                _eventList |= InputEventList.SchwererBlock;
                _eventList |= InputEventList.PlaceWater3;
            }
            if (_keyboardState.IsKeyDown(Keys.D4))
            {
                _eventList |= InputEventList.PlaceWater4;
            }
            if (_keyboardState.IsKeyDown(Keys.D5))
            {
                _eventList |= InputEventList.PlaceLever;
            }
            if (_keyboardState.IsKeyDown(Keys.D6))
            {
                _eventList |= InputEventList.PlaceSpike;
            }
            MouseState _mouseState = Mouse.GetState();

            if (_mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!leftMouseDown)
                {
                    _eventList |= InputEventList.MouseLeftClick;
                    leftMouseDown = true;
                }
            }
            else leftMouseDown = false;
            if (_mouseState.RightButton == ButtonState.Pressed)
            {
                if (!rightMouseDown)
                {
                    _eventList |= InputEventList.MouseRightClick;
                    rightMouseDown = true;
                }
            }
            else rightMouseDown = false;
            if (_mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (!middleMouseDown)
                {
                    _eventList |= InputEventList.MouseMiddleClick;
                    middleMouseDown = true;
                }
            }
            else middleMouseDown = false;

            Point _mouseMovement = new Point(_mouseState.X - PreviousMousePosition.X, _mouseState.Y - PreviousMousePosition.Y);

#if LINUX
            if (ignoreMouseBecauseLinuxImplementationIsShit)
            {
                if (Vector2.Distance(new Vector2(_mouseState.X, _mouseState.Y), _windowBounds.Size.ToVector2() * 0.5f) < MAGIC_MOUSE_PROBABLY_GOT_RESET_DIFFERENCE)
                    ignoreMouseBecauseLinuxImplementationIsShit = false;
                else
                {
                    DebugHelper.Log("Ignoring Mouse: " + _mouseState.X + ", " + _mouseState.Y);
                    Mouse.SetPosition(_windowBounds.Size.X / 2, _windowBounds.Size.Y / 2); //Mouse in die Mitte des Bildschirms einfangen, schlecht zum debuggen
                    _mouseMovement = new Point(0, 0);
                }
            }
#endif

            Vector2 _mouseMovementRelative = _mouseMovement.ToVector2() / _windowBounds.Size.ToVector2();

            InputEventArgs _args = new InputEventArgs(_eventList,
                new Point(_mouseState.X, _mouseState.Y),
                _mouseMovement, _mouseMovementRelative);

#if LINUX
            if (!ignoreMouseBecauseLinuxImplementationIsShit)
            {
                if (_flags.HasFlag(GameFlags.GameRunning) &&
                    Vector2.Distance(new Vector2(_mouseState.X, _mouseState.Y), _windowBounds.Size.ToVector2() * 0.5f) > RESET_MOUSE_DIFFERENCE)
                {
                    DebugHelper.Log("Mouse Position set to " + _windowBounds.Size.X / 2 + ", " + _windowBounds.Size.Y / 2);
                    Mouse.SetPosition(_windowBounds.Size.X / 2, _windowBounds.Size.Y / 2); //Mouse in die Mitte des Bildschirms einfangen, schlecht zum debuggen
                    PreviousMousePosition = new Point(_windowBounds.Size.X / 2, _windowBounds.Size.Y / 2);
                    ignoreMouseBecauseLinuxImplementationIsShit = true;
                }
                else
                {
                    PreviousMousePosition.X = _mouseState.X;
                    PreviousMousePosition.Y = _mouseState.Y;
                }
            }
#elif WINDOWS
            if (_flags.HasFlag(GameFlags.GameRunning))
            {
                Mouse.SetPosition(_windowBounds.Size.X / 2, _windowBounds.Size.Y / 2);
                PreviousMousePosition = new Point(_windowBounds.Size.X / 2, _windowBounds.Size.Y / 2);
            }
#endif

            return _args;
        }
    }

    struct InputEventArgs
    {
        public readonly InputEventList Events;
        public readonly Point MousePosition;
        public readonly Point MouseMovement;
        public readonly Vector2 MouseMovementRelative;

        public InputEventArgs(InputEventList _events, Point _mousePos, Point _mouseMov, Vector2 _mouseMovRel)
        {
            Events = _events;
            MousePosition = _mousePos;
            MouseMovement = _mouseMov;
            MouseMovementRelative = _mouseMovRel;
        }

        public override string ToString() => $"Event: { Events }\r\nMousePosition: { MousePosition }\r\nMouseMovement: { MouseMovement }\r\nMouseMovementRelative: { MouseMovementRelative }";
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
        Sprint = 0x100,

        PlaceWall = 0x200,
        RemoveBlock = 0x400,
        Delete = 0x400,

        MoveUp = 0x800,
        MoveDown = 0x1000,

        LeichterBlock = 0x2000,
        MittelschwererBlock = 0x4000,
        SchwererBlock = 0x8000,

        PlaceWater1 = 0x10000,
        PlaceWater2 = 0x20000,
        PlaceWater3 = 0x40000,
        PlaceWater4 = 0x80000,
        PlaceLever = 0x100000,
        PlaceSpike = 0x200000,

        Interact = 0x400000,
        Return = 0x800000
    }
}
