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
    class PressurePlate : ISpecialBlock
    {
        public Model Model;
        ContentManager ContentManager;
        public bool is_pressed;

        private ActionSyntaxParser.GameAction OnPressed, OnReleased;

        public Vector3Int Position
        {
            get;
        }


        public WorldBlock Type
        {
            get; private set;
        }

        public void press(GameState _state)
        {



            if (is_pressed == false)
            {
                Model = ContentManager.Load<Model>(ContentRessources.MODEL_PP_UNTEN);
                is_pressed = true;
                OnPressed?.BaseAction(_state);
                _state.World.Blocks[Position.X, Position.Y, Position.Z] = WorldBlock.PressurePlateDown;
                Type = WorldBlock.PressurePlateDown;
            }
            else
            {
                Model = ContentManager.Load<Model>(ContentRessources.MODEL_PP_OBEN);
                is_pressed = false;
                OnReleased?.BaseAction(_state);
                _state.World.Blocks[Position.X, Position.Y, Position.Z] = WorldBlock.PressurePlateUp;
                Type = WorldBlock.PressurePlateUp;
            }

        }



        public PressurePlate(Vector3Int _position)//_type= 0 up on create, 1 down
        {

            Position = _position;
            is_pressed = false;
            Type = WorldBlock.PressurePlateUp;

        }

        public PressurePlate(ConfigFile.ConfigNode _config, ObjectIDMapper _idMapper)
            : this(Vector3Int.Parse(_config.Items["position"]))
        {
            if (_config.Items.ContainsKey("on_pressed"))
            {
                OnPressed = ActionSyntaxParser.Parse(_config.Items["on_pressed"], this, _idMapper);
            }
            if (_config.Items.ContainsKey("on_released"))
            {
                OnReleased = ActionSyntaxParser.Parse(_config.Items["on_released"], this, _idMapper);
            }
        }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            Model = _contentManager.Load<Model>(ContentRessources.MODEL_PP_OBEN);

        }

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {
            Matrix _worldMatrix = Matrix.CreateScale(0.20f) * Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0.5f, 0.25f, 0.5f)));
            _effect.WorldMatrix = _worldMatrix;
            _effect.VertexFormat = VertexFormat.PositionColor;

            foreach (ModelMesh mesh in Model.Meshes)
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

        public ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper)
        {
            ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();

            _node.Items["position"] = Position.ToString();

            if (OnReleased != null)
                _node.Items["on_released"] = OnReleased.ActionEncoding(_mapper);
            if (OnPressed != null)
                _node.Items["on_pressed"] = OnPressed.ActionEncoding(_mapper);

            return _node;
        }
    }
}
