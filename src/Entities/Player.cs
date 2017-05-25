using Microsoft.Xna.Framework;
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
    class Player : IUpdateable, IPositionObject
    {
        public Vector3 Position { get; set; } //Position des Spielers
        public Model Model;
        bool Jump1;//Spieler befindet sich im Sprung 1 (einfacher Sprung)
        bool Jump2;//Spieler befindet sich im Sprung 2 (Doppelsprung
        bool Jumpcd;//Cooldown zwischen zwei Sprüngen, damit nicht beide gleichzeitig getriggert werden
        double Blockcd; // Cooldown zwischen dem Setzen von Blöcken, damit sie nicht ineinander gesetzt werden
        double Levercd;
        public static float Blickrichtung; //in Rad
        float BlickrichtungAdd; //schaue in Richtung W/A/S/D
        public static List<PlayerBlock> Blöcke; //Liste aller dem Spieler verfügbaren Blöcke
        ContentManager ContentManager;
        float _speedY = 0;
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
  
        public UpdateDelegate Update(GameState.View _stateView, InputEventArgs _inputArgs, double _passedTime)
        {
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
                    BlickrichtungAdd= MathHelper.Pi;
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

            if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft)) {
                _movement.X -= (float)Math.Sin(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
                _movement.Z += (float)Math.Cos(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
            }
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveRight))
            {
                _movement.X += (float)Math.Sin(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
                _movement.Z -= (float)Math.Cos(_stateView.CamAngle + MathHelper.PiOver2) * (float)(_passedTime * 0.005f);
            }
                
  
            if (_inputArgs.Events.HasFlag(InputEventList.Jump) && Jump1==false) {
                Jump1 = true;
                _speedY = 1.1f;
                Jumpcd = true;
            }

            if (Jump1 == true && !_inputArgs.Events.HasFlag(InputEventList.Jump))
                Jumpcd = false;

            if (Jump1==true && Jump2==false && Jumpcd==false && _inputArgs.Events.HasFlag(InputEventList.Jump))//Doppelsprung
            {
                Jump2 = true;                 
                _speedY = 1.1f;
            }
           
            _speedY -= 0.005f * (float)_passedTime;
            _movement.Y = _speedY * (float)_passedTime * 0.01f;

            Direction _info = CollisionDetector.CollisionWithWorld(ref _movement, new Hitbox(Position.X - 0.4f, Position.Y, Position.Z - 0.4f, 0.8f, 1.5f, 0.8f), _stateView.BlockWorld);
          
            List<Direction> _info2 = new List<Direction>();
            for (int i = 0; i < Blöcke.Count; i++)
                if(Blöcke[i].Zustand==(int)PlayerBlock.ZustandList.Gesetzt)
                _info2.Add(CollisionDetector.CollisionWithObject(ref _movement, new Hitbox(Position.X, Position.Y, Position.Z, 0.8f, 1.5f, 0.8f), new Hitbox(Blöcke[i].Position.X, Blöcke[i].Position.Y, Blöcke[i].Position.Z, 1f, 1f, 1f)));
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
                _speedY = 0;

            /*if (_speedY == 0)
            {
                Jump1 = false;
                Jump2 = false;
                Jumpcd = false;
            }*/
            //Lever collisions
            
            List<Direction> _infoLever = new List<Direction>();
            for (int i = 0; i < Lever.LeverList.Count; i++) { 
              _infoLever.Add(CollisionDetector.CollisionWithObject(ref _movement, new Hitbox(Position.X, Position.Y, Position.Z, 0.8f, 1.5f, 0.8f),Lever.LeverList[i].Hitbox));
            }
            
            for (int i = 0; i < _infoLever.Count; i++)
            {
                if (_infoLever[i].HasFlag(Direction.Back) && Lever.LeverList[i].Rotation==0 ||
                   _infoLever[i].HasFlag(Direction.Left) && Lever.LeverList[i].Rotation == Math.PI/2 ||
                   _infoLever[i].HasFlag(Direction.Front) && Lever.LeverList[i].Rotation == Math.PI ||
                   _infoLever[i].HasFlag(Direction.Right) && Lever.LeverList[i].Rotation == Math.PI*2/3)
                    if(_inputArgs.Events.HasFlag(InputEventList.MoveDown))
                        if (Levercd >= 5000) {
                            Levercd = 0;
                        Lever.LeverList[i].press(); //Bottem ist Starposition vorne
                        }
            }
            Levercd += _passedTime;
            Blockcd += _passedTime;      //Zeit erhöhen      
            
            //Setzen von Blöcken
            if (_inputArgs.Events.HasFlag(InputEventList.LeichterBlock)  && Blockcd > 1000)
            {
                           
                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].Zustand == (int)PlayerBlock.ZustandList.Bereit && Blöcke[i].Art == 0)
                    {
                        Blockcd = 0;
                        Blöcke[i].Zustand = (int)PlayerBlock.ZustandList.Übergang;
                        break;
                    }
               
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MittelschwererBlock) && Blockcd > 1000)
            {
             
                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].Zustand == (int)PlayerBlock.ZustandList.Bereit && Blöcke[i].Art == 1)
                    {
                        Blockcd = 0;
                        Blöcke[i].Zustand = (int)PlayerBlock.ZustandList.Übergang;
                        break;
                    }
            }
            if (_inputArgs.Events.HasFlag(InputEventList.SchwererBlock) && Blockcd > 1000)
            {
             
                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].Zustand == (int)PlayerBlock.ZustandList.Bereit && Blöcke[i].Art == 2)
                    {
                        Blockcd = 0;
                        Blöcke[i].Zustand = (int)PlayerBlock.ZustandList.Übergang;
                        break;
                    }
            }

       // Löschen mit Taste
            if (_inputArgs.Events.HasFlag(InputEventList.Delete))
            {
                for (int i = 0; i < Blöcke.Count; i++)
                    Blöcke[i].Zustand = (int)PlayerBlock.ZustandList.Delete;
            }

            List<UpdateDelegate> blockUpdateList = new List<UpdateDelegate>();
            foreach (PlayerBlock b in Blöcke)
                blockUpdateList.Add(b.Update(_stateView, _inputArgs, _passedTime));

            return (ref GameState _state) =>
            {
                _state.Player.Position += _movement;
                _state.Camera.ChangePosition(_movement);   //move Camera with Player   
                //Console.WriteLine(Position);

                foreach (UpdateDelegate u in blockUpdateList)
                    u(ref _state);
            };
        }

    }
}
