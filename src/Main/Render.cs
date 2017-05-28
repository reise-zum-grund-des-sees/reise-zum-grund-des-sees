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

        public Render(GraphicsDevice _graphicsDevice, ContentManager _content)
        {
            graphicsDevice = _graphicsDevice;
        }

        public void PlayerR(Player _player, ref Matrix _viewMatrix, ref Matrix _perspectiveMatrix)
        {
            //throw new NotImplementedException();
            if (_player.Healthcd <= 1000 && _player.Healthcd % 100 < 50) { }
            else { 
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
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
        public void SpikeR(List<Spike> _spike, ref Matrix _viewMatrix, ref Matrix _perspectiveMatrix)
        {

            for (int i = 0; i < _spike.Count; i++)
            {

                foreach (ModelMesh mesh in _spike[i].Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = Matrix.CreateScale(0.5f)*Matrix.CreateTranslation(Vector3.Add(_spike[i].Position, new Vector3(0.5f, 0.5f, 0.5f)));

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
