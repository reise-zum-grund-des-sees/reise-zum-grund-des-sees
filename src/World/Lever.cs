using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    class Lever : ISpecialBlock, IRotateable
    {
        public Model Model;

        public bool is_pressed;
        ContentManager ContentManager;
        public double Rotation;
        public bool alive;

        private ActionSyntaxParser.GameAction OnPressed, OnReleased;

        public Vector3Int Position
        {
            get;
        }

        public WorldBlock Type
        {
            get;
        }

        public Lever(Vector3Int _position)
        {
            alive = true;
            Position = _position;
            Type = WorldBlock.Lever;
            //Position = _position + new Vector3(0.5f,0.5f,0.5f);
            is_pressed = false;
         
        }

        public Lever(ConfigFile.ConfigNode _config, ObjectIDMapper _idMapper)
            : this(Vector3Int.Parse(_config.Items["position"]))
        {
            if (_config.Items.ContainsKey("pressed"))
            {
                is_pressed = Convert.ToBoolean(_config.Items["pressed"].ToString());
            }
            if (_config.Items.ContainsKey("on_pressed"))
            {
                OnPressed = ActionSyntaxParser.Parse(_config.Items["on_pressed"], this, _idMapper);
            }
            if (_config.Items.ContainsKey("on_released"))
            {
                OnReleased = ActionSyntaxParser.Parse(_config.Items["on_released"], this, _idMapper);
            }
            if (_config.Items.ContainsKey("rotation"))
            {
                Rotation = Convert.ToDouble(_config.Items["rotation"].ToString());
            }
          
            }


        public void Press(GameState _gs)
        {
            if (alive == true)
            {
                if (is_pressed == false)
                {
                    Model = ContentManager.Load<Model>(ContentRessources.MODEL_SCHALTER_UNTEN);
                    is_pressed = true;
                    OnPressed?.BaseAction(_gs);
                }
                else
                {
                    Model = ContentManager.Load<Model>(ContentRessources.MODEL_SCHALTER_OBEN);
                    is_pressed = false;
                    OnReleased?.BaseAction(_gs);
                }
            }
        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            return (ref GameState _state) =>
            {

            };
        }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            Model = _contentManager.Load<Model>((is_pressed)? ContentRessources.MODEL_SCHALTER_UNTEN : ContentRessources.MODEL_SCHALTER_OBEN);
        }

        public void Render(GameFlags _flags, IEffect _effect, GraphicsDevice _grDevice)
        {
            Matrix _worldMatrix = Matrix.CreateRotationZ((float)Rotation) * Matrix.CreateRotationX((float)Math.PI * 3 / 2) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0.5f, 0.5f, 0.5f)));
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

        public ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper)
        {
            ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();

            _node.Items["pressed"] = is_pressed.ToString();
            _node.Items["position"] = Position.ToString();
            _node.Items["rotation"] = Rotation.ToString();
            if (OnReleased != null)
                _node.Items["on_released"] = OnReleased.ActionEncoding(_mapper);
            if (OnPressed != null)
                _node.Items["on_pressed"] = OnPressed.ActionEncoding(_mapper);

            return _node;
        }

        public void Rotate(float _angle)
        {
            Rotation += _angle;
        }
    }
}
