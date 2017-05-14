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
        bool Alive;
        public Model Model;
        public Vector3 Position;

        public PlayerBlock(ContentManager contentManager, Player _player,int ArtdesBlocks)
        {

            Alive = true;
            Art = ArtdesBlocks;
            _speedY = 0;
            _movement = new Vector3(0, 0, 0);
            AktuelleDauer = 0;
            MaximialDauer = 15000;
            Position = _player.Position;        
            if (Art == 0){//leichterBlock
                AnzahlL++;
                Model = contentManager.Load<Model>("Block");
                   
            }
            if (Art == 1)//MittelschwererBlock
            {
                AnzahlM++;
                Model = contentManager.Load<Model>("Block");
          
            }
            if (Art == 2)//SchwererBlock
            {
                AnzahlS++;             
                Model = contentManager.Load<Model>("Block");
          
            }
            
           
            //Position des Blockes basierend auf Blickrichtung
            switch (_player.Blickrichtung)
            {
                case 0:
                    Position.Z += 1;
                    break;
                case 1:
                    Position.Z += 1;
                    Position.X += 1;
                    break;
                case 2:
                    Position.X += 1;
                    break;
                case 3:
                    Position.Z -= 1;
                    Position.X += 1;
                    break;
                case 4:
                    Position.Z -= 1;
                    break;
                case 5:
                    Position.Z -= 1;
                    Position.X -= 1;
                    break;
                case 6:
                    Position.X -= 1;
                    break;
                default:
                    Position.Z += 1;
                    Position.X -= 1;
                    break;
            }
      
        }

        public UpdateDelegate Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime)
        {
            if (MaximialDauer >= AktuelleDauer && Alive == true)
            {


                //Update Timer
                AktuelleDauer += _passedTime;
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
                List<Direction> _info2 = new List<Direction>();
                for (int i = 0; i < Player.Blöcke.Count; i++)
                    if (Vector3.Distance(Position, Player.Blöcke[i].Position) != 0)
                    {
                        _info2.Add(CollisionDetector.CollisionWithObject(ref _movement, new Hitbox(Position.X, Position.Y, Position.Z, 0.8f, 0.8f, 1f), new Hitbox(Player.Blöcke[i].Position.X, Player.Blöcke[i].Position.Y, Player.Blöcke[i].Position.Z, 0.8f, 0.8f, 1f)));                        
                    }
                for (int i = 0; i < Player.Blöcke.Count-1; i++)
                    if (_info2[i].HasFlag(Direction.Bottom) && _speedY < 0)
                    _speedY = 0;

                Direction _info3 = CollisionDetector.CollisionWithWorld(ref _movement, new Hitbox(Position.X, Position.Y, Position.Z , 0.5f, 0.5f, 1f), _view);
                    if (_info3.HasFlag(Direction.Bottom) && _speedY < 0)
                        _speedY = 0;

                if (Art!=0 && _speedY!=0)
                Position += _movement;

            }

            if (MaximialDauer < AktuelleDauer && Alive == true)
            {
                Alive = false;
                //Zerstöre Objekt
                if (Art == 0)
                {//leichterBlock
                    AnzahlL--;
                
                }
                if (Art == 1)//MittelschwererBlock
                {
                    AnzahlM--;
                
                }
                if (Art == 2)//SchwererBlock
                {
                    AnzahlS--;
                  
                }


            }
            else
            {
                //Objekt ist Tot

            }
            return (ref GameState _state) =>
            {
               
                //Console.WriteLine(Position);
            };
        }
    }
}
