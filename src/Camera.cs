﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ReiseZumGrundDesSees
{
	class Camera : IUpdateable
	{
		public Vector3 Position;
        public Vector3 LookAt;
        public Vector3 TargetToCam;
        public float Angle;
        public float Sumofangle;
        float Intensität=3f;
        public static bool is_running=false;
        //public Matrix CalculateViewMatrix => throw new NotImplementedException();
        // Matrix.CreateLookAt(new Vector3(Position.X, Position.Y, Position.Z), new Vector3(/* TODO: add camera rotation */), Vector3.UnitY);
        public Camera()
        {

        }
        public Matrix CalculateViewMatrix()
        {
            Vector2 _diffToPlayer = new Vector2(7, 10);
            Vector3 _position =
                new Vector3(LookAt.X - (float)Math.Sin(Angle) * _diffToPlayer.Y,
                LookAt.Y + _diffToPlayer.X,
                LookAt.Z + (float)Math.Cos(Angle) * _diffToPlayer.Y);
            return Matrix.CreateLookAt(_position, LookAt + new Vector3(0, 2, 0), Vector3.UnitY);
          
        }

        public void ChangePosition(Vector3 _movement)
        {
            Position += _movement;
        }

        public UpdateDelegate Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime)
		{
            //throw new NotImplementedException();
            return (ref GameState _state) =>
            {
                Angle += _inputArgs.MouseMovementRelative.X * 10f;
                LookAt = new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ);
              
             
                /*if (Position.Equals(new Vector3(0, 0, 0))) { 
                Position = new Vector3(_view.PlayerX, _view.PlayerY+5, _view.PlayerZ+5);
                    is_running = true; //nur Mousepossition zur Mittel wenn Camera erstellt
                }
                angle = _inputArgs.MouseMovementRelative.X * Intensität;
                Sumofangle += _inputArgs.MouseMovementRelative.X * Intensität;
                
                // Get our current target to camera vector (this is opposite of view from above)     
                TargetToCam = Vector3.Subtract(Position, new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ));

                // Calculate our rotation matrix     
                Matrix rotation = Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), angle);
                
                // rotate the TargetToCam     
                TargetToCam = Vector3.Transform(TargetToCam, rotation);

                // add our rotated TargetToCam to the target position to get the new camera position     
                Position = Vector3.Add(new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ), TargetToCam);
              
                LookAt = new Vector3(_view.PlayerX, _view.PlayerY + 2, _view.PlayerZ);*/

            };
        }
	}
}