using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    class MovingBlock : IWorldObject, IMovingObject, IStartStopable
    {
        private Vector3[] positionMarks;
        private int status;
        private float percentage = 0;
        private bool moving = true;
        private bool addedToCollisionManager = false;
        private Model model;

        public Vector3 Position { get; set; }

        bool movedbefore = true;
        public MovingBlock(List<Vector3> _positionMarks)
        {
         
                positionMarks = new Vector3[_positionMarks.Count];
                for (int i = 0; i < _positionMarks.Count; i++)
                    positionMarks[i] = _positionMarks.ElementAt(i);
                Position = positionMarks[0];
        
        }

        public MovingBlock(ConfigFile.ConfigNode _node, ObjectIDMapper _idMapper)
        {
            if (_node.Items["moving"].ToString() == "true")
                moving = true;
            else
            {
                moving = false;
                movedbefore = false;
            }
            List<Vector3> _positionMarks = new List<Vector3>();
            int _count = _node.Nodes.Count;
            for (int i = 0; i < _count; i++)
                _positionMarks.Add(_node.Nodes[i.IdAsString()].Items["position"].ToVector3());

            positionMarks = _positionMarks.ToArray();
            Position = positionMarks[0];
            Hitbox = new Hitbox(Position.X - 0.5f, Position.Y - 0.5f, Position.Z - 0.5f, 1f, 1f, 1f);
        }

        public bool HasMultipleHitboxes => false;
        public Hitbox Hitbox { get; private set; }
        public Hitbox[] Hitboxes => throw new NotImplementedException();
        public bool IsEnabled => true;

        public Vector3 Velocity { get; private set; }

        public void Start()
        {
            moving = true;
            if (movedbefore == false)
            {
                movedbefore = true;
                status++;
                status = status % positionMarks.Length;
            }
        }

        public void Stop()
        {
            moving = false;
        }

        Texture2D texture;
        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            model = _contentManager.Load<Model>(ContentRessources.MODEL_BLOCK);
            texture = _contentManager.Load<Texture2D>(ContentRessources.TEXTURE);
        }

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {
            Matrix _worldMatrix = Matrix.CreateTranslation(Position);
            _effect.WorldMatrix = _worldMatrix;
            _effect.VertexFormat = VertexFormat.PositionTexture;
            _effect.Texture = texture;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    DebugHelper.Information.RenderedOtherVertices += (uint)part.NumVertices;
                    part.Effect = _effect.Effect;
                }

                mesh.Draw();
            }
        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            if (!_flags.HasFlag(GameFlags.GameRunning) || !_flags.HasFlag(GameFlags.GameLoaded))
                return null;
            
            Vector3 _lastPosition = positionMarks[(status + positionMarks.Length - 1) % positionMarks.Length];
            Vector3 _nextPosition = positionMarks[status];

            Vector3 _movement = new Vector3(0, 0, 0);
            CollisionDetector.CollisionInfo _collisionInfo = null;
            CollisionDetector.CollisionInfo _oldCollisionInfo = null;
            if (moving && Vector3.Distance(this.Position,_view.Player.Position) < 50)
            {
                float _newPercentage = percentage + (float)_passedTime * 0.0005f;

                Vector3 _newPosition = (_nextPosition - _lastPosition) * _newPercentage + _lastPosition;
                _movement = _newPosition - Position;

                Vector3 _testMovement = _movement;
                _collisionInfo = _view.CollisionDetector.CheckCollision(ref _testMovement, this);

                foreach (var _item in _collisionInfo)
                    if (_item.Value.Object is IMoveable _moveable)
                        _moveable.Move(_movement - _testMovement, _view.CollisionDetector);

                _oldCollisionInfo = _collisionInfo;
                if (_movement.Length() < 10)
                    _collisionInfo = _view.CollisionDetector.CheckCollision(ref _movement, this);

                if (!_collisionInfo.Any())
                    percentage = _newPercentage;
                else
                {
                    foreach (var _item in _oldCollisionInfo)
                        if (_item.Value.Object is IMoveable _moveable)
                            _moveable.Move(-_movement + _testMovement, _view.CollisionDetector);
                }
            }


            if (percentage >= 1)
            {
                status++;
                status = status % positionMarks.Length;
                percentage = 0;
            }

            return (ref GameState _state) =>
            {
                if (!addedToCollisionManager)
                {
                    _state.CollisionDetector.AddObject(this);
                    addedToCollisionManager = true;
                }

                if (moving && _collisionInfo != null && !_collisionInfo.Any())
                {
                    Position += _movement;
                    Velocity = _movement;
                }
                else Velocity = Vector3.Zero;

                Hitbox = new Hitbox(Position.X - 0.5f, Position.Y - 0.5f, Position.Z - 0.5f, 1f, 1f, 1f);
            };
        }

        public ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper)
        {
            ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();

            _node.Items["moving"] = moving.ToString().ToLower();

            int _pointCount = 0;
            foreach (Vector3 _point in positionMarks)
            {
                ConfigFile.ConfigNode _curNode = new ConfigFile.ConfigNode();

                _curNode.Items["position"] = _point.ToNiceString();

                _node.Nodes[_pointCount.IdAsString()]
                    = _curNode;

                _pointCount++;
            }

            return _node;
        }
    }
}
