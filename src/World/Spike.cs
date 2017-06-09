using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    class Spike : ISpecialBlock
    {
        ContentManager ContentManager;
        public Model Model;

        public Vector3Int Position
        {
            get;
        }

        public WorldBlock Type
        {
            get;
        }
        public Spike(ContentManager _contentManager, Vector3Int _position)
        {
                Position = _position;
                Type = WorldBlock.Spikes;
                ContentManager = _contentManager;
                Model = _contentManager.Load<Model>(Content.MODEL_STACHELN);
                //SpikeList.Add(this);
         
        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            return (ref GameState _state) =>
            {

            };
        }

        public void Initialize(GraphicsDevice _graphicsDevice)
        {
        
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix)
        {
            {

                    foreach (ModelMesh mesh in Model.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.World = Matrix.CreateScale(0.5f) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0.5f, 0, 0.5f)));

                            effect.View = _viewMatrix;

                            effect.Projection = _perspectiveMatrix;

                        }

                        mesh.Draw();
                    }

               
            }
        }
    }
}
