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
            Model = _contentManager.Load<Model>(ContentRessources.MODEL_BLOCK);
        }

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {
            Matrix _worldMatrix =
                Matrix.CreateScale(0.45f) *
                Matrix.CreateRotationX(-MathHelper.PiOver2) *
                Matrix.CreateScale(1, 0.15f, 1) *
                Matrix.CreateTranslation(Position + new Vector3(0.5f, 0.0f, 0.5f));
            _effect.WorldMatrix = _worldMatrix;
            _effect.VertexFormat = VertexFormat.Position;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    DebugHelper.Information.RenderedOtherVertices += (uint)part.NumVertices;
                    part.Effect = _effect.Effect;
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
