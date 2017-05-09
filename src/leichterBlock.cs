using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    class LeichterBlock : IUpdateable
    {
        public static int Anzahl=0;
        public static int Maximalanzahl=3;
        public static int Maximum=3;
        public static double MaximialDauer;
        public double AktuelleDauer;
        bool Alive;
        public Vector3 Position;
        public LeichterBlock(Player _player)
        {
   
                Alive = true;
                AktuelleDauer = 0;
                Anzahl++;
                MaximialDauer = 15000;
                Position = _player.Position;
                Position.Y += 0.5f; //Auf Höhe des Spielers
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
            if (MaximialDauer >= AktuelleDauer && Alive==true)
            {
                
                return (ref GameState _state) =>
                {
                    //Update Timer
                    AktuelleDauer += _passedTime;
                    if (_view.GetBlock((int)Position.X, (int)(Position.Y), (int)Position.Z) != WorldBlock.Wall)//TODO: Stapeln von LeichtenBlöcken
                    {
                        Position.Y -= 0.032f;//hier Fallgeschwindigkeit momentan 2 Block pro Sekunde

                    }
                };
            }
            if(MaximialDauer < AktuelleDauer && Alive == true)
            {
                //Zerstöre Objekt
                Anzahl--;
                Alive = false;
                return (ref GameState _state) =>
                {

                };
            }
            else
            {
                //Objekt ist Tot
                return (ref GameState _state) =>
                {

                };
            }
        }
    }
}
