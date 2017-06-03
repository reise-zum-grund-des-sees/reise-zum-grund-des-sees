using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReiseZumGrundDesSees.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    class Player : IPlayer
    {
        public Vector3 Position { get; set; } //Position des Spielers

        public IReadOnlyList<IPlayerBlock> Blocks => Blöcke;

        public bool HasMultipleHitboxes => false;
        public Hitbox Hitbox => new Hitbox(Position, 0.8f, 1.5f, 0.8f);
        public Hitbox[] Hitboxes => throw new NotImplementedException();

        public Model Model;
        bool Jump1;//Spieler befindet sich im Sprung 1 (einfacher Sprung)
        bool Jump2;//Spieler befindet sich im Sprung 2 (Doppelsprung
        bool Jumpcd;//Cooldown zwischen zwei Sprüngen, damit nicht beide gleichzeitig getriggert werden
        double Blockcd; // Cooldown zwischen dem Setzen von Blöcken, damit sie nicht ineinander gesetzt werden
        double Levercd;
        public double Healthcd{ get; private set; }
        public float Blickrichtung{ get; private set; } //in Rad
        float BlickrichtungAdd; //schaue in Richtung W/A/S/D
        public List<PlayerBlock> Blöcke; //Liste aller dem Spieler verfügbaren Blöcke
        ContentManager ContentManager;
        float _speedY = 0;
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public Player(ContentManager contentManager, Vector3 _position)
        {
            ContentManager = contentManager;
            Position = _position;
            Jump1 = false;
            Jump2 = false;
            Jumpcd = false;
            Blickrichtung = 0;
            BlickrichtungAdd = 0;
            Blockcd = 0;
            Levercd = 0;
            Healthcd = 1001;
            MaxHealth = 3;
            Health = 3;
            Blöcke = new List<PlayerBlock>();
            Model = contentManager.Load<Model>("spielfigur");
            //Startblöcke, müsssen später auf Pickup hinzugefügt werden
            Blöcke.Add(new PlayerBlock(ContentManager, this, 0));
            Blöcke.Add(new PlayerBlock(ContentManager, this, 0));
            Blöcke.Add(new PlayerBlock(ContentManager, this, 0));
            Blöcke.Add(new PlayerBlock(ContentManager, this, 1));
            Blöcke.Add(new PlayerBlock(ContentManager, this, 1));
            Blöcke.Add(new PlayerBlock(ContentManager, this, 1));
            Blöcke.Add(new PlayerBlock(ContentManager, this, 2));
            Blöcke.Add(new PlayerBlock(ContentManager, this, 2));
            Blöcke.Add(new PlayerBlock(ContentManager, this, 2));
        }

        public UpdateDelegate Update(GameState.View _stateView, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            if (!_flags.HasFlag(GameFlags.GameRunning) | _flags.HasFlag(GameFlags.EditorMode))
                return null;

            // Nicht die Variablen hier ändern. Aber Kollisionserkennung hier berechnen.

            //Sprint noch nicht implementiert
            // float sprint = 1;          
            //if (_inputArgs.Events.HasFlag(InputEventList.Sprint)) sprint = 2;//Sprintgeschwindigkeit

            //Blickrichtung   
            Blickrichtung = -_stateView.CamAngle + (float)Math.PI;

            if ((_inputArgs.Events & InputEventList.MoveForwards) != 0)
            {
                if ((_inputArgs.Events & InputEventList.MoveLeft) != 0)
                    BlickrichtungAdd = MathHelper.PiOver4;
                else if ((_inputArgs.Events & InputEventList.MoveRight) != 0)
                    BlickrichtungAdd = -MathHelper.PiOver4;
                else
                    BlickrichtungAdd = 0;
            }
            else if ((_inputArgs.Events & InputEventList.MoveBackwards) != 0)
            {
                if ((_inputArgs.Events & InputEventList.MoveLeft) != 0)
                    BlickrichtungAdd = MathHelper.PiOver4 * 3;
                else if ((_inputArgs.Events & InputEventList.MoveRight) != 0)
                    BlickrichtungAdd = -MathHelper.PiOver4 * 3;
                else
                    BlickrichtungAdd = MathHelper.Pi;
            }
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft))
                BlickrichtungAdd = MathHelper.PiOver2;
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveRight))
                BlickrichtungAdd = MathHelper.PiOver2 * 3;

            Blickrichtung += BlickrichtungAdd;

            Vector3 _movement = new Vector3(0, 0, 0);

            if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards))
            {
                //_movement.Z -= (float)(_passedTime * 0.005);
                //_movement -= Vector3.Multiply(_stateView.TargetToCam, (float)(_passedTime * 0.001f));
                _movement.X += (float)Math.Sin(_stateView.CamAngle) * (float)(_passedTime * 0.005f);
                _movement.Z -= (float)Math.Cos(_stateView.CamAngle) * (float)(_passedTime * 0.005f);
            }
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards))
            {
                //_movement += Vector3.Multiply(_stateView.TargetToCam, (float)(_passedTime * 0.001f));
                _movement.X -= (float)Math.Sin(_stateView.CamAngle) * (float)(_passedTime * 0.005f);
                _movement.Z += (float)Math.Cos(_stateView.CamAngle) * (float)(_passedTime * 0.005f);
            }

            if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft))
            {
                _movement.X -= (float)Math.Sin(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
                _movement.Z += (float)Math.Cos(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
            }
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveRight))
            {
                _movement.X += (float)Math.Sin(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
                _movement.Z -= (float)Math.Cos(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
            }


            if (_inputArgs.Events.HasFlag(InputEventList.Jump) && Jump1 == false)
            {
                Jump1 = true;
                _speedY = 1.1f;
                Jumpcd = true;
            }

            if (Jump1 == true && !_inputArgs.Events.HasFlag(InputEventList.Jump))
                Jumpcd = false;

            if (Jump1 == true && Jump2 == false && Jumpcd == false && _inputArgs.Events.HasFlag(InputEventList.Jump))//Doppelsprung
            {
                Jump2 = true;
                _speedY = 1.1f;
            }

            _speedY -= 0.005f * (float)_passedTime;
            _movement.Y = _speedY * (float)_passedTime * 0.01f;

            if (_stateView.BlockWorld[(int)Position.X, (int)(Position.Y + 0.65f), (int)Position.Z] != WorldBlock.Water1 &&
                _stateView.BlockWorld[(int)Position.X, (int)(Position.Y + 0.4f), (int)Position.Z] != WorldBlock.Water2 &&
                _stateView.BlockWorld[(int)Position.X, (int)(Position.Y + 0.15f), (int)Position.Z] != WorldBlock.Water3 &&
                _stateView.BlockWorld[(int)Position.X, (int)(Position.Y - 0.1f), (int)Position.Z] != WorldBlock.Water4)
            {
                Direction _info = CollisionDetector.CollisionWithWorld(ref _movement, new Hitbox(Position.X - 0.4f, Position.Y, Position.Z - 0.4f, 0.8f, 1.5f, 0.8f), _stateView.BlockWorld);

                List<Direction> _info2 = new List<Direction>();
                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].Zustand == (int)PlayerBlock.State.Gesetzt)
                        _info2.Add(CollisionDetector.CollisionDetectionWithSplittedMovement(ref _movement, new Hitbox(Position.X - 0.4f, Position.Y, Position.Z - 0.4f, 0.8f, 1.5f, 0.8f),Blöcke[i].Hitbox));

                for (int i = 0; i < _info2.Count; i++)
                {
                    if (_info2[i].HasFlag(Direction.Bottom) && _speedY < 0)
                    {
                        _speedY = 0;
                        Jump1 = false;
                        Jump2 = false;
                        Jumpcd = false;
                    }
                    else if (_info2[i].HasFlag(Direction.Top) && _speedY > 0)
                        _speedY = 0;
                }

                if (_info.HasFlag(Direction.Bottom) && _speedY < 0)
                {
                    _speedY = 0;
                    Jump1 = false;
                    Jump2 = false;
                    Jumpcd = false;
                }
                else if (_info.HasFlag(Direction.Top) && _speedY > 0)
                {
                    _speedY = 0;
                }
            }
            else
            {//Wasser unter Spieler
                Health = 0; // Spieler stirbt
            }

            //Kollision mit Gegner         
            List<Direction> _info3 = new List<Direction>();
            Vector3 movementtemp = _movement;
            for (int i = 0; i < Enemy.EnemyList.Count; i++) {
                _info3.Add(CollisionDetector.CollisionDetectionWithSplittedMovement(ref movementtemp, Hitbox, Enemy.EnemyList[i].Hitbox));
            if((_info3[i].HasFlag(Direction.Back) || _info3[i].HasFlag(Direction.Top) ||_info3[i].HasFlag(Direction.Front) || _info3[i].HasFlag(Direction.Left) ||
                     _info3[i].HasFlag(Direction.Right)) && Healthcd > 1000) {
                Enemy.EnemyList[i].HitPlayer = true;
                Health--;
                Healthcd = 0;
                }
                if (_info3[i].HasFlag(Direction.Bottom))
                {
                    Enemy.EnemyList.RemoveAt(i);
                }
                }
            

            /*
         _stateView.BlockWorld[(int)Position.X, (int)(Position.Y - 0.1f), (int)Position.Z] != WorldBlock.Water1 &&
               _stateView.BlockWorld[(int)Position.X, (int)(Position.Y - 0.1f), (int)Position.Z] != WorldBlock.Water2 && _stateView.BlockWorld[(int)Position.X, (int)(Position.Y - 0.1f), (int)Position.Z] != WorldBlock.Water3
                    && _stateView.BlockWorld[(int)Position.X, (int)(Position.Y - 0.1f), (int)Position.Z] != WorldBlock.Water4
            */
            //Lever press
            if (_inputArgs.Events.HasFlag(InputEventList.Interact) && Levercd >= 1000)
            {
                for (int x = -2; x < 3; x++)
                    for (int y = -2; y < 3; y++)
                        for (int z = -2; z < 3; z++)
                        {
                            ISpecialBlock _obj = _stateView.WorldObjects.ObjectAt(x + (int)Position.X, y + (int)Position.Y, z + (int)Position.Z);
                            if (_obj != null && _obj.Type == WorldBlock.Lever)
                            {
                                float _dist = ChebyshevDistance(Position, _obj.Position + new Vector3(0.5f, 0.5f, 0.5f));
                                if (_dist < 1f)
                                {
                                    Levercd = 0;
                                    (_obj as Lever).press();
                                }
                            }
                        }
            }

            //Take Damage from Spikes

            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    for (int z = -2; z < 3; z++)
                    {
                        {
                            ISpecialBlock _obj = _stateView.WorldObjects.ObjectAt(x + (int)Position.X, y + (int)Position.Y, z + (int)Position.Z);
                            if (_obj != null && _obj.Type == WorldBlock.Spikes)
                            {

                                if (Vector3.Distance(Position, new Vector3(_obj.Position.X + 0.5f, _obj.Position.Y + 0.25f, _obj.Position.Z + 0.5f)) < 1f && Healthcd > 1000)
                                {
                                    Health--;
                                    Healthcd = 0;
                                }
                            }

                        }
                    }
                }
            }

            Levercd += _passedTime;
            Blockcd += _passedTime;      //Zeit erhöhen      
            Healthcd += _passedTime;

            //Setzen von Blöcken
            if (_inputArgs.Events.HasFlag(InputEventList.LeichterBlock) && Blockcd > 1000)
            {

                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].Zustand == (int)PlayerBlock.State.Bereit && Blöcke[i].Art == 0)
                    {
                        Blockcd = 0;
                        Blöcke[i].Zustand = (int)PlayerBlock.State.Übergang;
                        break;
                    }

            }
            if (_inputArgs.Events.HasFlag(InputEventList.MittelschwererBlock) && Blockcd > 1000)
            {

                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].Zustand == (int)PlayerBlock.State.Bereit && Blöcke[i].Art == 1)
                    {
                        Blockcd = 0;
                        Blöcke[i].Zustand = (int)PlayerBlock.State.Übergang;
                        break;
                    }
            }
            if (_inputArgs.Events.HasFlag(InputEventList.SchwererBlock) && Blockcd > 1000)
            {

                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].Zustand == (int)PlayerBlock.State.Bereit && Blöcke[i].Art == 2)
                    {
                        Blockcd = 0;
                        Blöcke[i].Zustand = (int)PlayerBlock.State.Übergang;
                        break;
                    }
            }

            // Löschen mit Taste
            if (_inputArgs.Events.HasFlag(InputEventList.Delete))
            {
                for (int i = 0; i < Blöcke.Count; i++)
                    Blöcke[i].Zustand = (int)PlayerBlock.State.Delete;
            }

            List<UpdateDelegate> blockUpdateList = new List<UpdateDelegate>();
            foreach (PlayerBlock b in Blöcke)
                blockUpdateList.Add(b.Update(_stateView, _flags, _inputArgs, _passedTime));

            //Health<=0 -> sterbe
            if (Health <= 0) gestorben();
            return (ref GameState _state) =>
            {
                this.Position += _movement;
                _state.Camera.ChangePosition(_movement);   //move Camera with Player   
                //Console.WriteLine(Position);

                foreach (UpdateDelegate u in blockUpdateList)
                    u(ref _state);
            };
        }
        public static float ChebyshevDistance(Vector3 a, Vector3 b)
        {
            Vector3 distance = Vector3.Subtract(a, b);
            float res = Math.Abs(distance.X);
            if (Math.Abs(distance.Y) > res) res = Math.Abs(distance.Y);
            if (Math.Abs(distance.Z) > res) res = Math.Abs(distance.Z);
            return res;
        }
        public void gestorben()
        {
            Health = MaxHealth; //Leben wieder voll
            for (int i = 0; i < Blöcke.Count; i++)
                Blöcke[i].Zustand = (int)PlayerBlock.State.Delete; //Bloeke zuruecksetzen
            Position = new Vector3(24, 32, 24); //Position zuruecksetzen, Hardcoded, da man nicht an new Vector3(_world.SpawnPosX, _world.SpawnPosY, _world.SpawnPosZ) rankommt
        }

        public bool CollidesWithWorldBlock(WorldBlock _block)
        {
            throw new NotImplementedException();
        }

        public bool CollidesWithObject(ICollisionObject _object)
        {
            throw new NotImplementedException();
        }
    }
}
