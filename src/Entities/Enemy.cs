﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace ReiseZumGrundDesSees
{
    class Enemy : IEnemy
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
        List<SoundEffect> soundEffects;
        public static List<Enemy> EnemyList = new List<Enemy>();
        public Vector3 Position { get; set; }

        private bool wasAddedToCollisionManager = false;
        private bool disposed = false;

        public bool HasMultipleHitboxes => false;
        public Hitbox Hitbox { get; private set; }
        public Hitbox[] Hitboxes => throw new NotImplementedException();
        public bool IsEnabled => true;

        public enum Art
        {
            Moving,
            Climbing,
            Jumping,
            Shooting,
            MandS
        }

        public Enemy(Vector3 _position, Art _typ)
        {

         
            Position = _position;        
            Gegnerart = _typ;      
            Hitbox = new Hitbox(Position.X, Position.Y, Position.Z, 1f - 0.5f, 0.9f, 1f - 0.5f,
                (_block) => true,
                (_obj) => !(_obj is Geschoss));
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

        public Enemy(ConfigFile.ConfigNode _config)
            : this((_config.Items["EnemyPosition"]).ToVector3(), (Art)Enum.Parse(typeof(Art), _config.Items["EnemyTyp"]))
        { }

        public ConfigFile.ConfigNode GetState()
        {
            ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();

            _node.Items["EnemyPosition"] = Position.ToNiceString();
            _node.Items["EnemyTyp"] = Gegnerart.ToString();
          
            return _node;
        }


        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {


            int Aggrorange = 15;
            Hitbox = new Hitbox(Position.X, Position.Y, Position.Z, 1f - 0.5f, 1f, 1f - 0.5f,
                (_block) => true,
                (_obj) => !(_obj is Geschoss));

            Vector3 _movement = new Vector3(0, 0, 0);

            Vector3 EnemytoPlayer = Vector3.Subtract(new Vector3(_view.PlayerX, _view.PlayerY+0.5f, _view.PlayerZ), Position);
            EnemytoPlayer.Normalize();
            Rotate = Math.Acos(Vector3.Dot(new Vector3(0, 0, -1), EnemytoPlayer)); //Rotation in Rad
            if (EnemytoPlayer.X > 0) Rotate *= -1;

            if (Gegnerart == Art.Moving || Gegnerart == Art.Climbing || Gegnerart == Art.Jumping || Gegnerart == Art.MandS)
            {
                _movement.Y -= 0.005f * (float)_passedTime;
                if (Vector3.Distance(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), Position) <= Aggrorange)
                {
                    IdleTimer = 0;

                    if (HitPlayer == false)
                    {
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
                    if (IdleTimer == 1)
                    {
                        Random = new Vector2((float)rnd.NextDouble() * 2 - 1f, (float)rnd.NextDouble() * 2 - 1f);
                        Random.Normalize();

                    }
                    if (Vector3.Distance(new Vector3(Position.X + Random.X * (float)(_passedTime * 0.0025f), Position.Y, Position.Z + Random.Y * (float)(_passedTime * 0.0025f)), SpawnPosition) < 5f)
                    {
                        _movement.X += Random.X * (float)(_passedTime * 0.00125f);
                        _movement.Z += Random.Y * (float)(_passedTime * 0.00125f);
                    }
                    //Blickrichtung

                    Rotate = Math.Acos(Vector3.Dot(new Vector3(0, 0, -1), new Vector3(Random.X, 0, Random.Y))); //Rotation in Rad
                    if (Random.X > 0) Rotate *= -1;
                    IdleTimer += _passedTime;
                }
                if (HitTimer > 1000) HitPlayer = false;
            }

            var _collInfo = _view.CollisionDetector.CheckCollision(ref _movement, this);

            if (Gegnerart == Art.Climbing) //Klettere über Bloecke
            {
                if ((_collInfo.ContainsKey(Direction.Front) ||
                     _collInfo.ContainsKey(Direction.Back) ||
                     _collInfo.ContainsKey(Direction.Right) ||
                     _collInfo.ContainsKey(Direction.Left)))
                {
                    _movement.Y += (float)(_passedTime * 0.01f);
                }
            }

            if (Gegnerart == Art.Jumping)//Springe
            {
                if (Jumptimer == 0 && (_collInfo.ContainsKey(Direction.Front) ||
                                       _collInfo.ContainsKey(Direction.Back) ||
                                       _collInfo.ContainsKey(Direction.Right) ||
                                       _collInfo.ContainsKey(Direction.Left)))
                {
                    speedY += 0.9f;
                }
                Jumptimer += _passedTime;

                speedY -= 0.005f * (float)_passedTime;
                if (speedY < 0) speedY = 0;
                if (_collInfo.ContainsKey(Direction.Bottom)) Jumptimer = 0;
                _movement.Y += speedY * (float)_passedTime * 0.01f;
            }

            if (Gegnerart == Art.Shooting || Gegnerart == Art.MandS)//Shoot
            {
                if (Geschosstimer > 1000) Geschosstimer = 0;//1 Schuss pro Sekunde
                if (Geschosstimer == 0 && Vector3.Distance(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), Position) <= Aggrorange
                && Vector3.Distance(new Vector3(_view.PlayerX, _view.PlayerY +0.25f, _view.PlayerZ), Position) > 2f)//Schieße nicht in Nahkampfreichweite
                {
                    //nicht schießen, wenn Block dazwischen in 2 Reichweite
                    bool dazwischen = false;
                    int dis =(int) Vector3.Distance(new Vector3(_view.PlayerX, _view.PlayerY + 0.25f, _view.PlayerZ), Position);
                    for (int i = 0; i < dis; i++) { 
                    if (_view.BlockWorld[(int)(Position.X+i*EnemytoPlayer.X), (int)(Position.Y + 0.25f+i*EnemytoPlayer.Y), (int)(Position.Z+i*EnemytoPlayer.Z)] == WorldBlock.Wall)                       
                    {
                        dazwischen = true;
                    }
                    }
                    if (dazwischen == false)
                    {
                        new Geschoss(ContentManager, new Vector3(Position.X, Position.Y + 0.25f, Position.Z), EnemytoPlayer);
                        soundEffects[0].Play();
                    }
                }
                Geschosstimer += _passedTime;
            }



            return (ref GameState _state) =>
            {
                this.Position += _movement;

                foreach (var _item in _collInfo)
                    if (_item.Value.CollisionType == CollisionDetector.CollisionSource.Type.WithObject &&
                        _item.Value.Object is IPlayer p &&
                        !HitPlayer)
                    {
                        p.Hit();
                        HitPlayer = true;
                    }

                if (disposed)
                {
                    if (wasAddedToCollisionManager)
                        _state.CollisionDetector.RemoveObject(this);
                    EnemyList.Remove(this);
                }
                else if (!wasAddedToCollisionManager)
                {
                    _state.CollisionDetector.AddObject(this);
                    wasAddedToCollisionManager = true;
                }
            };
        }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            soundEffects = new List<SoundEffect>();
            soundEffects.Add(ContentManager.Load<SoundEffect>(Content.SOUND_SHOOT)); //schiesen
            if (Gegnerart == Art.Shooting)
                Model = ContentManager.Load<Model>(Content.MODEL_GEGNER_2);
            else
                Model = ContentManager.Load<Model>(Content.MODEL_GEGNER_1);
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix, GraphicsDevice _grDevice)
        {
            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateRotationY((float)Rotate) * Matrix.CreateTranslation(this.Position);

                    effect.View = _viewMatrix;

                    effect.Projection = _perspectiveMatrix;

                }

                mesh.Draw();
            }


        }

        public void Hit()
        {
            disposed = true;
        }
    }
}
