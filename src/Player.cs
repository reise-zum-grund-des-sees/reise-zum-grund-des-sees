using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
	class Player : IUpdateable
	{
		public float xPos, yPos, zPos;

		public UpdateDelegate Update(GameState.View _state, InputManager.InputEvent _inputEvents, double _passedTime)
		{
			throw new NotImplementedException();
		}
	}
}
