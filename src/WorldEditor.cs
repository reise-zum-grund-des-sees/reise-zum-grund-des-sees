using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    class WorldEditor : IUpdateable
    {
        public Vector3 Position;
        public GraphicsDevice Device;

        public WorldEditor(Vector3 _position, GraphicsDevice _device)
        {
            Position = _position;
            Device = _device;
        }

        double f, b, l, r, u, d;
        const double movementSpeed = 200;
        public UpdateDelegate Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime)
        {
            Vector3 _difference = new Vector3(0, 0, 0);

            f += _passedTime;
            b += _passedTime;
            l += _passedTime;
            r += _passedTime;
            u += _passedTime;
            d += _passedTime;

            if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards) & f > movementSpeed)
            {
                _difference.Z += 1;
                f = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards) & b > movementSpeed)
            {
                _difference.Z -= 1;
                b = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft) & l > movementSpeed)
            {
                _difference.X += 1;
                l = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveRight) & r > movementSpeed)
            {
                _difference.X -= 1;
                r = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveUp) & u > movementSpeed)
            {
                _difference.Y += 1;
                u = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveDown) & d > movementSpeed)
            { 
                _difference.Y -= 1;
                d = 0;
            }


            return (ref GameState _state) =>
            {
                Position += _difference;
                int x = (int)Position.X;
                int y = (int)Position.Y;
                int z = (int)Position.Z;
                Vector3 blockPosition = new Vector3(x, y, z) + new Vector3(0.5f);
                _state.Camera.LookAt = blockPosition;
                _state.Camera.Position = blockPosition + new Vector3(0, 10, -7);

                if (_inputArgs.Events.HasFlag(InputEventList.MouseLeftClick) && _state.World.Blocks[x, y, z] != WorldBlock.Wall)
                {
                    _state.World.Blocks[x, y, z] = WorldBlock.Wall;
                    _state.World.GenerateVertices(Device);
                }
                else if (_inputArgs.Events.HasFlag(InputEventList.MouseRightClick) && _state.World.Blocks[x, y, z] != WorldBlock.None)
                {
                    _state.World.Blocks[x, y, z] = WorldBlock.None;
                    _state.World.GenerateVertices(Device);
                }
            };
        }
    }
}
