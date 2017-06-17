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
        int ChangeRequest;

        public Vector3Int Position
        {
            get;
        }


        public WorldBlock Type
        {
            get; private set;
        }

        public void press()
        {

            if (Type == WorldBlock.PressurePlateUp)
            {

                ChangeRequest = 1;
            }
        }

        public void depress()
        {
            ChangeRequest = 2;
        }

        public PressurePlate(Vector3Int _position, int _type)//_type= 0 up on create, 1 down
        {
            //für id müsste man alle erstellten Plattenpositionen speichern und wieder zuteilen, wenn Position schon mal vorhanden
            Position = _position;
            if (_type == 0)
            {
                Type = WorldBlock.PressurePlateUp;
            }
            else Type = WorldBlock.PressurePlateDown;
        }

        public PressurePlate(ConfigFile.ConfigNode _config)
            : this(Vector3Int.Parse(_config.Items["position"]), 0)
        { }

        public void Initialize(GraphicsDevice _graphicsDevice, ContentManager _contentManager)
        {
            ContentManager = _contentManager;
            Model = _contentManager.Load<Model>(Content.MODEL_BLOCK_LEICHT);
        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix, GraphicsDevice _grDevice)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    if (Type == WorldBlock.PressurePlateUp)
                        effect.World = Matrix.CreateScale(0.5f) * Matrix.CreateScale(1, 0.5f, 1) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0.5f, 0.25f, 0.5f)));
                    else
                        effect.World = Matrix.CreateScale(0.5f) * Matrix.CreateScale(1, 0.1f, 1) * Matrix.CreateTranslation(Vector3.Add(Position, new Vector3(0.5f, 0.05f, 0.5f)));

                    effect.View = _viewMatrix;

                    effect.Projection = _perspectiveMatrix;

                }

                mesh.Draw();
            }
        }

        public UpdateDelegate Update(GameState.View _view, GameFlags _flags, InputEventArgs _inputArgs, double _passedTime)
        {
            //if (Type == WorldBlock.PressurePlateDown) doSth();
            return (ref GameState _state) =>
            {
                if (ChangeRequest == 1 && Type == WorldBlock.PressurePlateUp)
                {
                    _state.World.Blocks[Position.X, Position.Y, Position.Z] = WorldBlock.PressurePlateDown;
                    Type = WorldBlock.PressurePlateDown;
                    Model = ContentManager.Load<Model>(Content.MODEL_BLOCK_SCHWER);
                }
                if (ChangeRequest == 2 && Type == WorldBlock.PressurePlateDown)
                {
                    _state.World.Blocks[Position.X, Position.Y, Position.Z] = WorldBlock.PressurePlateUp;
                    Type = WorldBlock.PressurePlateUp;
                    Model = ContentManager.Load<Model>(Content.MODEL_BLOCK_LEICHT);
                }

            };
        }

        public ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper)
        {
            ConfigFile.ConfigNode _node = new ConfigFile.ConfigNode();

            _node.Items["position"] = Position.ToString();

            return _node;
        }
    }
}
