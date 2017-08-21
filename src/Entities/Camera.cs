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
    interface ICamera : IReadonlyCamera, IUpdateable
    {
        new IReadonlyPositionObject Center { get; set; }
    }

    interface IReadonlyCamera : IReadonlyPositionObject
    {
        IReadonlyPositionObject Center { get; }
        Matrix CalculateViewMatrix();
        float Azimuth { get; }
    }

	class Camera : ICamera
	{
        public IReadonlyPositionObject Center { get; set; }
		public Vector3 Position { get; private set; }
        public Vector3 LookAt;
        public float Azimuth { get; private set; }
        public float Altitude { get; private set; }

        public Camera()
        {
        }

        public Matrix CalculateViewMatrix()
        {
            if (Center != null)
                LookAt = Center.Position;
            Vector2 _diffToPlayer = new Vector2(5, 0);
            Vector3 _position =
                new Vector3(
                    LookAt.X - (float)Math.Sin(Azimuth) * (float)Math.Cos(Altitude) * _diffToPlayer.X,
                    LookAt.Y + (float)Math.Sin(Altitude) * _diffToPlayer.X,
                    LookAt.Z + (float)Math.Cos(Azimuth) * (float)Math.Cos(Altitude) * _diffToPlayer.X);
            return Matrix.CreateLookAt(_position, LookAt + new Vector3(0, 1, 0), Vector3.UnitY);
          
        }

        public void CenterOn(IReadonlyPositionObject _centerObject)
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
                    Azimuth += _inputArgs.MouseMovementRelative.X * 10f;
                    if (Azimuth > MathHelper.TwoPi)
                        Azimuth -= MathHelper.TwoPi;
                    if (Azimuth < -MathHelper.TwoPi)
                        Azimuth += MathHelper.TwoPi;

                    Altitude += _inputArgs.MouseMovementRelative.Y * 10f;
                    if (Altitude > MathHelper.PiOver2 * 0.95f)
                        Altitude = MathHelper.PiOver2 * 0.95f;
                    if (Altitude < 0)//-MathHelper.PiOver2)
                        Altitude = 0;//-MathHelper.PiOver2;

                    if (Center != null)
                        LookAt = Center.Position;
                }
            };
        }
	}
}