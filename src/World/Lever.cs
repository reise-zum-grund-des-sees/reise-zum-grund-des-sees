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
    class Lever : IWorldObject
    {
        public Model Model;

        public bool is_pressed;
        ContentManager ContentManager;
        public double Rotation;
        public bool alive;

        public Hitbox Hitbox
        {
            get;
        }

        public Vector3Int Position
        {
            get;
        }

        public WorldBlock Type
        {
            get;
        }

        public Lever(ContentManager _contentManager, Vector3Int _position)
        {
            alive = true;
            Position = _position;
            Hitbox = new Hitbox(_position.X + 0.5f, _position.Y, _position.Z + 0.5f, 1f, 1f, 1f);//richtig schieben, im render mus auch Y+0.5f gesetzt werden
            Type = WorldBlock.Lever;
            //Position = _position + new Vector3(0.5f,0.5f,0.5f);
            is_pressed = false;
            ContentManager = _contentManager;
            Rotation = 0;
            Model = _contentManager.Load<Model>("schalter_oben");
        }
        public void press()
        {
            if (alive == true)
            {
                if (is_pressed == false)
                {
                    Model = ContentManager.Load<Model>("schalter_unten");
                    is_pressed = true;
                }
                else
                {
                    Model = ContentManager.Load<Model>("schalter_oben");
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

        public void Initialize(GraphicsDevice _graphicsDevice)
        {

        }

        public void Render(GameFlags _flags, Matrix _viewMatrix, Matrix _perspectiveMatrix)
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
    }
}
