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

        private readonly RasterizerState CounterClockwiseCull, NoCullMode;

        private readonly Model worldEditorCursor;

        public Render(GraphicsDevice _graphicsDevice, ContentManager _content)
        {
            graphicsDevice = _graphicsDevice;
            worldEditorCursor = _content.Load<Model>("cursor");

            CounterClockwiseCull = new RasterizerState();
            CounterClockwiseCull.CullMode = CullMode.CullCounterClockwiseFace;

            NoCullMode = new RasterizerState();
            NoCullMode.CullMode = CullMode.None;
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

        public void World(World _world, ref Matrix _viewMatrix, ref Matrix _perspectiveMatrix, out uint _renderedVertices, out uint _renderedChunks)
        {
            _renderedVertices = 0;
            _renderedChunks = 0;
        }

        public void PlayerR(Player _player, ref Matrix _viewMatrix, ref Matrix _perspectiveMatrix)
        {
            //throw new NotImplementedException();

            graphicsDevice.RasterizerState = NoCullMode;
            foreach (ModelMesh mesh in _player.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
       
                    effect.World = Matrix.CreateRotationY(Player.Blickrichtung) * Matrix.CreateTranslation(_player.Position);

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
                if (_block[i].Zustand == (int)PlayerBlock.ZustandList.Gesetzt) { 
                foreach (ModelMesh mesh in _block[i].Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = Matrix.CreateScale(0.5f)* Matrix.CreateTranslation(Vector3.Add(_block[i].Position, new Vector3(0, 0.5f, 0)));

                        effect.View = _viewMatrix;

                        effect.Projection = _perspectiveMatrix;

                    }

                    mesh.Draw();
                }
            }
        }
    }
        public void LeverR(List<Lever> _lever, ref Matrix _viewMatrix, ref Matrix _perspectiveMatrix)
        {

            for (int i = 0; i < _lever.Count; i++)
            {

                foreach (ModelMesh mesh in _lever[i].Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = Matrix.CreateRotationZ((float)_lever[i].Rotation) *Matrix.CreateRotationX((float)Math.PI*3/2)*Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Add(_lever[i].Position,new Vector3(0.5f, 0.5f, 0.5f)));

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
