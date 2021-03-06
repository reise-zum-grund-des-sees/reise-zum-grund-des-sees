using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ReiseZumGrundDesSees
{
    class Treasure : IRenderable
    {
        ContentManager ContentManager;
        Model model;
        public Vector3 Position
        {
            get;
        }

        public Treasure(Vector3 _position)
        {
            Position = _position;
        }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            model = ContentManager.Load<Model>(ContentRessources.MODEL_TRUHE_ZU); //Modell muss noch geändert werden
        }

        public void ChangeModel()
        {
            model = ContentManager.Load<Model>(ContentRessources.MODEL_TRUHE_OFFEN); //Modell muss noch geändert werden
          
        }

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {

            Matrix _worldMatrix = Matrix.CreateScale(1f) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0, 0.001f, 0)));
            _effect.WorldMatrix = _worldMatrix;
            _effect.VertexFormat = VertexFormat.PositionColor;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    _effect.Color = new Color((part.Effect as BasicEffect).DiffuseColor);
                    DebugHelper.Information.RenderedOtherVertices += (uint)part.NumVertices;

                    if (part.PrimitiveCount > 0)
                    {
                        _grDevice.SetVertexBuffer(part.VertexBuffer);
                        _grDevice.Indices = part.IndexBuffer;

                        for (int j = 0; j < part.Effect.CurrentTechnique.Passes.Count; j++)
                        {
                            _effect.Effect.CurrentTechnique.Passes[j].Apply();
                            _grDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                        }
                    }
                }
            }
        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            return (ref GameState _state) =>
            {

            };
        }

    }
}
