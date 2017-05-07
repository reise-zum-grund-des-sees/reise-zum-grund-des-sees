using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ReiseZumGrundDesSees
{
	class MainMenu
	{
		public MainMenu(ContentManager _content)
		{
		}

		public void Update(InputEventArgs _args, IMenuCallback _callback)
		{
			throw new NotImplementedException();
		}
		
		public void Render(SpriteBatch _spriteBatch)
		{
			throw new NotImplementedException();
		}
	}

	interface IMenuCallback
	{
		void StartNewGame();
		void LoadGame(string _path);
		void SaveGame(string _path);
		void ShowCredits();
		void ExitGame();
	}
}
