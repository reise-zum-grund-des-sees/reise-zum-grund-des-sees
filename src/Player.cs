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
        double CurrentJumpTime;
        double BlickTime;
        double Blockcd;
        public int Blickrichtung;
        public static List<PlayerBlock> Blöcke;
        public BoundingBox Box;
        ContentManager ContentManager;
        float FallOffset = 0.5f;//wann fällt man von Blöcken runter, soll zwischen 0.5f-1f liegen, je höher desto mehr Probleme treten bei Mapblöcken auf
        //>0.9 auch Probleme bei gesetzten Blöcken
        public Player(ContentManager contentManager, Vector3 _position)
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

        public UpdateDelegate Update(GameState.View _stateView, InputEventArgs _inputArgs, double _passedTime)
        {
            // Nicht die Variablen hier ändern. Aber Kollisionserkennung hier berechnen.

            int[] Kollision = new int[4];//für jede Mögliche Richtung Kollsion
            float hitbox = 0.51f;

            float sprint = 1;
            if (_inputArgs.Events.HasFlag(InputEventList.Sprint)) sprint = 2;//Sprintgeschwindigkeit

            //Blickrichtung   
            BlickTime += _passedTime; //Um Blöcke in 8 Richtungen setzen zu können
            if (_inputArgs.Events.HasFlag(InputEventList.MoveRight) && BlickTime > 100) //hier die Zeit zwischen seitliche Inputs
                Blickrichtung = 6;
            if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft) && BlickTime > 100)
                Blickrichtung = 2;
            if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards) && BlickTime > 100)
                Blickrichtung = 0;
            if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards) && BlickTime > 100)
                Blickrichtung = 4;

            if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards) && _inputArgs.Events.HasFlag(InputEventList.MoveRight))
            {
                Blickrichtung = 7;
                BlickTime = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards) && _inputArgs.Events.HasFlag(InputEventList.MoveLeft))
            {
                Blickrichtung = 3;
                BlickTime = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards) && _inputArgs.Events.HasFlag(InputEventList.MoveLeft))
            {
                Blickrichtung = 1;
                BlickTime = 0;
            }
            if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards) && _inputArgs.Events.HasFlag(InputEventList.MoveRight))
            {
                Blickrichtung = 5;
                BlickTime = 0;
            }
            //Kollision 4 Richtungen
            if (_stateView.GetBlock((int)(_stateView.PlayerX), (int)(_stateView.PlayerY + 0.05f), (int)(_stateView.PlayerZ + hitbox)) == WorldBlock.Wall)
            {
                Kollision[0] = 1;//vorne
            }
            if (_stateView.GetBlock((int)(_stateView.PlayerX), (int)(_stateView.PlayerY + 0.05f), (int)(_stateView.PlayerZ - hitbox)) == WorldBlock.Wall)
            {
                Kollision[2] = 1;//hinten
            }
            if (_stateView.GetBlock((int)(_stateView.PlayerX - hitbox), (int)(_stateView.PlayerY + 0.05f), (int)(_stateView.PlayerZ)) == WorldBlock.Wall)
            {
                Kollision[1] = 1;//Rechts
            }
            if (_stateView.GetBlock((int)(_stateView.PlayerX + hitbox), (int)(_stateView.PlayerY + 0.05f), (int)(_stateView.PlayerZ)) == WorldBlock.Wall)
            {
                Kollision[3] = 1;//links
            }
            //4 Seiten Kollision mit gesetzen Blöcken
            List<Vector3> bound = new List<Vector3>();
            bound.Add(Position + new Vector3(0.5f, 1.5f, 0.5f));
            bound.Add(Position + new Vector3(-0.5f, 1.5f, -0.5f));
            bound.Add(Position + new Vector3(0.5f, 1.5f, -0.5f));
            bound.Add(Position + new Vector3(-0.5f, 1.5f, 0.5f));
            bound.Add(Position + new Vector3(0.5f, 0.05f, 0.5f));
            bound.Add(Position + new Vector3(-0.5f, 0.05f, -0.5f));
            bound.Add(Position + new Vector3(0.5f, 0.05f, -0.5f));
            bound.Add(Position + new Vector3(-0.5f, 0.05f, 0.5f));
            Box = BoundingBox.CreateFromPoints(bound);

            for (int i = 0; i < Blöcke.Count; i++)
            {
                if (Box.Intersects(Blöcke.ElementAt(i).Box) && Kollision[3] == 0 && Blöcke.ElementAt(i).Position.X - Position.X > 0.5f && _inputArgs.Events.HasFlag(InputEventList.MoveLeft)) Kollision[3] = 1;//links von Spieler ist Wand
                if (Box.Intersects(Blöcke.ElementAt(i).Box) && Kollision[1] == 0 && Blöcke.ElementAt(i).Position.X - Position.X < -0.5f && _inputArgs.Events.HasFlag(InputEventList.MoveRight)) Kollision[1] = 1;//rechts
                if (Box.Intersects(Blöcke.ElementAt(i).Box) && Kollision[2] == 0 && Blöcke.ElementAt(i).Position.Z - Position.Z < -0.5f && _inputArgs.Events.HasFlag(InputEventList.MoveBackwards)) Kollision[2] = 1;//hinten
                if (Box.Intersects(Blöcke.ElementAt(i).Box) && Kollision[0] == 0 && Blöcke.ElementAt(i).Position.Z - Position.Z > 0.5f && _inputArgs.Events.HasFlag(InputEventList.MoveForwards)) Kollision[0] = 1;//vorne

            }


            return (ref GameState _state) =>
            {
                // Hier Variablen ändern - direkt, oder über _state.Player ...
                //ist unter dem Spieler ein Block? -> Wenn nein, falle nach unten, wenn er nicht gerade im Sprung ist
                bool j = false;
                //PROBLEMSTELLE
                if (_stateView.GetBlock((int)Position.X, (int)(Position.Y), (int)Position.Z) == WorldBlock.Wall ||
                _stateView.GetBlock((int)(Position.X + (FallOffset - 0.5f)), (int)(Position.Z), (int)_stateView.PlayerZ) == WorldBlock.Wall ||
                _stateView.GetBlock((int)(Position.X - (FallOffset - 0.5f)), (int)(Position.Z), (int)_stateView.PlayerZ) == WorldBlock.Wall ||
                _stateView.GetBlock((int)(Position.X), (int)(Position.Y), (int)(Position.Z + (FallOffset - 0.5f))) == WorldBlock.Wall ||
                _stateView.GetBlock((int)(Position.X), (int)(Position.Y), (int)(Position.Z - (FallOffset - 0.5f))) == WorldBlock.Wall
                )
                {
                    //Im Block steckend halbwechs funktionierend
  // if(Math.Abs(Position.Y- (int)(Position.Y) ) < 0.55f) Position.Y -= 0.032f;
                }
                else
                {


                    for (int i = 0; i < Blöcke.Count; i++)
                    {
                        if (Position.Y - Blöcke.ElementAt(i).Position.Y < 0.5f && Position.Y - Blöcke.ElementAt(i).Position.Y >0f
                        && Math.Abs(Blöcke.ElementAt(i).Position.X - Position.X) < FallOffset && Math.Abs(Blöcke.ElementAt(i).Position.Z - Position.Z) < FallOffset)
                            j = true;
                    }
                    if (j == true) { }
                    else

                        Position.Y -= 0.032f;//hier Fallgeschwindigkeit momentan 2 Block pro Sekunde
                }
                //gleiche PROBLEMSTELLE
                if (_stateView.GetBlock((int)Position.X, (int)(Position.Y), (int)Position.Z) == WorldBlock.Wall ||
                _stateView.GetBlock((int)(Position.X + (FallOffset - 0.5f)), (int)(Position.Z), (int)_stateView.PlayerZ) == WorldBlock.Wall ||
                _stateView.GetBlock((int)(Position.X - (FallOffset - 0.5f)), (int)(Position.Z), (int)_stateView.PlayerZ) == WorldBlock.Wall ||
                _stateView.GetBlock((int)(Position.X), (int)(Position.Y), (int)(Position.Z + (FallOffset - 0.5f))) == WorldBlock.Wall ||
                _stateView.GetBlock((int)(Position.X), (int)(Position.Y), (int)(Position.Z - (FallOffset - 0.5f))) == WorldBlock.Wall ||
               j ==true
               )
                {
                    Jump1 = false; // setze Sprung zurück
                    Jump2 = false; // setze Sprung zurück
                    CurrentJumpTime = 0;
                }



                if (_inputArgs.Events.HasFlag(InputEventList.Jump) && Jump1 == false)
                {
                    Jump1 = true;
                    CurrentJumpTime = 0;

                }

                if (Jump1 == true)
                {

                    CurrentJumpTime += _passedTime;

                    if (CurrentJumpTime < 500) //Zeit, wann nach Sprung 1, Sprung 2 bereit ist
                    {
                        bool k = false;
                        for (int i = 0; i < Blöcke.Count; i++)
                        if ( Blöcke.ElementAt(i).Position.Y- Position.Y  < 2f && Blöcke.ElementAt(i).Position.Y - Position.Y>0.1f
                       && Math.Abs(Blöcke.ElementAt(i).Position.X - Position.X) < FallOffset && Math.Abs(Blöcke.ElementAt(i).Position.Z - Position.Z) < FallOffset) k = true;

                        if (_stateView.GetBlock((int)_stateView.PlayerX, (int)(_stateView.PlayerY + 1.5f), (int)_stateView.PlayerZ) != WorldBlock.Wall && k == false)//über SPieler ein Block?

                            Position.Y += 0.082f;//Sprunghöhe 2
                        else
                        CurrentJumpTime = 500;
                        

                          
                    }

                    if (_inputArgs.Events.HasFlag(InputEventList.Jump) && Jump2 == false && CurrentJumpTime > 300)//Doppelsprung
                    {
                        //Doppelsprung setzt erneuten Sprung ein, neue Höhe ist Einsatzhöhe+Sprung
                        Jump2 = true;
                        CurrentJumpTime = 0;
                    }

                }



                if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards) && Kollision[0] == 0)
                {
                    Position.Z += 0.016f * sprint;
                }

                if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft) && Kollision[3] == 0)
                {
                    Position.X += 0.016f * sprint;
                }

                if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards) && Kollision[2] == 0)
                {
                    Position.Z -= 0.016f * sprint;
                }

                if (_inputArgs.Events.HasFlag(InputEventList.MoveRight) && Kollision[1] == 0)
                {
                    Position.X -= 0.016f * sprint;
                }
                Blockcd += _passedTime;
                if (_inputArgs.Events.HasFlag(InputEventList.LeichterBlock) && PlayerBlock.MaximumL > PlayerBlock.AnzahlL && Blockcd > 1000)
                {
                    Blöcke.Add(new PlayerBlock(ContentManager, this,0));
                    Blockcd = 0;
                }
                if (_inputArgs.Events.HasFlag(InputEventList.MittelschwererBlock) && PlayerBlock.MaximumM > PlayerBlock.AnzahlM && Blockcd > 1000)
                {
                    Blöcke.Add(new PlayerBlock(ContentManager, this, 1));
                    Blockcd = 0;
                }
                if (_inputArgs.Events.HasFlag(InputEventList.SchwererBlock) && PlayerBlock.MaximumS > PlayerBlock.AnzahlS && Blockcd > 1000)
                {
                    Blöcke.Add(new PlayerBlock(ContentManager, this, 2));
                    Blockcd = 0;
                }
                for (int i = 0; i < Blöcke.Count; i++)
                {
                    if (Blöcke.ElementAt(i).AktuelleDauer > PlayerBlock.MaximialDauer)
                        Blöcke.RemoveAt(i);
                }
                Console.WriteLine(Position);
            };
        }

    }
}
