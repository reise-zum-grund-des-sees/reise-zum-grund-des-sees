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
            model = contentManager.Load<Model>("Block");
        }
       
        public UpdateDelegate Update(GameState.View _stateView, InputEventArgs _inputArgs, double _passedTime)
		{
            // Nicht die Variablen hier ändern. Aber Kollisionserkennung hier berechnen.
            
            bool Kollision = false;
            /*
             // Wenn Kolision
             //Verhältnis vorher berechnen, momentane Blockgröße 1x1x1
			if(_stateView.GetBlock((int)_stateView.PlayerX, (int)_stateView.PlayerY, (int)_stateView.PlayerZ)==WorldBlock.Wall)
            {
                Kollision = true;
            }
           
            //ist unter dem Spieler ein Block? -> Wenn nein, falle nach unten, wenn er nicht gerade im Sprung ist
            if (_stateView.GetBlock((int)_stateView.PlayerX, (int)_stateView.PlayerY-1, (int)_stateView.PlayerZ) == WorldBlock.Wall && Jump1==false)
            {
                Position.Y -= 0.016f;//hier Fallgeschwindigkeit momentan 1 Block pro Sekunde
               
            }
            if(_stateView.GetBlock((int)_stateView.PlayerX, (int)_stateView.PlayerY-1, (int)_stateView.PlayerZ) == WorldBlock.Wall {
               Jump1 = false; // setze Sprung zurück
                  Jump2 = false; // setze Sprung zurück
            }
             */
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
                    if (CurrentJumpTime < 500)
                    {
                      
                        Position.Y += 0.066f;//Sprunghöhe 2
                    }
                 if(_inputArgs.Events.HasFlag(InputEventList.Jump) && Jump2 == false && CurrentJumpTime > 100)//Doppelsprung
                    {
                        Jump2 = true;
                        CurrentJumpTime = 0;
                    }
                    
                }
                if (Kollision == false)
                {
                    if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards))
                    {
                        Position.Z += 0.016f;
                    }

                    if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft))
                    {
                        Position.X += 0.016f;
                    }

                    if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards))
                    {
                        Position.Z -= 0.016f;
                    }

                    if (_inputArgs.Events.HasFlag(InputEventList.MoveRight))
                    {
                        Position.X -= 0.016f;
                    }
                }
			};
        }

    }
}
