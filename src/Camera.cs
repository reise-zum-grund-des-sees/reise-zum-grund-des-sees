using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
	class Camera
	{
		public Vector3 Position;

		public Matrix CalculateViewMatrix => throw new NotImplementedException();
            // Matrix.CreateLookAt(new Vector3(Position.X, Position.Y, Position.Z), new Vector3(/* TODO: add camera rotation */), Vector3.UnitY);
	}
}