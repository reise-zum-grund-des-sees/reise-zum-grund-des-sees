using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
//using System.Windows.Controls.Button;

namespace ReiseZumGrundDesSees
{
	class MainMenu
	{
        public Texture2D background;
        public Texture2D Button_Neues_Spiel_starten;
        public Texture2D Button_Spiel_Laden;
        public Texture2D Button_Spiel_Speichern;
        public Texture2D Button_Verlassen;
        public Texture2D Button_Credits;

        private Rectangle neuGame = new Rectangle(300, 70, 200, 70);
        private Rectangle load = new Rectangle(300, 150, 200, 70);
        private Rectangle save = new Rectangle(300, 230, 200, 70);
        private Rectangle credits = new Rectangle(300, 310, 200, 70);
        private Rectangle leave = new Rectangle(300, 390, 200, 70);
        private Rectangle full;

        private void CreateRectangles(int x, int y)
        {
            neuGame = new Rectangle((int)(x * 0.4), (int)(y * 0.15), (int)(x * 0.2), (int)(y * 0.1));
            load = new Rectangle((int)(x * 0.4), (int)(y * 0.30), (int)(x * 0.2), (int)(y * 0.1));
            save = new Rectangle((int)(x * 0.4), (int)(y * 0.45), (int)(x * 0.2), (int)(y * 0.1));
            credits = new Rectangle((int)(x * 0.4), (int)(y * 0.60), (int)(x * 0.2), (int)(y * 0.1));
            leave = new Rectangle((int)(x * 0.4), (int)(y * 0.75), (int)(x * 0.2), (int)(y * 0.1));
            full = new Rectangle(0, 0, x, y);
        }




        public MainMenu(ContentManager Content)
		{
            background = Content.Load<Texture2D>(ReiseZumGrundDesSees.Content.TEXTURE_BACKGROUND);
            Button_Neues_Spiel_starten = Content.Load<Texture2D>(ReiseZumGrundDesSees.Content.BUTTON_NEUES_SPIEL_STARTEN);
            Button_Spiel_Laden = Content.Load<Texture2D>(ReiseZumGrundDesSees.Content.BUTTON_SPIEL_LADEN);
            Button_Spiel_Speichern = Content.Load<Texture2D>(ReiseZumGrundDesSees.Content.BUTTON_SPIEL_SPEICHERN);
            Button_Verlassen = Content.Load<Texture2D>(ReiseZumGrundDesSees.Content.BUTTON_VERLASSEN);
            Button_Credits = Content.Load<Texture2D>(ReiseZumGrundDesSees.Content.BUTTON_CREDITS);
		}

        public void Update(InputEventArgs _args, IMenuCallback _callback, Point _windowSize)
		{
            CreateRectangles(_windowSize.X, _windowSize.Y);

            if (_args.Events.HasFlag(InputEventList.MouseLeftClick))
            {
                if (neuGame.Contains(_args.MousePosition))
                {
                    _callback.StartNewGame();
                }

                if (load.Contains(_args.MousePosition))
                {
                    System.Windows.Forms.FolderBrowserDialog _dialog = new System.Windows.Forms.FolderBrowserDialog();
                    _dialog.SelectedPath = Environment.CurrentDirectory;
                    if (_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        _callback.LoadGame(_dialog.SelectedPath);
                }

                if (save.Contains(_args.MousePosition))
                {
                    System.Windows.Forms.FolderBrowserDialog _dialog = new System.Windows.Forms.FolderBrowserDialog();
                    _dialog.SelectedPath = Environment.CurrentDirectory;
                    if (_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        _callback.SaveGame(_dialog.SelectedPath);
                } 

                if (credits.Contains(_args.MousePosition))
                {
                    _callback.ShowCredits();
                }

                if (leave.Contains(_args.MousePosition))
                {
                    _callback.ExitGame();
                }
            }
           

			//throw new NotImplementedException();
		}
		
		public void Render(SpriteBatch _spriteBatch)
		{
            _spriteBatch.Begin();
            _spriteBatch.Draw(background, full, Color.White);
            _spriteBatch.Draw(Button_Neues_Spiel_starten, neuGame, Color.White);
            _spriteBatch.Draw(Button_Spiel_Laden, load, Color.White);
            _spriteBatch.Draw(Button_Spiel_Speichern, save, Color.White);
            _spriteBatch.Draw(Button_Credits, credits, Color.White);
            _spriteBatch.Draw(Button_Verlassen, leave, Color.White);
            _spriteBatch.End();
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
