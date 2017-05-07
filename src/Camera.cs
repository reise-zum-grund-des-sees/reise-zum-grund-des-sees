using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
	class Camera : IUpdateable
	{
		public Vector3 Position;
        public Vector3 LookAt; 
        //public Matrix CalculateViewMatrix => throw new NotImplementedException();
        // Matrix.CreateLookAt(new Vector3(Position.X, Position.Y, Position.Z), new Vector3(/* TODO: add camera rotation */), Vector3.UnitY);
        public Camera()
        {
            // braucht erstmal keine Initialisssierung
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
                LookAt = new Vector3(_view.PlayerX, _view.PlayerY, _view.PlayerZ + 2);
                 //LookAt = new Vector3(0,0,0);
            };
        }
	}
}