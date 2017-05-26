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
        public IPositionObject Center { get; set; }
		public Vector3 Position;
        public Vector3 LookAt;
        public float Angle;

        public Camera()
        {
               
        }
        public Matrix CalculateViewMatrix()
        {
            if (Center != null)
                LookAt = Center.Position;
            Vector2 _diffToPlayer = new Vector2(7, 10);
            Vector3 _position =
                new Vector3(LookAt.X - (float)Math.Sin(Angle) * _diffToPlayer.Y,
                LookAt.Y + _diffToPlayer.X,
                LookAt.Z + (float)Math.Cos(Angle) * _diffToPlayer.Y);
            return Matrix.CreateLookAt(_position, LookAt + new Vector3(0, 2, 0), Vector3.UnitY);
          
        }

        public void CenterOn(IPositionObject _centerObject)
        {
            Center = _centerObject;
        }

        public void ChangePosition(Vector3 _movement)
        {
            Position += _movement;
        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
		{
            return (ref GameState _state) =>
            {
                if (_flags.HasFlag(GameFlags.GameRunning))
                {
                    Angle += _inputArgs.MouseMovementRelative.X * 10f;
                    if (Center != null)
                        LookAt = Center.Position;
                }
            };
        }
	}
}