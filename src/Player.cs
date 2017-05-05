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

        public UpdateDelegate Update(GameState.View _stateView, InputEventArgs _inputArgs, double _passedTime)
		{
			// Nicht die Variablen hier ändern. Aber Kollisionserkennung hier berechnen.

			// [...]

			return (ref GameState _state) =>
			{
				// Hier Variablen ändern - direkt, oder über _state.Player ...
				if (_inputArgs.Events.HasFlag(InputEventList.MoveForwards))
				{
					Position.Z += 0.016f;
				}

				if (_inputArgs.Events.HasFlag(InputEventList.MoveLeft))
				{
					Position.X += 0.016f;
				}

				if (_inputArgs.Events.HasFlag(InputEventList.MoveBackwards))
				{
					Position.Z -= 0.016f;
				}

				if (_inputArgs.Events.HasFlag(InputEventList.MoveRight))
				{
					Position.X -= 0.016f;
				}
			};
        }

    }
}
