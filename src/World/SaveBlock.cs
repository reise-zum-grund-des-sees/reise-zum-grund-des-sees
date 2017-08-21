using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    class SaveBlock : ISpecialBlock
    {
        public Vector3Int Position
        {
            get;
        }

        public WorldBlock Type
        {
            get;
        }
        public Model Model;
        ContentManager ContentManager;

        public SaveBlock(ConfigFile.ConfigNode _config, ObjectIDMapper _idMapper)
            : this(Vector3Int.Parse(_config.Items["position"])) { }

        public SaveBlock(Vector3Int _position)
        {
            Position = _position;
            Type = WorldBlock.SaveBlock;
        }

        public ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper)
        {
            ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();

            _node.Items["position"] = Position.ToString();
    

            return _node;
        }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            Model = _contentManager.Load<Model>(Content.MODEL_SAVEPOINT);
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix, GraphicsDevice _grDevice)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateScale(0.45f) *Matrix.CreateRotationX(-MathHelper.PiOver2)* Matrix.CreateScale(1, 0.15f, 1) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0.5f, 0.0f, 0.5f)));

                    effect.View = _viewMatrix;

                    effect.Projection = _perspectiveMatrix;

                }

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
