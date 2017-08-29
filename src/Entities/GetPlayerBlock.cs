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
    class GetPlayerBlock : IRenderable
    {
        ContentManager ContentManager;
        Model[] modelarray;
        Model model;
        public int Art;
        public static List<GetPlayerBlock> GetPlayerBlockList = new List<GetPlayerBlock>();

        public Vector3 Position
        {
            get;
        }

/*
        public GetPlayerBlock(ConfigFile.ConfigNode _config, ObjectIDMapper _idMapper)
            : this(Vector3Int.Parse(_config.Items["position"]), int.Parse(_config.Items["art"]))
        { }
        */
        public GetPlayerBlock(Vector3 _position, int _art)
        {       
            Position = _position;        
            Art = _art;
  
        }
        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            modelarray = new Model[3];       
                modelarray[0] = _contentManager.Load<Model>(ContentRessources.MODEL_BLOCK_LEICHT);
          
                modelarray[1] = _contentManager.Load<Model>(ContentRessources.MODEL_BLOCK_MEDIUM);
         
                modelarray[2] = _contentManager.Load<Model>(ContentRessources.MODEL_BLOCK_SCHWER);
            
        }

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {
            if (Art == 0) model = modelarray[0];
            if (Art == 1) model = modelarray[1];
            if (Art == 2) model = modelarray[2];

            Matrix _worldMatrix = Matrix.CreateScale(0.25f) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0, 0.5f, 0)));
            _effect.WorldMatrix = _worldMatrix;
            _effect.VertexFormat = VertexFormat.Position;

            foreach (ModelMesh mesh in model.Meshes)
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
        /*
        public ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper)
        {
            ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();

            _node.Items["position"] = Position.ToString();
            _node.Items["art"] = Art.ToString();
            return _node;
        }
        */
    }
}
