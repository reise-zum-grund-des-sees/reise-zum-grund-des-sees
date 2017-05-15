﻿using System;
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
        public InputEventArgs Update(Rectangle _windowBounds)
        {
            InputEventList _eventList = InputEventList.None;

            KeyboardState _keyboardState = Keyboard.GetState();

            if (_keyboardState.IsKeyDown(Keys.W)) _eventList |= InputEventList.MoveForwards;
            if (_keyboardState.IsKeyDown(Keys.A)) _eventList |= InputEventList.MoveLeft;
            if (_keyboardState.IsKeyDown(Keys.S)) _eventList |= InputEventList.MoveBackwards;
            if (_keyboardState.IsKeyDown(Keys.D)) _eventList |= InputEventList.MoveRight;
            
            if (_keyboardState.IsKeyDown(Keys.Q)) _eventList |= InputEventList.MoveUp;
            if (_keyboardState.IsKeyDown(Keys.E)) _eventList |= InputEventList.MoveDown;

            if (_keyboardState.IsKeyDown(Keys.Space)) _eventList |= InputEventList.Jump;
            if (_keyboardState.IsKeyDown(Keys.LeftShift)) _eventList |= InputEventList.Sprint;

            if (_keyboardState.IsKeyDown(Keys.D1)) _eventList |= InputEventList.LeichterBlock;
            if (_keyboardState.IsKeyDown(Keys.D2)) _eventList |= InputEventList.MittelschwererBlock;
            if (_keyboardState.IsKeyDown(Keys.D3)) _eventList |= InputEventList.SchwererBlock;
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
            
            Vector2 _moueMovementRelative = _mouseMovement.ToVector2() / _windowBounds.Size.ToVector2();

            InputEventArgs _args = new InputEventArgs(_eventList,
                new Point(_mouseState.X, _mouseState.Y),
                _mouseMovement, _moueMovementRelative);

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

        MoveUp = 0x800,
        MoveDown = 0x1000,

        LeichterBlock = 0x2000,
        MittelschwererBlock = 0x30000,
        SchwererBlock = 0x4000
    }
}
