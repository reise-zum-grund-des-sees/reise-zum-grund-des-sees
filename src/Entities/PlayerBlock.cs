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

        public float LifetimePercentage => throw new NotImplementedException();

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

        public static int AnzahlL = 0;
        public static int MaximumL = 3;
        public static int AnzahlM = 0;
        public static int MaximumM = 3;
        public static int AnzahlS = 0;
        public static int MaximumS = 3;
        public static double MaximialDauer;
        public double AktuelleDauer;
        float _speedY;
        Vector3 _movement;
        public int Art;
        public Model Model;
        public Vector3 Position;
        public int Zustand = 0;
        public double Deletetime;
        private bool wasAddedToCollisionManager = false;

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


        public PlayerBlock(ContentManager contentManager, Player _player, int ArtdesBlocks)
        {

            Art = ArtdesBlocks;
            _speedY = 0;
            _movement = new Vector3(0, 0, 0);
            AktuelleDauer = 0;
            MaximialDauer = 15000;
            Position = _player.Position;

            Zustand = (int)State.Bereit;
            if (Art == 0)
            {//leichterBlock
             //  AnzahlL++;
                Model = contentManager.Load<Model>("leichter_Block");
            }
            if (Art == 1)//MittelschwererBlock
            {
                // AnzahlM++;
                Model = contentManager.Load<Model>("mittelschwerer_Block");
            }
            if (Art == 2)//SchwererBlock
            {
                // AnzahlS++;             
                Model = contentManager.Load<Model>("schwerer_Block");
            }




        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            //Löschen aller Blöcke und Setze CD aller Blöcke auf 5 Sekunden
            if (Zustand == (int)State.Delete)
            {
                Deletetime += _passedTime;
                AktuelleDauer = 0;
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
                Vector3 Blick = Vector3.Transform(new Vector3(0, 0, -1), Matrix.CreateRotationY(Player.Blickrichtung));
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
            }

            else if (MaximialDauer < AktuelleDauer && Zustand == (int)State.Gesetzt)
            {
                Zustand = (int)State.CD;
                //Zerstöre Objekt


            }
            else
            {
                //Objekt ist Tot
                //MaximumL als allgemeines Maximum, müsste addiertes Maximum sein, da ist cd aber zu hoch
                if (MaximumL * MaximialDauer <= AktuelleDauer && Zustand == (int)State.CD)
                {
                    Zustand = (int)State.Bereit;
                    AktuelleDauer = 0;


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
    }
}
