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
            model = ContentManager.Load<Model>(ContentRessources.MODEL_BLOCK_LEICHT); //Modell muss noch geändert werden
        }

        public void ChangeModel()
        {
            model = ContentManager.Load<Model>(ContentRessources.MODEL_BLOCK_MEDIUM); //Modell muss noch geändert werden
        }

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {
            Matrix _worldMatrix = Matrix.CreateScale(0.25f) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0, 0.5f, 0)));
            _effect.WorldMatrix = _worldMatrix;
            _effect.VertexFormat = VertexFormat.Position;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = _effect.Effect;

                mesh.Draw();
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
