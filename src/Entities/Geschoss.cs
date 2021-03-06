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
    class Geschoss : IUpdateable, IReadonlyPositionObject, IRenderable, ICollisionObject
    {
        public Vector3 Position { get; private set; }
        public Vector3 SpawnPosition;
        public Vector3 Movement { get; private set; }

        public bool HasMultipleHitboxes => false;
        public Hitbox Hitbox { get; private set; }
        public Hitbox[] Hitboxes => throw new NotImplementedException();
        public bool IsEnabled => true;

        ContentManager ContentManager;
        public Model Model;

        public Geschoss(ContentManager contentManager, Vector3 _position, Vector3 _movement)
        {
            ContentManager = contentManager;
            Model = contentManager.Load<Model>(ContentRessources.MODEL_GESCHOSS);
            Position = _position;
            SpawnPosition = _position;
            Movement = _movement;
            Hitbox = new Hitbox(Position.X, Position.Y, Position.Z, 0.25f - 0.125f, 0.25f, 0.25f - 0.125f, Hitbox.Type.Geschoss);
        }
        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            //throw new NotImplementedException();
        }

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {
            Matrix _worldMatrix = Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Add(this.Position, new Vector3(0, 0.5f, 0)));
            _effect.WorldMatrix = _worldMatrix;
            _effect.VertexFormat = VertexFormat.Position;

            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = _effect.Effect;

                mesh.Draw();
            }

        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            double MaxDistance = 15;
            Hitbox = new Hitbox(Position.X, Position.Y, Position.Z, 0.25f - 0.125f, 0.25f, 0.25f - 0.125f, Hitbox.Type.Geschoss);
            Vector3 _movement = new Vector3(0, 0, 0);
            if (Vector3.Distance(Position, SpawnPosition) > MaxDistance)
                _view.Geschosse.Remove(this);
            else
                _movement += Movement * (float)(_passedTime * 0.006f);

            var _collInfo = _view.CollisionDetector.CheckCollision(ref _movement, this);

            if (_collInfo.Any())
                _view.Geschosse.Remove(this);

            return (ref GameState _state) =>
            {
                this.Position += _movement;

                if (_collInfo.Any())
                    _state.CollisionDetector.RemoveObject(this);

                foreach (var _item in _collInfo)
                {
                    if (_item.Value.CollisionType == CollisionDetector.CollisionSource.Type.WithObject &&
                        _item.Value.Object is IPlayer p)

                        p.Hit();
                }
            };
        }
    }
}
