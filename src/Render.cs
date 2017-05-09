using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    static class Render
    {
        public static void World(Matrix m, World _world, BasicEffect _effect, GraphicsDevice _device)
        {
            _effect.VertexColorEnabled = true;

            int maxX = _world.Vertices.GetLength(0);
            int maxZ = _world.Vertices.GetLength(1);

            for (int x = 0; x < maxX; x++)
                for (int z = 0; z < maxZ; z++)
                {
                    if (_world.Vertices[x, z].Length != 0)
                    {
                        _effect.View = m;
                        _effect.World = Matrix.CreateTranslation(x * _world.RegionSizeX, 0, z * _world.RegionSizeZ);
                        _effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 1f, 50f);

                        RasterizerState rasterizerState = new RasterizerState();
                        rasterizerState.CullMode = CullMode.None;
                        _device.RasterizerState = rasterizerState;

                        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                            pass.Apply();

                        _device.SetVertexBuffer(_world.VertexBuffers[x, z]);
                        _device.DrawPrimitives(PrimitiveType.TriangleList, 0, _world.Vertices[x, z].Length / 3);
                    }
                }
        }

        public static void Player(Matrix m, Player _player)
        {
            //throw new NotImplementedException();

            foreach (ModelMesh mesh in _player.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateTranslation(_player.Position);

                    effect.View = m;

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 1f, 50f);

                }

                mesh.Draw();
            }

        }

        // ...
    }
}
