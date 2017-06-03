using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees.Entities
{
    class Enemy : IUpdateable, IPositionObject, IRenderable
    {
        ContentManager ContentManager;
        public Model Model;
        public Art Gegnerart;
        public bool HitPlayer; //Wegrennen vom Spieler wenn getroffen
        public double HitTimer; // für 1 Sekunde
        public static List<Enemy> EnemyList = new List<Enemy>();
        public bool HasMultipleHitboxes{ get; }

        public Hitbox Hitbox => new Hitbox(Position, 1f - 0.5f, 1f, 1f - 0.5f);

        public Hitbox[] Hitboxes => throw new NotImplementedException();

        public Vector3 Position { get; set; }
       
        public enum Art
        {
            Moving,
            Climbing,
            Shooting,
            MandS
        }

        public Enemy(ContentManager contentManager, Vector3 _position, Art _typ)
        {
           
            ContentManager = contentManager;
            Model = contentManager.Load<Model>("Gegner1");
            HasMultipleHitboxes = false;
            Position = _position;
            Gegnerart= _typ;
            HitPlayer = false;
            HitTimer = 0;
            EnemyList.Add(this);
        }


        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            int Aggrorange = 15;
            Vector3 _movement = new Vector3(0, 0, 0);
            _movement.Y-= 0.005f * (float)_passedTime;
            if (Gegnerart == Art.Moving || Gegnerart == Art.Climbing)
            { 
            if (Vector3.Distance(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), Position) <= Aggrorange )
            {
                Vector3 EnemytoPlayer = Vector3.Subtract(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), Position);
                EnemytoPlayer.Normalize();
                    if(HitPlayer == false) {
                        HitTimer = 0;
                _movement.X += EnemytoPlayer.X * (float)(_passedTime * 0.005f);
                _movement.Z += EnemytoPlayer.Z * (float)(_passedTime * 0.005f);
                    }
                    if (HitPlayer == true)
                    {
                        HitTimer += _passedTime;
                        _movement.X -= EnemytoPlayer.X * (float)(_passedTime * 0.005f);
                        _movement.Z -= EnemytoPlayer.Z * (float)(_passedTime * 0.005f);
                    }
                }
                if (HitTimer > 1000) HitPlayer = false;
            Direction _info = CollisionDetector.CollisionWithWorld(ref _movement, Hitbox, _view.BlockWorld);
            List<Direction> _info2 = new List<Direction>();
            for(int i=0;i<_view.PlayerBlocks.Count;i++)
            _info2.Add(CollisionDetector.CollisionDetectionWithSplittedMovement(ref _movement, Hitbox, _view.PlayerBlocks[i].Hitbox));
                if (Gegnerart == Art.Climbing) //Spring über Bloecke
                {
                    if ((_info.HasFlag(Direction.Front) || _info.HasFlag(Direction.Back) || _info.HasFlag(Direction.Right) || _info.HasFlag(Direction.Left)) )
                    {
                        _movement.Y += (float)(_passedTime * 0.01f);
                    }
                }
            }


            return (ref GameState _state) =>
            {
                this.Position += _movement;
             
            };
        }

        public void Initialize(GraphicsDevice _graphicsDevice)
        {
           // throw new NotImplementedException();
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix)
        {
          
            
                    foreach (ModelMesh mesh in this.Model.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            //effect.EnableDefaultLighting();
                            effect.World = Matrix.CreateScale(0.045f) * Matrix.CreateTranslation(Vector3.Add(this.Position, new Vector3(0, 0.5f, 0)));

                            effect.View = _viewMatrix;

                            effect.Projection = _perspectiveMatrix;

                        }

                        mesh.Draw();
                    }
                
           
        }
    }
}
