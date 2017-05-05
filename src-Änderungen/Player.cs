using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
	class Player : IUpdateable
	{
        public Model model;
        public Vector3 pos;

        public Player(ContentManager contentManager, Vector3 position)
        {
            pos = position;
            model = contentManager.Load<Model>("Block");
        }

        public UpdateDelegate Update(GameState.View _state, InputManager.InputEvent _inputEvents, double _passedTime)
		{
			throw new NotImplementedException();
		}

        public void bewegen()
        {
            KeyboardState state = Keyboard.GetState();
            Keys[] pressed = state.GetPressedKeys();
            foreach (Keys key in pressed)
            {
                if (key.Equals(Keys.W))
                {
                    pos.Z += 0.016f;
                }

                if (key.Equals(Keys.A))
                {
                    pos.X += 0.016f;
                }

                if (key.Equals(Keys.S))
                {
                    pos.Z -= 0.016f;
                }

                if (key.Equals(Keys.D))
                {
                    pos.X -= 0.016f;
                }
            }
        }

    }
}
