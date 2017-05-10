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
    class PlayerBlock
    {
        public static int AnzahlL = 0;  
        public static int MaximumL = 3;
        public static int AnzahlM = 0;
        public static int MaximumM = 3;
        public static int AnzahlS = 0;
        public static int MaximumS = 3;
        public static double MaximialDauer;
        public double AktuelleDauer;
        public int Art;
        bool Alive;
        public Model Model;
        public Vector3 Position;
        public BoundingBox Box;
        public PlayerBlock(ContentManager contentManager, Player _player,int ArtdesBlocks)
        {

            Alive = true;
            Art = ArtdesBlocks;
            AktuelleDauer = 0;
            MaximialDauer = 15000;
            Position = _player.Position;
            Position.Y += 0.5f; //Auf Höhe des Spielers
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
            List<Vector3> bound = new List<Vector3>();
            bound.Add(Position + new Vector3(0.5f, 0.5f, 0.5f));
            bound.Add(Position + new Vector3(-0.5f, -0.5f, 0.5f));
            bound.Add(Position + new Vector3(0.5f, -0.5f, 0.5f));
            bound.Add(Position + new Vector3(-0.5f, 0.5f, 0.5f));
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
             
                if (_view.GetBlock((int)Position.X, (int)(Position.Y - 0.5f), (int)Position.Z) != WorldBlock.Wall)
                {
                    if (Art != 0) {                  
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
                    int k = 0;
                    for (int i = 0; i < Player.Blöcke.Count; i++)//Kollision beim Fallen untereinander, erlaubt ineinander stecken
                    {
                        if (Box.Intersects(Player.Blöcke[i].Box) == true) k++;    //immer Kollision mit sich selbst              
                    }
                        if (k <= 1)
                        {
                            if (Art == 1)
                                Position.Y -= 0.032f;//hier Fallgeschwindigkeit momentan 2 Block pro Sekunde
                            if (Art == 2)
                                Position.Y -= 0.048f;//hier Fallgeschwindigkeit momentan 3 Block pro Sekunde
                        }

                    }
                 
                  
                }

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
        }
    }
}
