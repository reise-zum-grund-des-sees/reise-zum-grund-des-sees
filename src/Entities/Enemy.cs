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
        public static List<Enemy> EnemyList = new List<Enemy>();
        public bool HasMultipleHitboxes{ get; }

        public Hitbox Hitbox => new Hitbox(Position, 1f, 1f, 1f);

        public Hitbox[] Hitboxes => throw new NotImplementedException();

        public Vector3 Position { get; set; }
       
        public enum Art
        {
            Moving,
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
            EnemyList.Add(this);
        }
      

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            int Aggrorange = 15;
            Vector3 _movement = new Vector3(0, 0, 0);
     
            if(Vector3.Distance(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ),Position) < Aggrorange)
            {
                Vector3 EnemytoPlayer = Vector3.Subtract(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), Position);
                EnemytoPlayer.Normalize();
                _movement.X += EnemytoPlayer.X * (float)(_passedTime * 0.005f);
                _movement.Z += EnemytoPlayer.Z * (float)(_passedTime * 0.005f);
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
                            effect.EnableDefaultLighting();
                            effect.World = Matrix.CreateScale(0.05f) * Matrix.CreateTranslation(Vector3.Add(this.Position, new Vector3(0, 0.5f, 0)));

                            effect.View = _viewMatrix;

                            effect.Projection = _perspectiveMatrix;

                        }

                        mesh.Draw();
                    }
                
           
        }
    }
}
