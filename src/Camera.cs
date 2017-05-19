using System;
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
        public float Angle;
        public static bool is_running=false; //wenn True wird Camera Mittig fixiert

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


            };
        }
	}
}