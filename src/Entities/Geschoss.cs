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
    class Geschoss : IUpdateable, IPositionObject, IRenderable
    {
        public Vector3 Position { get; private set; }
        public Vector3 SpawnPosition;
        public Vector3 Movement { get; private set; }
        ContentManager ContentManager;
        public Model Model;
        public Hitbox Hitbox;
        public static List<Geschoss> GeschossList = new List<Geschoss>();

        public Geschoss(ContentManager contentManager, Vector3 _position, Vector3 _movement)
        {
            ContentManager = contentManager;
            Model = contentManager.Load<Model>("Block");
            Position = _position;
            SpawnPosition = _position;
            Movement = _movement;
            Hitbox = new Hitbox(Position, 0.25f - 0.125f, 0.25f, 0.25f - 0.125f);
            GeschossList.Add(this);
        }
        public void Initialize(GraphicsDevice _graphicsDevice)
        {
            //throw new NotImplementedException();
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix)
        {

            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Add(this.Position, new Vector3(0, 0.5f, 0)));

                    effect.View = _viewMatrix;

                    effect.Projection = _perspectiveMatrix;

                }

                mesh.Draw();
            }

        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            double MaxDistance = 15;
            Hitbox = new Hitbox(Position, 0.5f - 0.125f, 0.5f, 0.5f - 0.125f);
            Vector3 _movement = new Vector3(0, 0, 0);
            if (Vector3.Distance(Position, SpawnPosition) > MaxDistance)
                GeschossList.Remove(this);
            else
                _movement += Movement * (float)(_passedTime * 0.006f);

            //Direction _info = CollisionDetector.CollisionWithWorld(ref _movement, Hitbox, _view.BlockWorld);
            //if (_info.HasFlag(Direction.Front) || _info.HasFlag(Direction.Back) || _info.HasFlag(Direction.Right) || _info.HasFlag(Direction.Left) || _info.HasFlag(Direction.Top) || _info.HasFlag(Direction.Bottom))
            //    GeschossList.Remove(this);
            //List<Direction> _info2 = new List<Direction>();
            //for (int i = 0; i < _view.Player.Blocks.Count; i++)
            //{
            //    _info2.Add(CollisionDetector.CollisionDetectionWithSplittedMovement(ref _movement, Hitbox, _view.Player.Blocks[i].Hitbox));
            //    if (_info2[i].HasFlag(Direction.Front) || _info2[i].HasFlag(Direction.Back) || _info2[i].HasFlag(Direction.Right) || _info2[i].HasFlag(Direction.Left) || _info2[i].HasFlag(Direction.Top) || _info2[i].HasFlag(Direction.Bottom))
            //    {
            //        GeschossList.Remove(this);
            //        break;
            //    }
            //}

            return (ref GameState _state) =>
            {
                this.Position += _movement;

            };
        }
    }
}
