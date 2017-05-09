using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
	class Camera : IUpdateable
	{
		public Vector3 Position;
        public Vector3 LookAt;
        public Vector2 Rotation;
        int Bewegungssensivitaet=10;
        float Offset;
        //public Matrix CalculateViewMatrix => throw new NotImplementedException();
        // Matrix.CreateLookAt(new Vector3(Position.X, Position.Y, Position.Z), new Vector3(/* TODO: add camera rotation */), Vector3.UnitY);
        public Camera(bool fullscreen)
        {
           
            Rotation = new Vector2(0,0);
            if (fullscreen == true)
                Offset = -2.875f;
            else
                Offset = 0;
        }
        public Matrix CalculateViewMatrix()
        {
                return Matrix.CreateLookAt(Position,LookAt, Vector3.UnitY);
          
        }
        public UpdateDelegate Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime)
		{
            //throw new NotImplementedException();
            return (ref GameState _state) =>
            {
                Position = new Vector3(_view.PlayerX, _view.PlayerY + 5, _view.PlayerZ - 5);
                //LookAt = new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ + 2); //altes LookAt
                //Rotation.X Startpunkt=400, Rotation.Y Startpunkt = 240     
                if (Rotation.X >= 0- _inputArgs.MouseMovementRelative.X && Rotation.X <= 1 - _inputArgs.MouseMovementRelative.X)
                    Rotation.X += _inputArgs.MouseMovementRelative.X;
                if (Rotation.Y >= 0 - _inputArgs.MouseMovementRelative.Y && Rotation.Y <= 1 - _inputArgs.MouseMovementRelative.Y)
                    Rotation.Y += _inputArgs.MouseMovementRelative.Y;

                // Doesn't work for larger Y-Values!!!
                //LookAt = new Vector3(_view.PlayerX - (Rotation.X-0.5f) * Bewegungssensivitaet + Offset , 0, _view.PlayerZ - (Rotation.Y - 0.5f) * Bewegungssensivitaet + 5);
                LookAt = new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ);
        

            };
        }
	}
}