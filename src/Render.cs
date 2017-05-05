using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
	static class Render
	{
		public static void World(Matrix m, World _world)
		{
			throw new NotImplementedException();
		}

		public static void Player(Matrix m, Player _player)
		{
			throw new NotImplementedException();
			/*
            foreach (ModelMesh mesh in _player.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    effect.World = Matrix.CreateTranslation(_player.pos);

                    effect.View = m;

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 5000f);

                }

                mesh.Draw();
            }
			*/
		}

		// ...
	}
}
