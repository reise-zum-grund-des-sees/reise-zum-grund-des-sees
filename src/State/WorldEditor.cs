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
    class WorldEditor : IUpdateable, IReadonlyPositionObject, IRenderable
    {
        public Vector3 Position { get; set; }
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private Model cursorModel;

        private bool recording = false, replaying = false;
        private List<Actions> recordingBuffer = new List<Actions>();
        private int replayingIndex = 0;

        private List<Vector3> MovingBlockPosition = new List<Vector3>();
        public WorldEditor(Vector3 _position, ContentManager _content)
        {
            Position = _position;
            content = _content;
            cursorModel = _content.Load<Model>(Content.MODEL_CURSOR);
        }

        [Flags]
        private enum Actions
        {
            None = 0,
            MoveLeft = 1,
            MoveRight = 1 << 1,
            MoveUp = 1 << 2,
            MoveDown = 1 << 3,
            MoveForwards = 1 << 4,
            MoveBack = 1 << 5,

            PutWall = 1 << 6,
            PutWater1 = 1 << 7,
            PutWater2 = 1 << 8,
            PutWater3 = 1 << 9,
            PutWater4 = 1 << 10,
            PutWater4Infinite = 1 << 11,
            PutLever = 1 << 12,
            PutSpikes = 1 << 13,
            PutPressurePlate = 1 << 14,
            RemoveBlock = 1 << 15,

            Rotate = 1 << 16,
            PutMovingBlock = 1 << 17,
            PutMovingBlockEnd = 1 << 18
        }

        bool recordingReleased = true, replayingReleased = true;
        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            if (!_flags.HasFlag(GameFlags.EditorMode))
                return null;

            Actions _actions = GetActions(_inputArgs.Events, _view, _passedTime);

            f += _passedTime;
            b += _passedTime;
            l += _passedTime;
            r += _passedTime;
            u += _passedTime;
            d += _passedTime;

            return (ref GameState _state) =>
            {
                if (replaying)
                {
                    if (replayingIndex >= recordingBuffer.Count)
                        replaying = false;
                    else
                        _actions = recordingBuffer[replayingIndex++];
                }

                ApplyActions(_actions, _state);

                if (!replaying && !recording)
                {
                    if (!_inputArgs.Events.HasFlag(InputEventList.Record))
                        recordingReleased = true;
                    else if (recordingReleased)
                    {
                        recording = true;
                        recordingBuffer.Clear();
                        recordingReleased = false;
                    }

                    if (!_inputArgs.Events.HasFlag(InputEventList.Replay))
                        replayingReleased = true;
                    else if (replayingReleased)
                    {
                        replaying = true;
                        replayingIndex = 0;
                        replayingReleased = false;
                    }
                }
                else if (recording)
                {
                    if (!_inputArgs.Events.HasFlag(InputEventList.Record))
                        recordingReleased = true;
                    else if (recordingReleased)
                    {
                        recording = false;
                        recordingReleased = false;
                        string s = "";
                        foreach (var i in recordingBuffer) s += i.ToString() + " | ";
                        DebugHelper.Log(s);
                    }

                    if (!recordingBuffer.Any() ||
                        (_actions &
                            (Actions.MoveLeft |
                             Actions.MoveRight |
                             Actions.MoveUp |
                             Actions.MoveDown |
                             Actions.MoveForwards |
                             Actions.MoveBack)) != Actions.None ||
                        _actions != recordingBuffer[recordingBuffer.Count - 1])
                    {
                        recordingBuffer.Add(_actions);
                    }
                }

            };
        }

        double f, b, l, r, u, d;
        bool rotateReleased = false;
        const double movementSpeed = 200;
        private Actions GetActions(InputEventList _inputEvents, GameState.View _stateView, double _passedTime)
        {
            Actions _actions = Actions.None;

            Vector3 _difference = Vector3.Zero;

            if (_inputEvents.HasFlag(InputEventList.Rotate))
            {
                if (rotateReleased)
                {
                    _actions |= Actions.Rotate;
                    rotateReleased = false;
                }
            }
            else rotateReleased = true;

            if (_inputEvents.HasFlag(InputEventList.MoveForwards) & (b > movementSpeed | replaying))
            {
                _actions |= Actions.MoveForwards;
                b = 0;
            }

            if (_inputEvents.HasFlag(InputEventList.MoveBackwards) & (f > movementSpeed | replaying))
            {
                _actions |= Actions.MoveBack;
                f = 0;
            }

            if (_inputEvents.HasFlag(InputEventList.MoveLeft) & (r > movementSpeed | replaying))
            {
                _actions |= Actions.MoveLeft;
                r = 0;
            }

            if (_inputEvents.HasFlag(InputEventList.MoveRight) & (l > movementSpeed | replaying))
            {
                _actions |= Actions.MoveRight;
                l = 0;
            }

            if (_inputEvents.HasFlag(InputEventList.MoveUp) & (u > movementSpeed | replaying))
            {
                _actions |= Actions.MoveUp;
                u = 0;
            }

            if (_inputEvents.HasFlag(InputEventList.MoveDown) & (d > movementSpeed | replaying))
            {
                _actions |= Actions.MoveDown;
                d = 0;
            }

            int x = (int)Position.X;
            int y = (int)Position.Y;
            int z = (int)Position.Z;

            if (_inputEvents.HasFlag(InputEventList.MouseLeft))
                _actions |= Actions.PutWall;

            else if (_inputEvents.HasFlag(InputEventList.MouseRight))
                _actions |= Actions.RemoveBlock;

            else if (_inputEvents.HasFlag(InputEventList.PlaceWater1))
                _actions |= Actions.PutWater1;

            else if (_inputEvents.HasFlag(InputEventList.PlaceWater2))
                _actions |= Actions.PutWater2;

            else if (_inputEvents.HasFlag(InputEventList.PlaceWater3))
                _actions |= Actions.PutWater3;

            else if (_inputEvents.HasFlag(InputEventList.PlaceWater4))
                _actions |= Actions.PutWater4;

            else if (_inputEvents.HasFlag(InputEventList.PlaceWater4Infinite))
                _actions |= Actions.PutWater4Infinite;

            else if (_inputEvents.HasFlag(InputEventList.PlaceLever))
                _actions |= Actions.PutLever;

            else if (_inputEvents.HasFlag(InputEventList.PlaceSpike))
                _actions |= Actions.PutSpikes;

            else if (_inputEvents.HasFlag(InputEventList.PlacePressurePlate))
                _actions |= Actions.PutPressurePlate;

            else if (_inputEvents.HasFlag(InputEventList.PlaceMovingBlock))
               _actions |= Actions.PutMovingBlock;

            else if (_inputEvents.HasFlag(InputEventList.PlaceMovingBlockEnd))
                _actions |= Actions.PutMovingBlockEnd;
          
            return _actions;
        }

        private void ApplyActions(Actions _actions, GameState _state)
        {
            Vector3 _difference = Vector3.Zero;
            if (_actions.HasFlag(Actions.MoveLeft))
                _difference += new Vector3(-1, 0, 0);
            if (_actions.HasFlag(Actions.MoveRight))
                _difference += new Vector3(1, 0, 0);
            if (_actions.HasFlag(Actions.MoveForwards))
                _difference += new Vector3(0, 0, -1);
            if (_actions.HasFlag(Actions.MoveBack))
                _difference += new Vector3(0, 0, 1);
            if (_actions.HasFlag(Actions.MoveUp))
                _difference += new Vector3(0, 1, 0);
            if (_actions.HasFlag(Actions.MoveDown))
                _difference += new Vector3(0, -1, 0);

            float _direction = (float)Math.Round(-_state.Camera.Angle * 4 / MathHelper.TwoPi) * MathHelper.TwoPi / 4f;
            _difference = Vector3.Transform(_difference, Quaternion.CreateFromAxisAngle(Vector3.Up, _direction));

            if (_difference.X >= 1 / Math.Sqrt(2)) Position += Vector3.Right;
            else if (_difference.X <= -1 / Math.Sqrt(2)) Position += Vector3.Left;

            if (_difference.Y == 1) Position += Vector3.Up;
            else if (_difference.Y == -1) Position += Vector3.Down;

            if (_difference.Z >= 1 / Math.Sqrt(2)) Position += Vector3.Backward;
            else if (_difference.Z <= -1 / Math.Sqrt(2)) Position += Vector3.Forward;

            int x = (int)Position.X;
            int y = (int)Position.Y;
            int z = (int)Position.Z;
          
            if (_actions.HasFlag(Actions.PutWall))
                _state.World.Blocks[x, y, z] = WorldBlock.Wall;

            else if (_actions.HasFlag(Actions.RemoveBlock))
                _state.World.Blocks[x, y, z] = WorldBlock.None;

            else if (_actions.HasFlag(Actions.PutWater1))
                _state.World.Blocks[x, y, z] = WorldBlock.Water1;

            else if (_actions.HasFlag(Actions.PutWater2))
                _state.World.Blocks[x, y, z] = WorldBlock.Water2;

            else if (_actions.HasFlag(Actions.PutWater3))
                _state.World.Blocks[x, y, z] = WorldBlock.Water3;

            else if (_actions.HasFlag(Actions.PutWater4))
                _state.World.Blocks[x, y, z] = WorldBlock.Water4;

            else if (_actions.HasFlag(Actions.PutWater4Infinite))
                _state.World.Blocks[x, y, z] = WorldBlock.Water4Infinite;

            else if (_actions.HasFlag(Actions.PutLever))
                _state.World.Blocks[x, y, z] = WorldBlock.Lever;

            else if (_actions.HasFlag(Actions.Rotate) &&
                     _state.World.Blocks[x, y, z].IsSpecialBlock() &&
                     _state.World.BlockAt(x, y, z) is IRotateable _rot)
                _rot.Rotate(MathHelper.PiOver2);

            else if (_actions.HasFlag(Actions.PutSpikes))
                _state.World.Blocks[x, y, z] = WorldBlock.Spikes;

            else if (_actions.HasFlag(Actions.PutPressurePlate))
                _state.World.Blocks[x, y, z] = WorldBlock.PressurePlateUp;

         
            else if (_actions.HasFlag(Actions.PutMovingBlock))
            {
                if(MovingBlockPosition.Count==0)
                    MovingBlockPosition.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                else if(MovingBlockPosition.ElementAt(MovingBlockPosition.Count-1)!= new Vector3(x + 0.5f, y + 0.5f, z + 0.5f))
                MovingBlockPosition.Add(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
              
            }
            else if (_actions.HasFlag(Actions.PutMovingBlockEnd))
            {
                new MovingBlock(MovingBlockPosition);
                MovingBlock.MovingBlockList[MovingBlock.MovingBlockList.Count - 1].Initialize(graphicsDevice,content);
                MovingBlockPosition.Clear();
     
            }
        }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            graphicsDevice = _graphicsDevice;
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _projectionMatrix, GraphicsDevice _grDevice)
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
