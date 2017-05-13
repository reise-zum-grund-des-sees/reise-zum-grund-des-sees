﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    class Render
    {
        private readonly GraphicsDevice graphicsDevice;

        private readonly BasicEffect worldEffect;
        private readonly RasterizerState CounterClockwiseCull, NoCullMode;

        private readonly Model worldEditorCursor;

        public Render(GraphicsDevice _graphicsDevice, ContentManager _content)
        {
            graphicsDevice = _graphicsDevice;
            Texture2D blocktexture = _content.Load<Texture2D>("blocktexture");
            worldEditorCursor = _content.Load<Model>("cursor");

            CounterClockwiseCull = new RasterizerState();
            CounterClockwiseCull.CullMode = CullMode.CullCounterClockwiseFace;

            NoCullMode = new RasterizerState();
            NoCullMode.CullMode = CullMode.None;

            worldEffect = new BasicEffect(graphicsDevice);
            worldEffect.VertexColorEnabled = true;
            worldEffect.TextureEnabled = true;
            worldEffect.Texture = blocktexture;
        }

        public void WorldEditor(WorldEditor _editor, ref Matrix _viewMatrix, ref Matrix _perspectiveMatrix)
        {
            graphicsDevice.RasterizerState = NoCullMode;
            foreach (ModelMesh mesh in worldEditorCursor.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    Vector3 _blockPosition = new Vector3((int)_editor.Position.X + 0.5f, (int)_editor.Position.Y + 0.5f, (int)_editor.Position.Z + 0.5f);
                    effect.World = Matrix.CreateTranslation(_blockPosition);

                    effect.View = _viewMatrix;
                    effect.Projection = _perspectiveMatrix;

                }

                mesh.Draw();
            }
        }

        public void World(World _world, ref Matrix _viewMatrix, ref Matrix _perspectiveMatrix)
        {
            int maxX = _world.Vertices.GetLength(0);
            int maxZ = _world.Vertices.GetLength(1);

            for (int x = 0; x < maxX; x++)
                for (int z = 0; z < maxZ; z++)
                {
                    if (_world.Vertices[x, z].Length != 0)
                    {
                        worldEffect.View = _viewMatrix;
                        worldEffect.World = Matrix.CreateTranslation(x * _world.RegionSizeX, 0, z * _world.RegionSizeZ);
                        worldEffect.Projection = _perspectiveMatrix;

                        graphicsDevice.RasterizerState = CounterClockwiseCull;

                        foreach (EffectPass pass in worldEffect.CurrentTechnique.Passes)
                            pass.Apply();

                        graphicsDevice.SetVertexBuffer(_world.VertexBuffers[x, z]);
                        graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _world.Vertices[x, z].Length / 3);
                    }
                }
        }

        public void Player(Player _player, ref Matrix _viewMatrix, ref Matrix _perspectiveMatrix)
        {
            //throw new NotImplementedException();

            graphicsDevice.RasterizerState = NoCullMode;
            foreach (ModelMesh mesh in _player.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateRotationY(MathHelper.ToRadians(45*_player.Blickrichtung))* Matrix.CreateTranslation(_player.Position);

                    effect.View = _viewMatrix;
                    effect.Projection = _perspectiveMatrix;

                }

                mesh.Draw();
            }

        }
        public void LeichterBlock(List<PlayerBlock> _block, ref Matrix _viewMatrix, ref Matrix _perspectiveMatrix)
        {
            // benötige Block mit Größe 1x1x1? und Mittelpunkt in 0.5x0.5x0.5
            for (int i = 0; i < _block.Count; i++)
            {
                foreach (ModelMesh mesh in _block[i].Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = Matrix.CreateTranslation(_block[i].Position);

                        effect.View = _viewMatrix;

                        effect.Projection = _perspectiveMatrix;

                    }

                    mesh.Draw();
                }
            }
        }
        // ...
    }
}
