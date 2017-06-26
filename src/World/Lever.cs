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
            Rotation = 0;
        }

        public Lever(ConfigFile.ConfigNode _config, ObjectIDMapper _idMapper)
            : this(Vector3Int.Parse(_config.Items["position"]))
        { }


        public void press()
        {
            if (alive == true)
            {
                if (is_pressed == false)
                {
                    Model = ContentManager.Load<Model>(Content.MODEL_SCHALTER_UNTEN);
                    is_pressed = true;
                }
                else
                {
                    Model = ContentManager.Load<Model>(Content.MODEL_SCHALTER_OBEN);
                    is_pressed = false;
                }
            }
        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            //throw new NotImplementedException();
            return (ref GameState _state) =>
            {

            };
        }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            Model = _contentManager.Load<Model>((is_pressed)? Content.MODEL_SCHALTER_OBEN : Content.MODEL_SCHALTER_UNTEN);
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix, GraphicsDevice _grDevice)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateRotationZ((float)Rotation) * Matrix.CreateRotationX((float)Math.PI * 3 / 2) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0.5f, 0.5f, 0.5f)));

                    effect.View = _viewMatrix;

                    effect.Projection = _perspectiveMatrix;

                }

                mesh.Draw();
            }
        }

        public ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper)
        {
            ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();

            _node.Items["pressed"] = is_pressed.ToString();
            _node.Items["position"] = Position.ToString();

            return _node;
        }

        public void Rotate(float _angle)
        {
            Rotation += _angle;
        }
    }
}
