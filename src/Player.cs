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
    class Player : IUpdateable
    {
        public Vector3 Position;
        public Model Model;
        bool Jump1;
        bool Jump2;
        bool Jumpcd;
        double BlickTime;
        double Blockcd;
        public static float Blickrichtung; //in Grad
        Vector3 NullGrad = new Vector3(0,5,5);
        public static List<PlayerBlock> Blöcke;
        ContentManager ContentManager;
        public float FallOffset = 0.8f; //wann fällt der Spieler runter, soll zwischen 0.5f-1f liegen, je höher desto mehr Probleme treten bei Mapblöcken auf
        float _speedY = 0;
        public Player(ContentManager contentManager, Vector3 _position)
        {
            ContentManager = contentManager;
            Position = _position;
            Jump1 = false;
            Jump2 = false;
            Jumpcd = false;
            Blickrichtung = 4;
            BlickTime = 0;
            Blockcd = 0;
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
            Blickrichtung = (Vector3.Dot(_stateView.TargetToCam, NullGrad) / (_stateView.TargetToCam.Length() * NullGrad.Length()));//1 Vorne, 0 Hinten
            if (_stateView.TargetToCam.X > 0) Blickrichtung *= -1; // Vorzeichen der Seite
            Vector3 _movement = new Vector3(0, 0, 0);
            Vector2 Rotation = new Vector2(_stateView.TargetToCam.X, _stateView.TargetToCam.Z);
            //Console.WriteLine(Vector3.Distance(_stateView.TargetToCam, NullGrad));
           
            if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards))
                //_movement.Z -= (float)(_passedTime * 0.005);
            _movement -= Vector3.Multiply(_stateView.TargetToCam, (float)(_passedTime * 0.001f));
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards))
                _movement += Vector3.Multiply(_stateView.TargetToCam, (float)(_passedTime * 0.001f));

            if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft)) {
                _movement.X -= _stateView.TargetToCam.Z * (float)(_passedTime * 0.001f);
                _movement.Z += _stateView.TargetToCam.X * (float)(_passedTime * 0.001f);
            }
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveRight))
            {
                _movement.X += _stateView.TargetToCam.Z * (float)(_passedTime * 0.001f);
                _movement.Z -= _stateView.TargetToCam.X * (float)(_passedTime * 0.001f);
            }
                
  
            if (_inputArgs.Events.HasFlag(InputEventList.Jump) && Jump1==false) {
                Jump1 = true;
                _speedY = 1;
                Jumpcd = true;
            }

            if (Jump1 == true && !_inputArgs.Events.HasFlag(InputEventList.Jump))
                Jumpcd = false;

            if (Jump1==true && Jump2==false && Jumpcd==false && _inputArgs.Events.HasFlag(InputEventList.Jump))//Doppelsprung
            {
                Jump2 = true;                 
                _speedY = 1;
            }
           
            _speedY -= 0.005f * (float)_passedTime;
            _movement.Y = _speedY * (float)_passedTime * 0.01f;

            Direction _info = CollisionDetector.CollisionWithWorld(ref _movement, new Hitbox(Position.X - 0.4f, Position.Y, Position.Z - 0.4f, 0.8f, 0.8f, 1.5f), _stateView);
            List<Direction> _info2 = new List<Direction>();
            for (int i = 0; i < Blöcke.Count; i++)
                if(Blöcke[i].Zustand==(int)PlayerBlock.ZustandList.Gesetzt)
                _info2.Add(CollisionDetector.CollisionWithObject(ref _movement, new Hitbox(Position.X, Position.Y, Position.Z, 0.8f, 0.8f, 1.5f), new Hitbox(Blöcke[i].Position.X, Blöcke[i].Position.Y, Blöcke[i].Position.Z, 0.8f, 0.8f, 1f)));
            for (int i = 0; i < _info2.Count; i++)
            {
                if (_info2[i].HasFlag(Direction.Bottom) && _speedY < 0)
                    _speedY = 0;
                else if (_info2[i].HasFlag(Direction.Top) && _speedY > 0)
                    _speedY = 0;
            }

            if (_info.HasFlag(Direction.Bottom) && _speedY < 0)
                _speedY = 0;
            else if (_info.HasFlag(Direction.Top) && _speedY > 0)
                _speedY = 0;

            if (_speedY == 0)
            {
                Jump1 = false;
                Jump2 = false;
                Jumpcd = false;
            }

            Blockcd += _passedTime;
            //Beim finden neuer Blöcke ins Array
      
            
            
            if (_inputArgs.Events.HasFlag(InputEventList.LeichterBlock)  && Blockcd > 1000)
            {
                           
                for (int i = 0; i < Blöcke.Count; i++)
                    if (Blöcke[i].Zustand == (int)PlayerBlock.ZustandList.Bereit && Blöcke[i].Art == 0)
                    {
                        Blockcd = 0;
                        Blöcke[i].Zustand = (int)PlayerBlock.ZustandList.Übergang;
                        Console.WriteLine("0");
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

            /*
            for (int i = 0; i < Blöcke.Count; i++)
            {
                if (Blöcke.ElementAt(i).AktuelleDauer > PlayerBlock.MaximialDauer)
                    Blöcke.RemoveAt(i);
            }
            */

            if (_inputArgs.Events.HasFlag(InputEventList.Delete))
            {
                for (int i = 0; i < Blöcke.Count; i++)
                    Blöcke[i].Zustand = (int)PlayerBlock.ZustandList.Delete;
            }
            return (ref GameState _state) =>
            {
                _state.Player.Position += _movement;
                _state.Camera.ChangePosition(_movement);   //move Camera with Player   
                //Console.WriteLine(Position);
            };
        }

    }
}
