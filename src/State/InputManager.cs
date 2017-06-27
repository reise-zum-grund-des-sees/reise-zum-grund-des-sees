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
            if (_keyboardState.IsKeyDown(Keys.R)) _eventList |= InputEventList.Rotate;

            if (_keyboardState.IsKeyDown(Keys.F12))
            {
                if (_keyboardState.IsKeyDown(Keys.LeftControl)) _eventList |= InputEventList.Replay;
                else _eventList |= InputEventList.Record;
            }

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
                if (_keyboardState.IsKeyDown(Keys.LeftControl))
                    _eventList |= InputEventList.PlaceWater4Infinite;
                else
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
            if (_keyboardState.IsKeyDown(Keys.D7))
            {
                _eventList |= InputEventList.PlacePressurePlate;
            }
            if (_keyboardState.IsKeyDown(Keys.D8))
            {
                _eventList |= InputEventList.PlaceMovingBlock;
            }
            if (_keyboardState.IsKeyDown(Keys.D9))
            {
                _eventList |= InputEventList.PlaceMovingBlockEnd;            
            }
            MouseState _mouseState = Mouse.GetState();

            if (_mouseState.LeftButton == ButtonState.Pressed)
            {
                _eventList |= InputEventList.MouseLeft;
                if (!leftMouseDown)
                {
                    _eventList |= InputEventList.MouseLeftClick;
                    leftMouseDown = true;
                }
            }
            else leftMouseDown = false;
            if (_mouseState.RightButton == ButtonState.Pressed)
            {
                _eventList |= InputEventList.MouseRight;
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
    public enum InputEventList : Int64
    {
        None                = 0,

        MoveForwards        = 1,
        MoveLeft            = 1 << 1,
        MoveRight           = 1 << 2,
        MoveBackwards       = 1 << 3,

        MouseLeftClick      = 1 << 4,
        MouseRightClick     = 1 << 5,
        MouseMiddleClick    = 1 << 6,

        Jump                = 1 << 7,
        Sprint              = 1 << 8,

        PlaceWall           = 1 << 9,
        RemoveBlock         = 1 << 10,
        Delete              = 1 << 11,

        MoveUp              = 1 << 12,
        MoveDown            = 1 << 13,

        LeichterBlock       = 1 << 14,
        MittelschwererBlock = 1 << 15,
        SchwererBlock       = 1 << 16,

        PlaceWater1         = 1 << 17,
        PlaceWater2         = 1 << 18,
        PlaceWater3         = 1 << 19,
        PlaceWater4         = 1 << 20,
        PlaceLever          = 1 << 21,
        PlaceSpike          = 1 << 22,

        Interact            = 1 << 23,
        Return              = 1 << 24,
        PlacePressurePlate  = 1 << 25,

        PlaceWater4Infinite = 1 << 26,

        MouseLeft           = 1 << 27,
        MouseRight          = 1 << 28,

        Record              = 1 << 29,
        Replay              = 1 << 30,

        Rotate              = 1 << 31,
        PlaceMovingBlock    = 1 << 7,
        PlaceMovingBlockEnd = 1 << 8
    }
}
