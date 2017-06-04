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
        public double Rotate;
        public Vector3 SpawnPosition;//für Idlemovement, damit Gegner nicht wegrennt
        public double IdleTimer;
        Vector2 Random;
        double Jumptimer;
        double Geschosstimer;
        float speedY;
        public static List<Enemy> EnemyList = new List<Enemy>();
        public Vector3 Position { get; set; }
        public Hitbox Hitbox;
        public enum Art
        {
            Moving,
            Climbing,
            Jumping,
            Shooting,
            MandS
        }

        public Enemy(ContentManager contentManager, Vector3 _position, Art _typ)
        {
            
            ContentManager = contentManager;
            Model = contentManager.Load<Model>("Gegner1");      
            Position = _position;
            Gegnerart= _typ;
            Hitbox = new Hitbox(Position, 1f - 0.5f, 1f, 1f - 0.5f);
            HitPlayer = false;
            HitTimer = 0;
            IdleTimer = 0;
            Geschosstimer = 0;
            Rotate = 0;
            SpawnPosition = _position;
            Random = new Vector2();
            Jumptimer = 1;
            speedY = 0;
            EnemyList.Add(this);
        }


        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
       
            
            int Aggrorange = 15;
            Hitbox = new Hitbox(Position, 1f - 0.5f, 1f, 1f - 0.5f);//wenn Mase der Gegner (1,1,1)

            Vector3 _movement = new Vector3(0, 0, 0);

            Vector3 EnemytoPlayer = Vector3.Subtract(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), Position);
            EnemytoPlayer.Normalize();
            Rotate = Math.Acos(Vector3.Dot(new Vector3(0, 0, -1), EnemytoPlayer)); //Rotation in Rad
            if (EnemytoPlayer.X > 0) Rotate *= -1;

            if (Gegnerart == Art.Moving || Gegnerart == Art.Climbing || Gegnerart == Art.Jumping || Gegnerart == Art.MandS)
            {
                _movement.Y -= 0.005f * (float)_passedTime;
                if (Vector3.Distance(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), Position) <= Aggrorange )
            {
                    IdleTimer = 0;
               
                    if (HitPlayer == false) {
                        HitTimer = 0;
                      
                _movement.X += EnemytoPlayer.X * (float)(_passedTime * 0.0025f);
                _movement.Z += EnemytoPlayer.Z * (float)(_passedTime * 0.0025f);
                    }
                    if (HitPlayer == true)
                    {
                        HitTimer += _passedTime;
                        Rotate *= -1;
                        _movement.X -= EnemytoPlayer.X * (float)(_passedTime * 0.0025f);
                        _movement.Z -= EnemytoPlayer.Z * (float)(_passedTime * 0.0025f);
                    }
                }
                else //idlemovement
                {
                   
                    HitPlayer = false;
                    if (IdleTimer == 0) SpawnPosition = Position;
                    if (IdleTimer >= 2000) IdleTimer = 1;
                    Random rnd = new Random();            
                    if (IdleTimer== 1)
                    {                    
                        Random = new Vector2((float)rnd.NextDouble()*2-1f, (float)rnd.NextDouble() * 2 - 1f);
                        Random.Normalize();
                       
                    }
                    if (Vector3.Distance(new Vector3(Position.X + Random.X * (float)(_passedTime * 0.0025f), Position.Y,Position.Z + Random.Y * (float)(_passedTime * 0.0025f)),SpawnPosition)<5f) {
                        _movement.X += Random.X * (float)(_passedTime * 0.00125f);
                        _movement.Z += Random.Y * (float)(_passedTime * 0.00125f);
                    }
                    //Blickrichtung

                    Rotate = Math.Acos(Vector3.Dot(new Vector3(0, 0, -1), new Vector3(Random.X,0,Random.Y))); //Rotation in Rad
                    if (Random.X > 0) Rotate *= -1;
                    IdleTimer += _passedTime;
                }
                if (HitTimer > 1000) HitPlayer = false;
            }
            Direction _info = CollisionDetector.CollisionWithWorld(ref _movement, Hitbox, _view.BlockWorld);
               
            List<Direction> _info2 = new List<Direction>();
            for(int i=0;i<_view.PlayerBlocks.Count;i++)
            _info2.Add(CollisionDetector.CollisionDetectionWithSplittedMovement(ref _movement, Hitbox, _view.PlayerBlocks[i].Hitbox));
                if (Gegnerart == Art.Climbing) //Klettere über Bloecke
                {
                    if ((_info.HasFlag(Direction.Front) || _info.HasFlag(Direction.Back) || _info.HasFlag(Direction.Right) || _info.HasFlag(Direction.Left)))
                    {
                        _movement.Y += (float)(_passedTime * 0.01f);
                    }
                    else
                    {
                        for (int i = 0; i < _info2.Count; i++)
                        {
                            if ((_info2[i].HasFlag(Direction.Front) || _info2[i].HasFlag(Direction.Back) || _info2[i].HasFlag(Direction.Right) || _info2[i].HasFlag(Direction.Left)))
                            {
                                _movement.Y += (float)(_passedTime * 0.01f);
                                break;
                            }
                        }
                    }

                }
                if (Gegnerart == Art.Jumping)//Springe
                {
                  
              if (Jumptimer==0 && (_info.HasFlag(Direction.Front) || _info.HasFlag(Direction.Back) || _info.HasFlag(Direction.Right) || _info.HasFlag(Direction.Left)))
                    {
                        speedY += 0.9f;
                    }
                    else
                    {
                        for (int i = 0; i < _info2.Count; i++)
                        {
                            if (Jumptimer == 0 && (_info2[i].HasFlag(Direction.Front) || _info2[i].HasFlag(Direction.Back) || _info2[i].HasFlag(Direction.Right) || _info2[i].HasFlag(Direction.Left)))
                            {
                                speedY += 0.9f;
                                break;
                            }
                        }
                    }
                    Jumptimer += _passedTime;
                    
                    speedY -= 0.005f * (float)_passedTime;
                    if (speedY < 0) speedY = 0;
                    if (_info.HasFlag(Direction.Bottom)) Jumptimer = 0;
                    _movement.Y += speedY * (float)_passedTime * 0.01f;
               
                }

                if (Gegnerart == Art.Shooting || Gegnerart == Art.MandS)//Shoot
                {
                if (Geschosstimer > 1000) Geschosstimer = 0;//1 Schuss pro Sekunde
                    if (Geschosstimer== 0 && Vector3.Distance(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), Position) <= Aggrorange
                    && Vector3.Distance(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), Position) > 2f)//Schieße nicht in Nahkampfreichweite
                {    
                       new Geschoss(ContentManager, Position, EnemytoPlayer);    
                }                  
                    Geschosstimer += _passedTime;
             
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
                            effect.World = Matrix.CreateScale(0.045f) *Matrix.CreateRotationY((float)Rotate)* Matrix.CreateTranslation(Vector3.Add(this.Position, new Vector3(0, 0.5f, 0)));

                            effect.View = _viewMatrix;

                            effect.Projection = _perspectiveMatrix;

                        }

                        mesh.Draw();
                    }
                
           
        }
    }
}
