using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    class PlayerBlock : IPlayerBlock
    {
        public State CurrentState => (State)Zustand;
        public Type BlockType => (Type)Art;

        public float LifetimePercentage { get; private set; }

        public bool HasMultipleHitboxes => ((Type)Art == Type.Medium) ? true : false;
        public Hitbox Hitbox => new Hitbox(Position.X - 0.5f, Position.Y, Position.Z - 0.5f, 1f, 1f, 1f,
            (_block) => !_block.IsWater(),
            (_obj) => true);
        public Hitbox[] Hitboxes => new Hitbox[] {
            new Hitbox(Position.X - 0.5f, Position.Y, Position.Z - 0.5f, 1f, 1f, 1f,
                (_block) => !_block.IsWater(),
                (_obj) => true),
            new Hitbox(Position.X - 0.5f, Position.Y + 0.5f, Position.Z - 0.5f, 1f, 0.5f, 1f)
        };
        public bool IsEnabled => CurrentState == State.Gesetzt;

        public double MaximialDauer;
        public double AktuelleDauer;
        float _speedY;
        Vector3 _movement;
        public int Art;
        public Model Model;
        public Vector3 Position { get; private set; }
        public int Zustand = 0;
        public double Deletetime;
        private bool wasAddedToCollisionManager = false;
        double CD_DISTANCE = 20;
        public double _verbleibenerCD;
        public enum State
        {
            Bereit = 0,
            Gesetzt = 1,
            CD = 2,
            Übergang = 3,
            Delete = 4
        }
        public enum Type
        {
            Light,
            Medium,
            Heavy
        }


        public PlayerBlock(Player _player, int ArtdesBlocks)
        {
            Art = ArtdesBlocks;
            _speedY = 0;
            _movement = new Vector3(0, 0, 0);
            AktuelleDauer = 0;
            MaximialDauer = 15000;
            Position = _player.Position;
            LifetimePercentage = 1f;
            _verbleibenerCD = 5;
            Zustand = (int)State.Bereit;
        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            //Livetime
            /*
            if (AktuelleDauer < MaximialDauer)
                LifetimePercentage = (float)(AktuelleDauer) / (float)(MaximialDauer);
            else
                LifetimePercentage = 1;
                */
            //Löschen aller Blöcke und Setze CD aller Blöcke auf 5 Sekunden
            if (Zustand == (int)State.Delete)
            {
                if (Deletetime == 0) //depress
                {
                    //Kollision mit Pressure Plate
                    Vector3 KolHelp = new Vector3(0, -0.01f, 0);
                    var _collInfo = _view.CollisionDetector.CheckCollision(ref KolHelp, this);
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            for (int z = -1; z < 2; z++)
                            {
                                ISpecialBlock _obj = _view.WorldObjects.BlockAt((int)Position.X + x, (int)(Position.Y) + y, (int)Position.Z + z);
                                if (_collInfo.ContainsKey(Direction.Bottom) &&
                                _collInfo[Direction.Bottom].WorldBlock.IsPressurePlate())
                                {

                                    if (_obj != null && _obj.Type == WorldBlock.PressurePlateDown)
                                    {
                                        (_obj as PressurePlate).depress();
                                    }

                                }
                            }
                        }
                    }
                }
                Deletetime += _passedTime;
                // AktuelleDauer = _view.Player.Blocks.Count * MaximialDauer - 5000 + Deletetime;//!!! Diese Zeile auch ändern, wenn CD verändert wird
                if (Deletetime >= 5000)
                {
                    Deletetime = 0;
                    Zustand = (int)State.Bereit;
                }
            }

            if (Zustand == (int)State.Übergang)
            {
                //Position des Blockes basierend auf Blickrichtung
                Position = new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ);
                Vector3 Blick = Vector3.Transform(new Vector3(0, 0, -1), Matrix.CreateRotationY(_view.Player.Blickrichtung));
                Blick.Normalize();
                Position -= new Vector3(Blick.X * 1.5f, 0, Blick.Z * 1.5f);
                //Console.WriteLine(Position);
                AktuelleDauer = 0;
                Zustand = (int)State.Gesetzt;
            }
            if (Zustand == (int)State.Gesetzt || Zustand == (int)State.CD)
                AktuelleDauer += _passedTime;//Update Timer

            if (MaximialDauer >= AktuelleDauer && Zustand == (int)State.Gesetzt)
            {
                _movement = new Vector3(0, 0, 0);
                //Wenn keine Kolision mit Wand oder Block     

                if (Art == 1)
                {
                    _speedY -= 0.005f * (float)_passedTime;
                    _movement.Y += _speedY * (float)_passedTime * 0.01f;
                }
                if (Art == 2)
                {
                    _speedY -= 0.005f * (float)_passedTime;
                    _movement.Y += _speedY * (float)_passedTime * 0.015f;
                }
                //unter Block ist kein Wasser 


                var _collInfo = _view.CollisionDetector.CheckCollision(ref _movement, this);

                if (_collInfo.ContainsKey(Direction.Bottom))
                    _speedY = 0;

                if (Art != 0)// && _speedY != 0)
                    Position += _movement;


                //Kollision mit Pressure Plate
                Vector3 KolHelp = new Vector3(0, -0.01f, 0);
                _collInfo = _view.CollisionDetector.CheckCollision(ref KolHelp, this);
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            ISpecialBlock _obj = _view.WorldObjects.BlockAt((int)Position.X + x, (int)(Position.Y) + y, (int)Position.Z + z);
                            if (_collInfo.ContainsKey(Direction.Bottom) &&
                            _collInfo[Direction.Bottom].WorldBlock.IsPressurePlate())
                            {
                                if (_obj != null && _obj.Type == WorldBlock.PressurePlateUp)
                                {
                                    (_obj as PressurePlate).press();
                                }
                                if (MaximialDauer < AktuelleDauer + _passedTime && _obj != null && _obj.Type == WorldBlock.PressurePlateDown)
                                {
                                    (_obj as PressurePlate).depress();
                                }

                            }
                        }
                    }
                }

            }

          
                //Objekt ist Tot

                if (Vector3.Distance(Position, new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ)) > CD_DISTANCE)
                {
                    _verbleibenerCD -= _passedTime;
                    if (_verbleibenerCD <= 0)
                    {
                        Zustand = (int)State.Delete;
                        _verbleibenerCD = 5;
                    }

                }
           
            return (ref GameState _state) =>
            {
                if (!wasAddedToCollisionManager)
                {
                    _state.CollisionDetector.AddObject(this);
                    wasAddedToCollisionManager = true;
                }
                //Console.WriteLine(Position);
            };
        }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            if (Art == 0)
            {//leichterBlock
                Model = _contentManager.Load<Model>(Content.MODEL_BLOCK_LEICHT);
            }
            if (Art == 1)//MittelschwererBlock
            {
                Model = _contentManager.Load<Model>(Content.MODEL_BLOCK_MEDIUM);
            }
            if (Art == 2)//SchwererBlock
            {
                Model = _contentManager.Load<Model>(Content.MODEL_BLOCK_SCHWER);
            }
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix, GraphicsDevice _grDevice)
        {
            if (CurrentState == PlayerBlock.State.Gesetzt)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0, 0.5f, 0)));

                        effect.View = _viewMatrix;

                        effect.Projection = _perspectiveMatrix;

                    }

                    mesh.Draw();
                }
            }
        }
    }
}
