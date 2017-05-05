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
        public Vector3 Position;

        public Player(Vector3 _position)
        {
            Position = _position;
        }

        public UpdateDelegate Update(GameState.View _stateView, InputEvent _inputEvents, double _passedTime)
		{
			// Nicht die Variablen hier ändern. Aber Kollisionserkennung hier berechnen.

			// [...]

			return new UpdateDelegate((ref GameState _state) =>
			{
				// Hier Variablen ändern - direkt, oder über _state.Player ...
				if (_inputEvents.HasFlag(InputEvent.MoveForwards))
				{
					Position.Z += 0.016f;
				}

				if (_inputEvents.HasFlag(InputEvent.MoveLeft))
				{
					Position.X += 0.016f;
				}

				if (_inputEvents.HasFlag(InputEvent.MoveBackwards))
				{
					Position.Z -= 0.016f;
				}

				if (_inputEvents.HasFlag(InputEvent.MoveRight))
				{
					Position.X -= 0.016f;
				}
			});
        }

    }
}
