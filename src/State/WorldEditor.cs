using System;
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
            cursorModel = _content.Load<Model>(Content.MODEL_CURSOR);
        }

        double f, b, l, r, u, d;
        const double movementSpeed = 200;
        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            if (!_flags.HasFlag(GameFlags.EditorMode))
                return null;

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

                if (_inputArgs.Events.HasFlag(InputEventList.MouseLeftClick) && _state.World.Blocks[x, y, z] == WorldBlock.None)
                    _state.World.Blocks[x, y, z] = WorldBlock.Wall;

                else if (_inputArgs.Events.HasFlag(InputEventList.MouseRightClick) && _state.World.Blocks[x, y, z] == WorldBlock.Lever)
                { //remove lever, Lever duerfen nicht von anderen Blöcken uebersschrieben werden              
                    _state.World.RemoveObject(new Vector3Int(x, y, z));
                    _state.World.Blocks[x, y, z] = WorldBlock.None;
                }
                else if (_inputArgs.Events.HasFlag(InputEventList.MouseRightClick) && _state.World.Blocks[x, y, z] == WorldBlock.Spikes)
                { //remove spike, Spike duerfen nicht von anderen Blöcken uebersschrieben werden              
                    _state.World.RemoveObject(new Vector3Int(x, y, z));
                    _state.World.Blocks[x, y, z] = WorldBlock.None;
                }
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

                else if (_inputArgs.Events.HasFlag(InputEventList.PlaceLever) && _state.World.Blocks[x, y, z] != WorldBlock.Lever)//Schalter ertmal auf Taste 5 belegen
                {
                    Lever lever = new Lever(content, new Vector3Int(x, y, z));
                    _state.World.AddObject(lever);
                    //_state.World.Blocks[x, y, z] = WorldBlock.Lever;
                }

                else if (_state.World.Blocks[x, y, z] == WorldBlock.Lever && _inputArgs.Events.HasFlag(InputEventList.MouseLeftClick))
                    (_state.World.BlockAt(x, y, z) as Lever).Rotation += Math.PI / 2;

                else if (_inputArgs.Events.HasFlag(InputEventList.PlaceSpike) && _state.World.Blocks[x, y, z] != WorldBlock.Spikes)//Schalter ertmal auf Taste 5 belegen
                {
                    Spike spike = new Spike(content, new Vector3Int(x, y, z));
                    _state.World.AddObject(spike);
                }

            };
        }

        public void Initialize(GraphicsDevice _graphicsDevice)
        {
            graphicsDevice = _graphicsDevice;
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _projectionMatrix)
        {
            if (!_flags.HasFlag(GameFlags.EditorMode))
                return;

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
