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
        public Vector2 Offset;
        int Bewegungssensivitaet=120;
        int FensterBreite;
        int FensterHoehe;
        //public Matrix CalculateViewMatrix => throw new NotImplementedException();
        // Matrix.CreateLookAt(new Vector3(Position.X, Position.Y, Position.Z), new Vector3(/* TODO: add camera rotation */), Vector3.UnitY);
        public Camera(int Width, int Height)
        {
            // evtl. auch Höhe übergeben
            Rotation = new Vector2(0,0);
            FensterBreite = Width;
            FensterHoehe = Height;
            Offset = new Vector2(-Width/2 / Bewegungssensivitaet - 0.3f, 0);
          
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
                Position = new Vector3(_view.PlayerX, _view.PlayerY + 10, _view.PlayerZ - 10);
                //LookAt = new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ + 2); altes LookAt
                //Rotation.X Startpunkt=400, Rotation.Y Startpunkt = 240
                if(Rotation.X >= -100 - _inputArgs.MouseMovement.X &&Rotation.X<=1000-_inputArgs.MouseMovement.X)
                    Rotation.X += _inputArgs.MouseMovement.X;
                if (Rotation.Y >= 0 - _inputArgs.MouseMovement.Y && Rotation.Y <= 620- _inputArgs.MouseMovement.Y)
                    Rotation.Y += _inputArgs.MouseMovement.Y;
               
                LookAt = new Vector3(_view.PlayerX+Rotation.X/ Bewegungssensivitaet + Offset.X,0, _view.PlayerZ + Rotation.Y/ Bewegungssensivitaet + 2+Offset.Y);
                
                
            };
        }
	}
}