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
        public Model model;
        bool Jump1;
        bool Jump2;
        double CurrentJumpTime;

        public Player(ContentManager contentManager, Vector3 _position)
        {
            Position = _position;
            Jump1 = false;
            Jump2 = false;
            CurrentJumpTime = 0;
            model = contentManager.Load<Model>("spielfigur");
        }
       
        public UpdateDelegate Update(GameState.View _stateView, InputEventArgs _inputArgs, double _passedTime)
		{
            // Nicht die Variablen hier ändern. Aber Kollisionserkennung hier berechnen.

            int[] Kollision = new int[8];//für jede Mögliche Richtung Kollsion
            float hitbox=0.2f;
            // Wenn Kolision
            //Verhältnis vorher berechnen, momentane Blockgröße 1x1x1
            if (_stateView.GetBlock((int)(_stateView.PlayerX + hitbox), (int)(_stateView.PlayerY+0.05f), (int)_stateView.PlayerZ) == WorldBlock.Wall)
            {
                Kollision[0] = 1;
            }
            if (_stateView.GetBlock((int)(_stateView.PlayerX - hitbox), (int)(_stateView.PlayerY + 0.05f), (int)_stateView.PlayerZ) == WorldBlock.Wall)
            {
                Kollision[4] = 1;
            }
            if (_stateView.GetBlock((int)(_stateView.PlayerX ), (int)(_stateView.PlayerY + 0.05f), (int)(_stateView.PlayerZ-hitbox)) == WorldBlock.Wall)
            {
                Kollision[2] = 1;
            }
            if (_stateView.GetBlock((int)(_stateView.PlayerX), (int)(_stateView.PlayerY + 0.05f), (int)(_stateView.PlayerZ+hitbox)) == WorldBlock.Wall)
            {
                Kollision[6] = 1;
            }
       

            //ist unter dem Spieler ein Block? -> Wenn nein, falle nach unten, wenn er nicht gerade im Sprung ist
            if (_stateView.GetBlock((int)_stateView.PlayerX, (int)(_stateView.PlayerY), (int)_stateView.PlayerZ) != WorldBlock.Wall)
            {
                Position.Y -= 0.032f;//hier Fallgeschwindigkeit momentan 2 Block pro Sekunde
               
            }
            if(_stateView.GetBlock((int)_stateView.PlayerX, (int)_stateView.PlayerY, (int)_stateView.PlayerZ) == WorldBlock.Wall) {
               Jump1 = false; // setze Sprung zurück
                  Jump2 = false; // setze Sprung zurück
                CurrentJumpTime = 0;
            }
          
            return (ref GameState _state) =>
			{
                // Hier Variablen ändern - direkt, oder über _state.Player ...
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
                      
                        Position.Y += 0.082f;//Sprunghöhe 2
                    }
                    
                    if (_inputArgs.Events.HasFlag(InputEventList.Jump) && Jump2 == false && CurrentJumpTime > 300)//Doppelsprung
                    {
                        Console.WriteLine(CurrentJumpTime);
                        Jump2 = true;
                        CurrentJumpTime = 0;
                    }
                    
                }
              
                    float sprint=1;
                    if (_inputArgs.Events.HasFlag(InputEventList.Sprint)) sprint = 2;//Sprintgeschwindigkeit
                    
                    if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards) && Kollision[0]==0)
                    {
                        Position.Z += 0.016f*sprint;
                    }

                    if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft) && Kollision[6] == 0)
                    {
                        Position.X += 0.016f * sprint;
                    }

                    if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards) && Kollision[4] == 0)
                    {
                        Position.Z -= 0.016f * sprint;
                    }

                    if (_inputArgs.Events.HasFlag(InputEventList.MoveRight) && Kollision[2] == 0)
                    {
                        Position.X -= 0.016f * sprint;
                    }
                
			};
        }

    }
}
