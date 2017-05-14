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
    class Player2 : IUpdateable
    {
        public Vector3 Position;
        public Model Model;
        bool Jump1;
        bool Jump2;
        double CurrentJumpTime;
        double BlickTime;
        double Blockcd;
        public int Blickrichtung;
        public static List<PlayerBlock> Blöcke;
        public BoundingBox Box;
        ContentManager ContentManager;
        public float FallOffset = 0.8f;//wann fällt der Spieler runter, soll zwischen 0.5f-1f liegen, je höher desto mehr Probleme treten bei Mapblöcken auf
        
        public Player2(ContentManager contentManager, Vector3 _position)
        {
            ContentManager = contentManager;
            Position = _position;
            Jump1 = false;
            Jump2 = false;
            CurrentJumpTime = 0;
            Blickrichtung = 0;
            BlickTime = 0;
            Blockcd = 0;
            Blöcke = new List<PlayerBlock>();
            Model = contentManager.Load<Model>("spielfigur");
        }

        float _speedY = 0;

        public UpdateDelegate Update(GameState.View _stateView, InputEventArgs _inputArgs, double _passedTime)
        {
            // Nicht die Variablen hier ändern. Aber Kollisionserkennung hier berechnen.

            Vector3 _movement = new Vector3(0, 0, 0);

            if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards))
                _movement.Z -= (float)(_passedTime * 0.005);
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards))
                _movement.Z += (float)(_passedTime * 0.005);

            if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft))
                _movement.X -= (float)(_passedTime * 0.005);
            else if (_inputArgs.Events.HasFlag(InputEventList.MoveRight))
                _movement.X += (float)(_passedTime * 0.005);

            if (_inputArgs.Events.HasFlag(InputEventList.Jump))
                _speedY = 1;

            _speedY -= 0.005f * (float)_passedTime;
            _movement.Y = _speedY * (float)_passedTime * 0.005f;

            Direction _info = CollisionDetector.CollisionWithWorld(ref _movement, new Hitbox(Position.X - 0.4f, Position.Y, Position.Z - 0.4f, 0.8f, 0.8f, 1.5f), _stateView);

            if (_info.HasFlag(Direction.Bottom) && _speedY < 0)
                _speedY = 0;
            else if (_info.HasFlag(Direction.Top) && _speedY > 0)
                _speedY = 0;

            return (ref GameState _state) =>
            {
                _state.Player.Position += _movement;
            };
        }

    }
}
