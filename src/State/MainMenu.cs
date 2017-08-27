using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ReiseZumGrundDesSees
{
	class MainMenu : IOverlay
	{
        private Texture2D texture;

        private bool showSaveGame = false;
        private Point windowSize;
        private GameFlags flags;

        private readonly IMenuCallback menuCallback;

        public MainMenu(Texture2D _texture, ContentManager Content, IMenuCallback _menuCallback)
		{
            menuCallback = _menuCallback;
            texture = _texture;
		}

        public void Update(InputEventArgs _args, GameState _gameState, GameFlags _flags, Point _windowSize)
		{
            flags = _flags;
            windowSize = _windowSize;
            showSaveGame = _flags.HasFlag(GameFlags.GameLoaded);

            if (_args.Events.HasFlag(InputEventList.MouseLeftClick))
            {
                if (scale(box_newGameBox, windowSize).Contains(_args.MousePosition))
                {
                    menuCallback.StartNewGame();
                }

                if (scale(box_loadGameBox, windowSize).Contains(_args.MousePosition))
                {
                    System.Windows.Forms.FolderBrowserDialog _dialog = new System.Windows.Forms.FolderBrowserDialog();
                    _dialog.SelectedPath = Environment.CurrentDirectory;
                    if (_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        menuCallback.LoadGame(_dialog.SelectedPath);
                }

                if (scale(box_endGameBox, windowSize).Contains(_args.MousePosition))
                    menuCallback.ExitGame();
            }
		}

        private readonly Rectangle tex_box1 = new Rectangle(407, 3, 408, 138);
        private readonly Rectangle tex_box2 = new Rectangle(405, 138, 407, 141);
        private readonly Rectangle tex_box3 = new Rectangle(406, 276, 403, 137);
        private readonly Rectangle tex_square1 = new Rectangle(10, 7, 395, 404);

        private readonly Rectangle tex_newGame = new Rectangle(213, 440, 330, 79);
        private readonly Rectangle tex_loadGame = new Rectangle(545, 441, 293, 80);
        private readonly Rectangle tex_options = new Rectangle(9, 667, 368, 78);
        private readonly Rectangle tex_endGame = new Rectangle(206, 521, 238, 67);

        private readonly Rectangle box_newGameBox = new Rectangle(618, 0, 382, 200);
        private readonly Rectangle box_loadGameBox = new Rectangle(618, 200, 382, 200);
        private readonly Rectangle box_optionsBox = new Rectangle(618, 400, 382, 200);
        private readonly Rectangle box_emptyBox = new Rectangle(618, 600, 382, 200);
        private readonly Rectangle box_endGameBox = new Rectangle(618, 800, 382, 200);

        private readonly Rectangle box_square = new Rectangle(0, 0, 618, 1000);

        private readonly Rectangle box_newGameText = new Rectangle(688, 60, 242, 80);
        private readonly Rectangle box_loadGameText = new Rectangle(708, 260, 202, 80);
        private readonly Rectangle box_optionsText = new Rectangle(688, 460, 242, 80);
        private readonly Rectangle box_endGameText = new Rectangle(708, 860, 202, 80);
		
		public void Render(SpriteBatch _spriteBatch)
		{
            _spriteBatch.Begin();

            _spriteBatch.Draw(texture, scale(box_newGameBox, windowSize), tex_box1, Color.White);
            _spriteBatch.Draw(texture, scale(box_newGameText, windowSize), tex_newGame, Color.White);

            _spriteBatch.Draw(texture, scale(box_loadGameBox, windowSize), tex_box2, Color.White);
            _spriteBatch.Draw(texture, scale(box_loadGameText, windowSize), tex_loadGame, Color.White);

            _spriteBatch.Draw(texture, scale(box_optionsBox, windowSize), tex_box3, Color.White);
            _spriteBatch.Draw(texture, scale(box_optionsText, windowSize), tex_options, Color.White);

            _spriteBatch.Draw(texture, scale(box_emptyBox, windowSize), tex_box2, Color.White);

            _spriteBatch.Draw(texture, scale(box_endGameBox, windowSize), tex_box1, Color.White);
            _spriteBatch.Draw(texture, scale(box_endGameText, windowSize), tex_endGame, Color.White);

            if (!flags.HasFlag(GameFlags.GameLoaded))
                _spriteBatch.Draw(texture, scale(box_square, windowSize), tex_square1, Color.White);
            _spriteBatch.End();
		}

        private Rectangle scale(Rectangle _box, Point _size)
        {
            return new Rectangle((int)Math.Round(_box.X * _size.X / 1000.0),
                                 (int)Math.Round(_box.Y * _size.Y / 1000.0),
                                 (int)Math.Round(_box.Width * _size.X / 1000.0),
                                 (int)Math.Round(_box.Height * _size.Y / 1000.0));
        }
	}

	interface IMenuCallback
	{
		void StartNewGame();
		void LoadGame(string _path);
        void ShowOptions();
		void ExitGame();
	}
}
