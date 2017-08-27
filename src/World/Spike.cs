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
        public Spike(Vector3Int _position)
        {
            Position = _position;
            Type = WorldBlock.Spikes;
        }

        public Spike(ConfigFile.ConfigNode _config, ObjectIDMapper _idMapper)
            : this(Vector3Int.Parse(_config.Items["position"]))
        { }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            return (ref GameState _state) =>
            {

            };
        }

        Texture2D texture;
        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            texture = _contentManager.Load<Texture2D>(ContentRessources.TEXTURE);
            Model = _contentManager.Load<Model>(ContentRessources.MODEL_STACHELN);
        }

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {
            Matrix _worldMatrix = Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0.5f, 0, 0.5f)));
            _effect.WorldMatrix = _worldMatrix;
            _effect.VertexFormat = VertexFormat.PositionTexture;
            _effect.Texture = texture;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = _effect.Effect;

                mesh.Draw();
            }
    }

    public ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper)
    {
        ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();

        _node.Items["position"] = Position.ToString();

        return _node;
    }
}
}
