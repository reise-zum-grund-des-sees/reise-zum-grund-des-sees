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
    class LeichterBlock
    {
        public static int Anzahl = 0;
        public static int Maximalanzahl = 3;
        public static int Maximum = 3;
        public static double MaximialDauer;
        public double AktuelleDauer;
        bool Alive;
        public Model Model;
        public Vector3 Position;
        public BoundingBox Box;
        public LeichterBlock(ContentManager contentManager, Player _player)
        {

            Alive = true;
            AktuelleDauer = 0;
            Anzahl++;
            MaximialDauer = 15000;
            Position = _player.Position;
            Position.Y += 0.5f; //Auf Höhe des Spielers
            Model = contentManager.Load<Model>("Block");
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
            List<Vector3> bound = new List<Vector3>();
            bound.Add(Position + new Vector3(0.5f, 0.5f, 0.5f));
            bound.Add(Position + new Vector3(-0.5f, -0.5f, 0.5f));
            bound.Add(Position + new Vector3(0.5f, -0.5f, 0.5f));
            bound.Add(Position + new Vector3(-0.5f, 0.5f, 0.5f));
            bound.Add(Position + new Vector3(0.5f, 0.5f, 0.5f));
            bound.Add(Position + new Vector3(0.5f, 0.5f, -0.5f));
            bound.Add(Position + new Vector3(-0.5f, -0.5f, -0.5f));
            bound.Add(Position + new Vector3(0.5f, -0.5f, -0.5f));
            bound.Add(Position + new Vector3(-0.5f, 0.5f, -0.5f));
            Box = BoundingBox.CreateFromPoints(bound);


        }

        public void Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime)
        {
            if (MaximialDauer >= AktuelleDauer && Alive == true)
            {


                //Update Timer
                AktuelleDauer += _passedTime;
                if (_view.GetBlock((int)Position.X, (int)(Position.Y - 0.5f), (int)Position.Z) != WorldBlock.Wall)//TODO: Stapeln von Leichten Blöcken
                {
                    Position.Y -= 0.032f;//hier Fallgeschwindigkeit momentan 2 Block pro Sekunde
                    List<Vector3> bound = new List<Vector3>();
                    bound.Add(Position + new Vector3(0.5f, 0.5f, 0.5f));
                    bound.Add(Position + new Vector3(-0.5f, -0.5f, 0.5f));
                    bound.Add(Position + new Vector3(0.5f, -0.5f, 0.5f));
                    bound.Add(Position + new Vector3(-0.5f, 0.5f, 0.5f));
                    bound.Add(Position + new Vector3(0.5f, 0.5f, 0.5f));
                    bound.Add(Position + new Vector3(0.5f, 0.5f, -0.5f));
                    bound.Add(Position + new Vector3(-0.5f, -0.5f, -0.5f));
                    bound.Add(Position + new Vector3(0.5f, -0.5f, -0.5f));
                    bound.Add(Position + new Vector3(-0.5f, 0.5f, -0.5f));
                    Box = BoundingBox.CreateFromPoints(bound);
                }

            }
            if (MaximialDauer < AktuelleDauer && Alive == true)
            {
                //Zerstöre Objekt
                Anzahl--;
                Alive = false;

            }
            else
            {
                //Objekt ist Tot

            }
        }
    }
}
