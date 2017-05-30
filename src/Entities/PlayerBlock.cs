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
    class PlayerBlock : IUpdateable
    {
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
        public int Zustand=0;
        public double Deletetime;
        public enum ZustandList
        {
            Bereit = 0,
            Gesetzt = 1,
            CD = 2,
            Übergang = 3,
            Delete = 4
          
        }
        public PlayerBlock(ContentManager contentManager, Player _player,int ArtdesBlocks)
        {

            Art = ArtdesBlocks;
            _speedY = 0;
            _movement = new Vector3(0, 0, 0);
            AktuelleDauer = 0;
            MaximialDauer = 15000;
            Position = _player.Position;
           
            Zustand = (int)ZustandList.Bereit;
            if (Art == 0){//leichterBlock
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
            if(Zustand == (int)ZustandList.Delete)
            {
                Deletetime += _passedTime;
                AktuelleDauer = 0;
                if (Deletetime >= 5000)
                {
                    Deletetime = 0;
                    Zustand = (int)ZustandList.Bereit;
                }
            }
            
            if (Zustand == (int)ZustandList.Übergang) {
                //Position des Blockes basierend auf Blickrichtung
                Position = new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ);
                Vector3 Blick =  Vector3.Transform(new Vector3(0,0,-1), Matrix.CreateRotationY(Player.Blickrichtung));
                Blick.Normalize();
                Position -= new Vector3(Blick.X * 1.5f, 0, Blick.Z * 1.5f);
                //Console.WriteLine(Position);
                AktuelleDauer = 0;
                Zustand = (int)ZustandList.Gesetzt;
            }
            if(Zustand == (int)ZustandList.Gesetzt || Zustand == (int)ZustandList.CD)
            AktuelleDauer += _passedTime;//Update Timer

            if (MaximialDauer >= AktuelleDauer && Zustand == (int)ZustandList.Gesetzt)
            {           
                _movement = new Vector3(0,0, 0);
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
              
               
                    List<Direction> _info2 = new List<Direction>();
                for (int i = 0; i < Player.Blöcke.Count; i++)
                    if (Vector3.Distance(Position, Player.Blöcke[i].Position) != 0 && Player.Blöcke[i].Zustand==(int)PlayerBlock.ZustandList.Gesetzt)
                    {
                        _info2.Add(CollisionDetector.CollisionDetectionWithSplittedMovement(ref _movement, new Hitbox(Position.X, Position.Y, Position.Z, 1f, 1f, 1f), new Hitbox(Player.Blöcke[i].Position.X, Player.Blöcke[i].Position.Y, Player.Blöcke[i].Position.Z, 1f, 1f, 1f)));                        
                    }
                for (int i = 0; i < _info2.Count-1; i++)
                    if (_info2[i].HasFlag(Direction.Bottom) && _speedY < 0)
                    _speedY = 0;
              

                if ((_view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water1 ||
             _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water2 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water3
                 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water4 )||(
                
                 _view.BlockWorld[(int)(Position.X+0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z + 0.5f)] == WorldBlock.Water1 ||
             _view.BlockWorld[(int)(Position.X + 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z + 0.5f)] == WorldBlock.Water2 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water3
                  || _view.BlockWorld[(int)(Position.X + 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z + 0.5f)] == WorldBlock.Water4) ||(

                           _view.BlockWorld[(int)(Position.X - 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z + 0.5f)] == WorldBlock.Water1 ||
             _view.BlockWorld[(int)(Position.X - 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z + 0.5f)] == WorldBlock.Water2 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water3
                || _view.BlockWorld[(int)(Position.X - 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z + 0.5f)] == WorldBlock.Water4 )||(

                           _view.BlockWorld[(int)(Position.X + 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z - 0.5f)] == WorldBlock.Water1 ||
             _view.BlockWorld[(int)(Position.X + 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z - 0.5f)] == WorldBlock.Water2 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water3
                 || _view.BlockWorld[(int)(Position.X + 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z - 0.5f)] == WorldBlock.Water4 )||(

                           _view.BlockWorld[(int)(Position.X - 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z - 0.5f)] == WorldBlock.Water1 ||
             _view.BlockWorld[(int)(Position.X - 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z - 0.5f)] == WorldBlock.Water2 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water3
                  || _view.BlockWorld[(int)(Position.X - 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z - 0.5f)] == WorldBlock.Water4 )||(

                           _view.BlockWorld[(int)(Position.X ), (int)(Position.Y - 0.05f), (int)(Position.Z + 0.5f)] == WorldBlock.Water1 ||
             _view.BlockWorld[(int)(Position.X), (int)(Position.Y - 0.05f), (int)(Position.Z + 0.5f)] == WorldBlock.Water2 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water3
                 || _view.BlockWorld[(int)(Position.X), (int)(Position.Y - 0.05f), (int)(Position.Z + 0.5f)] == WorldBlock.Water4 )||(

                           _view.BlockWorld[(int)(Position.X), (int)(Position.Y - 0.05f), (int)(Position.Z - 0.5f)] == WorldBlock.Water1 ||
             _view.BlockWorld[(int)(Position.X), (int)(Position.Y - 0.05f), (int)(Position.Z - 0.5f)] == WorldBlock.Water2 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water3
                  || _view.BlockWorld[(int)(Position.X), (int)(Position.Y - 0.05f), (int)(Position.Z - 0.5f)] == WorldBlock.Water4) ||(

                           _view.BlockWorld[(int)(Position.X + 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z)] == WorldBlock.Water1 ||
             _view.BlockWorld[(int)(Position.X + 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z)] == WorldBlock.Water2 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water3
                 || _view.BlockWorld[(int)(Position.X + 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z)] == WorldBlock.Water4 )||(

                           _view.BlockWorld[(int)(Position.X - 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z)] == WorldBlock.Water1 ||
             _view.BlockWorld[(int)(Position.X - 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z )]== WorldBlock.Water2 || _view.BlockWorld[(int)Position.X, (int)(Position.Y - 0.05f), (int)Position.Z] == WorldBlock.Water3
                  || _view.BlockWorld[(int)(Position.X - 0.5f), (int)(Position.Y - 0.05f), (int)(Position.Z )] == WorldBlock.Water4 ) )
                
                  // Wasser unter Block
                {
                    if (Art == 1)
                    {
                        //hier auch 9 Seiten Kollision möglich z.B. new Hitbox(Position.X+0.5f, Position.Y+0.8f, Position.Z+0.5f, 0.001f, 0.001f, 0.001f)
                        Direction _info3 = CollisionDetector.CollisionWithWorld(ref _movement, new Hitbox(Position.X, Position.Y+0.8f, Position.Z, 0.001f, 0.001f, 0.001f), _view.BlockWorld);
                        if (_info3.HasFlag(Direction.Bottom) && _speedY < 0)
                            _speedY = 0;
                    }
                    //Art 2 (schwerer Block) keine Kollision
                }
                else // kein Wasser unter Block
                {
                    
                    Direction _info3 = CollisionDetector.CollisionWithWorld(ref _movement, new Hitbox(Position.X, Position.Y, Position.Z, 1f, 1f, 1f), _view.BlockWorld);
                    if (_info3.HasFlag(Direction.Bottom) && _speedY < 0)
                        _speedY = 0;
                   
                 
                }
                if (Art!=0 && _speedY!=0)
                Position += _movement;

            }

            else if (MaximialDauer < AktuelleDauer && Zustand == (int)ZustandList.Gesetzt)
            {
                Zustand = (int)ZustandList.CD;
                //Zerstöre Objekt
           

            }
            else
            {
                //Objekt ist Tot
                //MaximumL als allgemeines Maximum, müsste addiertes Maximum sein, da ist cd aber zu hoch
                if (MaximumL * MaximialDauer <= AktuelleDauer && Zustand == (int)ZustandList.CD)
                {
                    Zustand = (int)ZustandList.Bereit;
                    AktuelleDauer = 0;
                 

                }
            }
            return (ref GameState _state) =>
            {
               
                //Console.WriteLine(Position);
            };
        }
    }
}
