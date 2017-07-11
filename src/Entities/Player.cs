using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public IList<IPlayerBlock> Blocks => Blöcke;
        IReadOnlyList<IReadonlyPlayerBlock> IReadonlyPlayer.Blocks => (IReadOnlyList<IPlayerBlock>)Blöcke;

        public bool HasMultipleHitboxes => false;
        public Hitbox Hitbox => new Hitbox(Position - new Vector3(hitboxSize.X * 0.5f, 0, hitboxSize.Z * 0.5f), hitboxSize);
        private readonly Vector3 hitboxSize = new Vector3(0.8f, 1.5f, 0.8f);
        public Hitbox[] Hitboxes => throw new NotImplementedException();
        public bool IsEnabled => true;

        private bool wasAddedToCollisionManager = false;

        private Model model;
        private Effect effect;

        bool Jump1;//Spieler befindet sich im Sprung 1 (einfacher Sprung)
        bool Jump2;//Spieler befindet sich im Sprung 2 (Doppelsprung
        bool Jumpcd;//Cooldown zwischen zwei Sprüngen, damit nicht beide gleichzeitig getriggert werden
        double Blockcd; // Cooldown zwischen dem Setzen von Blöcken, damit sie nicht ineinander gesetzt werden
        double Levercd;
        public double Healthcd { get; private set; }
        public float Blickrichtung { get; private set; } //in Rad
        float BlickrichtungAdd; //schaue in Richtung W/A/S/D
        public IList<IPlayerBlock> Blöcke; //Liste aller dem Spieler verfügbaren Blöcke
        ContentManager ContentManager;
        GraphicsDevice GraphicDevice;
        float _speedY = 0;
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        List<SoundEffect> soundEffects;

        //wie viel Blöcke hat der Spieler bereit
        public static int AnzahlBlockReadyL = 0;
        public static int AnzahlBlockReadyM = 0;
        public static int AnzahlBlockReadyS = 0;

        public Player(Vector3 _position)
        {
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
            Blöcke = new List<IPlayerBlock>();
            //Startblöcke, müsssen später auf Pickup hinzugefügt werden

            Blöcke.Add(new PlayerBlock(this, 0));
            Blöcke.Add(new PlayerBlock(this, 0));
            Blöcke.Add(new PlayerBlock(this, 0));
            Blöcke.Add(new PlayerBlock(this, 1));
            Blöcke.Add(new PlayerBlock(this, 1));
            Blöcke.Add(new PlayerBlock(this, 1));
            Blöcke.Add(new PlayerBlock(this, 2));
            Blöcke.Add(new PlayerBlock(this, 2));
            Blöcke.Add(new PlayerBlock(this, 2));

        }

        public Player(ConfigFile.ConfigNode _playerNode) :
            this(_playerNode.Items["position"].ToVector3())
        {
            Health = int.Parse(_playerNode.Items["health"]);
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
                if (Jump1)
                {
                    _movement.X += (float)Math.Sin(_stateView.CamAngle) * (float)(_passedTime * 0.003f);
                    _movement.Z -= (float)Math.Cos(_stateView.CamAngle) * (float)(_passedTime * 0.003f);
                }
                else
                {
                    _movement.X += (float)Math.Sin(_stateView.CamAngle) * (float)(_passedTime * 0.005f);
                    _movement.Z -= (float)Math.Cos(_stateView.CamAngle) * (float)(_passedTime * 0.005f);
                }
            }
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards))
            {
                //_movement += Vector3.Multiply(_stateView.TargetToCam, (float)(_passedTime * 0.001f));
                if (Jump1)
                {
                    _movement.X -= (float)Math.Sin(_stateView.CamAngle) * (float)(_passedTime * 0.003f);
                    _movement.Z += (float)Math.Cos(_stateView.CamAngle) * (float)(_passedTime * 0.003f);
                }
                else
                {
                    _movement.X -= (float)Math.Sin(_stateView.CamAngle) * (float)(_passedTime * 0.005f);
                    _movement.Z += (float)Math.Cos(_stateView.CamAngle) * (float)(_passedTime * 0.005f);
                }
            }

            if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft))
            {
                if (Jump1)
                {
                    _movement.X -= (float)Math.Sin(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.003f);
                    _movement.Z += (float)Math.Cos(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.003f);
                }
                else
                {
                    _movement.X -= (float)Math.Sin(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
                    _movement.Z += (float)Math.Cos(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
                }
            }
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveRight))
            {
                if (Jump1)
                {
                    _movement.X += (float)Math.Sin(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.003f);
                    _movement.Z -= (float)Math.Cos(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.003f);
                }
                else
                {
                    _movement.X += (float)Math.Sin(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
                    _movement.Z -= (float)Math.Cos(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
                }
            }


            if (_inputArgs.Events.HasFlag(InputEventList.Jump) && Jump1 == false)
            {
                Jump1 = true;
                _speedY = 0.9f;//1.1f;
                Jumpcd = true;
                soundEffects[0].Play();
            }

            if (Jump1 == true && !_inputArgs.Events.HasFlag(InputEventList.Jump))
                Jumpcd = false;

            if (Jump1 == true && Jump2 == false && Jumpcd == false && _inputArgs.Events.HasFlag(InputEventList.Jump))//Doppelsprung
            {
                Jump2 = true;
                _speedY = 0.9f;
                soundEffects[0].Play();
            }

            _speedY -= 0.005f * (float)_passedTime;
            _movement.Y = _speedY * (float)_passedTime * 0.01f;

            var _collisionInformation = _stateView.CollisionDetector.CheckCollision(ref _movement, this);

            if (_collisionInformation.ContainsKey(Direction.Bottom) &&
                _collisionInformation[Direction.Bottom].CollisionType == CollisionDetector.CollisionSource.Type.WithObject &&
                _collisionInformation[Direction.Bottom].Object is IMovingObject _moving &&
                _moving.Velocity != Vector3.Zero)
            {
                _movement += _moving.Velocity;
                _collisionInformation = _stateView.CollisionDetector.CheckCollision(ref _movement, this);
                Jump1 = false;
                Jump2 = false;
                Jumpcd = false;
            }

            if (_collisionInformation.ContainsKey(Direction.Bottom))
            {
                if (_speedY < 0)
                {
                    _speedY = 0;
                    Jump1 = false;
                    Jump2 = false;
                    Jumpcd = false;
                }

                if (_collisionInformation[Direction.Bottom].CollisionType == CollisionDetector.CollisionSource.Type.WithWorldBlock &&
                    _collisionInformation[Direction.Bottom].WorldBlock.IsWater())
                    Health = 0;
            }
            else if (_collisionInformation.ContainsKey(Direction.Top))
                if (_speedY > 0)
                    _speedY = 0;

            //Take Damage from Spikes

            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    for (int z = -2; z < 3; z++)
                    {
                        {
                            ISpecialBlock _obj = _stateView.WorldObjects.BlockAt(x + (int)Position.X, y + (int)Position.Y, z + (int)Position.Z);
                            if (_obj != null && _obj.Type == WorldBlock.Spikes)
                            {

                                if (Vector3.Distance(Position, new Vector3(_obj.Position.X + 0.5f, _obj.Position.Y + 0.25f, _obj.Position.Z + 0.5f)) < 1f)
                                {
                                    Hit();
                                }
                            }

                        }
                    }
                }
            }

            //Aufsammeln von PlayerBlöcken
            for (int i = 0; i < GetPlayerBlock.GetPlayerBlockList.Count; i++)
            {

                if (Vector3.Distance(Position, new Vector3(GetPlayerBlock.GetPlayerBlockList[i].Position.X + 0.5f,
                    GetPlayerBlock.GetPlayerBlockList[i].Position.Y + 0.25f, GetPlayerBlock.GetPlayerBlockList[i].Position.Z + 0.5f)) < 1f)
                {

                    if (GetPlayerBlock.GetPlayerBlockList[i].Art == 0)
                        Blöcke.Add(new PlayerBlock(this, 0));
                    if (GetPlayerBlock.GetPlayerBlockList[i].Art == 1)
                        Blöcke.Add(new PlayerBlock(this, 1));
                    if (GetPlayerBlock.GetPlayerBlockList[i].Art == 2)
                        Blöcke.Add(new PlayerBlock(this, 2));
                    Blöcke[Blöcke.Count - 1].Initialize(GraphicDevice, ContentManager);
                    GetPlayerBlock.GetPlayerBlockList.RemoveAt(i);
                }
            }


            Levercd += _passedTime;
            Blockcd += _passedTime;      //Zeit erhöhen      
            Healthcd += _passedTime;

            //Setzen von Blöcken
            if (_inputArgs.Events.HasFlag(InputEventList.LeichterBlock) && Blockcd > 500)
            {

                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].CurrentState == PlayerBlock.State.Bereit && Blöcke[i].BlockType == PlayerBlock.Type.Light)
                    {
                        Blockcd = 0;
                        (Blöcke[i] as PlayerBlock).Zustand = (int)PlayerBlock.State.Übergang;
                        soundEffects[3].Play();
                        break;
                    }

            }
            if (_inputArgs.Events.HasFlag(InputEventList.MittelschwererBlock) && Blockcd > 500)
            {

                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].CurrentState == PlayerBlock.State.Bereit && Blöcke[i].BlockType == PlayerBlock.Type.Medium)
                    {
                        Blockcd = 0;
                        (Blöcke[i] as PlayerBlock).Zustand = (int)PlayerBlock.State.Übergang;
                        soundEffects[3].Play();
                        break;
                    }
            }
            if (_inputArgs.Events.HasFlag(InputEventList.SchwererBlock) && Blockcd > 500)
            {

                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].CurrentState == PlayerBlock.State.Bereit && Blöcke[i].BlockType == PlayerBlock.Type.Heavy)
                    {
                        Blockcd = 0;
                        (Blöcke[i] as PlayerBlock).Zustand = (int)PlayerBlock.State.Übergang;
                        soundEffects[3].Play();
                        break;
                    }
            }
            //Soundeffekt wenn nicht bereit
            bool BlockReadyL = true;
            bool BlockReadyM = true;
            bool BlockReadyS = true;
            AnzahlBlockReadyL = 0;
            AnzahlBlockReadyM = 0;
            AnzahlBlockReadyS = 0;
            for (int i = 0; i < Blöcke.Count; i++)
            {
                if (Blöcke[i].CurrentState != PlayerBlock.State.Bereit && Blöcke[i].BlockType == PlayerBlock.Type.Light)
                    BlockReadyL = false;
                if (Blöcke[i].CurrentState == PlayerBlock.State.Bereit && Blöcke[i].BlockType == PlayerBlock.Type.Light)
                    AnzahlBlockReadyL++;
                if (Blöcke[i].CurrentState != PlayerBlock.State.Bereit && Blöcke[i].BlockType == PlayerBlock.Type.Medium)
                    BlockReadyM = false;
                if (Blöcke[i].CurrentState == PlayerBlock.State.Bereit && Blöcke[i].BlockType == PlayerBlock.Type.Medium)
                    AnzahlBlockReadyM++;
                if (Blöcke[i].CurrentState != PlayerBlock.State.Bereit && Blöcke[i].BlockType == PlayerBlock.Type.Heavy)
                    BlockReadyS = false;
                if (Blöcke[i].CurrentState == PlayerBlock.State.Bereit && Blöcke[i].BlockType == PlayerBlock.Type.Heavy)
                    AnzahlBlockReadyS++;
            }
            if (Blockcd > 100 && _inputArgs.Events.HasFlag(InputEventList.LeichterBlock) && BlockReadyL == false)
                soundEffects[5].Play();
            if (Blockcd > 100 && _inputArgs.Events.HasFlag(InputEventList.MittelschwererBlock) && BlockReadyM == false)
                soundEffects[5].Play();
            if (Blockcd > 100 && _inputArgs.Events.HasFlag(InputEventList.SchwererBlock) && BlockReadyS == false)
                soundEffects[5].Play();


            // Löschen mit Taste
            if (_inputArgs.Events.HasFlag(InputEventList.Delete))
            {
                soundEffects[6].Play();
                for (int i = 0; i < Blöcke.Count; i++)
                    (Blöcke[i] as PlayerBlock).Zustand = (int)PlayerBlock.State.Delete;
            }
            //Davorstehenden Block loeschen
            Vector3 Blick = Vector3.Transform(new Vector3(0, 0, -1), Matrix.CreateRotationY(Blickrichtung));
            Blick.Normalize();
            Vector3 BlickPosition = new Vector3(-Blick.X, 0, -Blick.Z); //Blickrichtung des Spielers
            Blick = Vector3.Add(Position, BlickPosition);

            if (_inputArgs.Events.HasFlag(InputEventList.Return))
                for (int i = 0; i < Blöcke.Count; i++)
                    if (Vector3.Distance(Blöcke[i].Position, Blick) < 1 && (Blöcke[i] as PlayerBlock).Zustand == (int)PlayerBlock.State.Gesetzt)
                    {
                        (Blöcke[i] as PlayerBlock).Zustand = (int)PlayerBlock.State.Delete;
                        soundEffects[6].Play();
                    }
            List<UpdateDelegate> blockUpdateList = new List<UpdateDelegate>();
            foreach (PlayerBlock b in Blöcke)
                blockUpdateList.Add(b.Update(_stateView, _flags, _inputArgs, _passedTime));

            //Health<=0 -> sterbe
            if (Health <= 0) gestorben();
            return (ref GameState _state) =>
            {
                this.Position += _movement;
                //Console.WriteLine(Position);

                //Lever press
                if (_inputArgs.Events.HasFlag(InputEventList.Interact) && Levercd >= 1000)
                {
                    for (int x = -2; x < 3; x++)
                        for (int y = -2; y < 3; y++)
                            for (int z = -2; z < 3; z++)
                            {
                                ISpecialBlock _obj = _stateView.WorldObjects.BlockAt(x + (int)Position.X, y + (int)Position.Y, z + (int)Position.Z);
                                if (_obj != null && _obj.Type == WorldBlock.Lever)
                                {
                                    float _dist = ChebyshevDistance(Position, _obj.Position + new Vector3(0.5f, 0.5f, 0.5f));
                                    if (_dist < 1f)
                                    {
                                        Levercd = 0;
                                        (_obj as Lever).Press(_state);
                                    }
                                }
                            }
                }


                foreach (UpdateDelegate u in blockUpdateList)
                    u(ref _state);

                if (_collisionInformation.ContainsKey(Direction.Bottom) &&
                    _collisionInformation[Direction.Bottom].CollisionType == CollisionDetector.CollisionSource.Type.WithObject &&
                    _collisionInformation[Direction.Bottom].Object is IHitable h)
                {
                    h.Hit();
                    soundEffects[4].Play();
                }

                if (!wasAddedToCollisionManager)
                {
                    _state.CollisionDetector.AddObject(this);
                    wasAddedToCollisionManager = true;
                }
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

        public void Move(Vector3 _movement, IReadonlyCollisionDetector _collDetector)
        {
            _collDetector.CheckCollision(ref _movement, this);
            Position += _movement;
        }

        public void gestorben()
        {
            soundEffects[1].Play();
            Health = MaxHealth; //Leben wieder voll
            for (int i = 0; i < Blöcke.Count; i++)
                (Blöcke[i] as PlayerBlock).Zustand = (int)PlayerBlock.State.Delete; //Bloeke zuruecksetzen
            Position = new Vector3(146.5f, 34, 194.5f); //Position zuruecksetzen, Hardcoded, da man nicht an new Vector3(_world.SpawnPosX, _world.SpawnPosY, _world.SpawnPosZ) rankommt
        }

        public void Hit()
        {
            if (Healthcd > 1000)
            {
                Health--;
                Healthcd = 0;
                soundEffects[2].Play();
            }
        }


        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            GraphicDevice = _graphicsDevice;
            model = ContentManager.Load<Model>(Content.MODEL_SPIELFIGUR);
            effect = ContentManager.Load<Effect>(Content.EFFECT_PLAYER);
            soundEffects = new List<SoundEffect>();
            soundEffects.Add(ContentManager.Load<SoundEffect>(Content.SOUND_JUMPING)); // Springen
            soundEffects.Add(ContentManager.Load<SoundEffect>(Content.SOUND_DIE)); //Sterben
            soundEffects.Add(ContentManager.Load<SoundEffect>(Content.SOUND_GETHIT)); //schaden bekommen
            soundEffects.Add(ContentManager.Load<SoundEffect>(Content.SOUND_KLONG)); //Block setzen
            soundEffects.Add(ContentManager.Load<SoundEffect>(Content.SOUND_BLOP)); //Gegner stirbt
            soundEffects.Add(ContentManager.Load<SoundEffect>(Content.SOUND_ERROR)); //wenn cd von Blöcken
            soundEffects.Add(ContentManager.Load<SoundEffect>(Content.SOUND_RESET)); //wenn cd von Blöcken

            foreach (var _block in Blöcke)
                _block.Initialize(_graphicsDevice, _contentManager);
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix, GraphicsDevice _grDevice)
        {
            foreach (var _block in Blöcke)
                _block.Render(_flags, _viewMatrix, _perspectiveMatrix, _grDevice);

            if (!(Healthcd <= 1000 && Healthcd % 100 < 50))
            {
                _grDevice.RasterizerState = RasterizerState.CullNone;
                foreach (ModelMesh _mesh in model.Meshes)
                {
                    foreach (ModelMeshPart _part in _mesh.MeshParts)
                    {
                        _part.Effect = effect;

                        effect.Parameters["WorldViewProjection"].SetValue(
                            Matrix.CreateRotationY(Blickrichtung) *
                            Matrix.CreateTranslation(Position) *
                            _viewMatrix *
                            _perspectiveMatrix);
                    }

                    _mesh.Draw();
                }
            }
        }

        public ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper)
        {
            ConfigFile.ConfigNode n = new ConfigFile.ConfigNode();

            n.Items["position"] = Position.ToNiceString();
            n.Items["health"] = Health.ToString();

            return n;
        }
    }
}
