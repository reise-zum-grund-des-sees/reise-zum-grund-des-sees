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
		public float xPos, yPos, zPos;

        public Matrix CalculateViewMatrix(Player player) {
            Matrix ViewMatrix = Matrix.CreateLookAt(new Vector3(player.pos.X, player.pos.Y + 15, player.pos.Z - 10), new Vector3(player.pos.X, player.pos.Y, player.pos.Z + 2), Vector3.UnitY);
            return ViewMatrix;
	}
}