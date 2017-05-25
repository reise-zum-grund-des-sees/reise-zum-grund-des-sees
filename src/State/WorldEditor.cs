﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ReiseZumGrundDesSees
{
    class WorldEditor : IUpdateable, IPositionObject, IRenderable
    {
        public Vector3 Position { get; set; }
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private Model cursorModel;

        public WorldEditor(Vector3 _position, ContentManager _content)
        {
            Position = _position;
            content = _content;
            cursorModel = _content.Load<Model>("cursor");
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

            if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards) & b > movementSpeed)
            {        
                _difference.Z -= 1;
                b = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards) & f > movementSpeed)
            {
                _difference.Z += 1;
                f = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft) & r > movementSpeed)
            {
                _difference.X -= 1;
                r = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveRight) & l > movementSpeed)
            {    
                _difference.X += 1;
                l = 0;
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

            float _direction = (float)Math.Round(-_view.CamAngle * 4 / MathHelper.TwoPi) * MathHelper.TwoPi / 4f;
            _difference = Vector3.Transform(_difference, Quaternion.CreateFromAxisAngle(Vector3.Up, _direction));


            return (ref GameState _state) =>
            {
                Position += _difference;
                int x = (int)Position.X;
                int y = (int)Position.Y;
                int z = (int)Position.Z;
                Vector3 blockPosition = new Vector3(x, y, z) + new Vector3(0.5f);
                /*_state.Camera.LookAt = blockPosition;
                _state.Camera.Position = blockPosition + new Vector3(0, 10, -7);*/

                if (_inputArgs.Events.HasFlag(InputEventList.MouseLeftClick) && _state.World.Blocks[x, y, z] == WorldBlock.None && !_inputArgs.Events.HasFlag(InputEventList.LeichterBlock))
                    _state.World.Blocks[x, y, z] = WorldBlock.Wall;

                else if (_inputArgs.Events.HasFlag(InputEventList.MouseRightClick) && _state.World.Blocks[x, y, z] != WorldBlock.None)
                    _state.World.Blocks[x, y, z] = WorldBlock.None;

                else if (_inputArgs.Events.HasFlag(InputEventList.PlaceWater1) && _state.World.Blocks[x, y, z] != WorldBlock.Water1)
                    _state.World.Blocks[x, y, z] = WorldBlock.Water1;

                else if (_inputArgs.Events.HasFlag(InputEventList.PlaceWater2) && _state.World.Blocks[x, y, z] != WorldBlock.Water2)
                    _state.World.Blocks[x, y, z] = WorldBlock.Water2;

                else if (_inputArgs.Events.HasFlag(InputEventList.PlaceWater3) && _state.World.Blocks[x, y, z] != WorldBlock.Water3)
                    _state.World.Blocks[x, y, z] = WorldBlock.Water3;

                else if (_inputArgs.Events.HasFlag(InputEventList.PlaceWater4) && _state.World.Blocks[x, y, z] != WorldBlock.Water4)
                    _state.World.Blocks[x, y, z] = WorldBlock.Water4;

                else if (_inputArgs.Events.HasFlag(InputEventList.MouseLeftClick) && _inputArgs.Events.HasFlag(InputEventList.LeichterBlock) && (_state.World.Blocks[x, y, z] == WorldBlock.None || _state.World.Blocks[x, y, z] == WorldBlock.Lever))//Schalter ertmal auf Taste 1 und Linksklick belegen
                {

                    if (_state.World.Blocks[x, y, z] == WorldBlock.Lever)
                        Lever.AtPosition(Position).Rotation += Math.PI / 2;

                    else
                    {
                        _state.World.Blocks[x, y, z] = WorldBlock.Lever;
                        Lever lever = new Lever(content, Position);
                    }

                }
            };
        }

        public void Initialize(GraphicsDevice _graphicsDevice)
        {
            graphicsDevice = _graphicsDevice;
        }

        public void Render(Matrix _viewMatrix, Matrix _projectionMatrix)
        {
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            foreach (ModelMesh mesh in cursorModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    Vector3 _blockPosition = new Vector3((int)Position.X + 0.5f, (int)Position.Y + 0.5f, (int)Position.Z + 0.5f);
                    effect.World = Matrix.CreateTranslation(_blockPosition);

                    effect.View = _viewMatrix;
                    effect.Projection = _projectionMatrix;

                }

                mesh.Draw();
            }
        }
    }
}