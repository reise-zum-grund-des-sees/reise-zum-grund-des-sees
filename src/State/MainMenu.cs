using System;
using System.IO;
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

        public State CurrentState = State.Default;
        public enum State
        {
            Default,
            Load,
            Save
        }

        private Point windowSize;
        private GameFlags flags;

        private readonly IMenuCallback menuCallback;

        private readonly SpriteFont font;

        public MainMenu(Texture2D _texture, ContentManager Content, IMenuCallback _menuCallback)
        {
            menuCallback = _menuCallback;
            texture = _texture;
            font = Content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_20);
        }

        public void Update(InputEventArgs _args, GameState _gameState, GameFlags _flags, Point _windowSize)
        {
            flags = _flags;
            windowSize = _windowSize;
            showSaveGame = _flags.HasFlag(GameFlags.GameLoaded);

            if (_args.Events.HasFlag(InputEventList.MouseLeftClick))
            {
                if (CurrentState == State.Default)
                {
                    if (scale(box_newGameBox, windowSize).Contains(_args.MousePosition))
                        menuCallback.StartNewGame();
                    else if (scale(box_loadGameBox, windowSize).Contains(_args.MousePosition))
                        CurrentState = State.Load;
                    else if (scale(box_saveGameBox, windowSize).Contains(_args.MousePosition))
                        CurrentState = State.Save;
                }
                else if (CurrentState == State.Load)
                {
                    if (scale(box_newGameBox, windowSize).Contains(_args.MousePosition))
                    {
                        string _path = Path.Combine(Environment.CurrentDirectory, "save", "slot1");
                        if (Directory.Exists(_path))
                            menuCallback.LoadGame(_path);
                    }
                    else if (scale(box_loadGameBox, windowSize).Contains(_args.MousePosition))
                    {
                        string _path = Path.Combine(Environment.CurrentDirectory, "save", "slot2");
                        if (Directory.Exists(_path))
                            menuCallback.LoadGame(_path);
                    }
                    else if (scale(box_saveGameBox, windowSize).Contains(_args.MousePosition))
                    {
                        string _path = Path.Combine(Environment.CurrentDirectory, "save", "slot3");
                        if (Directory.Exists(_path))
                            menuCallback.LoadGame(_path);
                    }
                }
                else if (CurrentState == State.Save)
                {
                    if (scale(box_newGameBox, windowSize).Contains(_args.MousePosition))
                    {
                        string _path = Path.Combine(Environment.CurrentDirectory, "save", "slot1");
                        menuCallback.SaveGame(_path);
                    }
                    else if (scale(box_loadGameBox, windowSize).Contains(_args.MousePosition))
                    {
                        string _path = Path.Combine(Environment.CurrentDirectory, "save", "slot2");
                        menuCallback.SaveGame(_path);
                    }
                    else if (scale(box_saveGameBox, windowSize).Contains(_args.MousePosition))
                    {
                        string _path = Path.Combine(Environment.CurrentDirectory, "save", "slot3");
                        menuCallback.SaveGame(_path);
                    }
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
        private readonly Rectangle tex_saveGame = new Rectangle(220, 747, 420, 81);
        private readonly Rectangle tex_endGame = new Rectangle(206, 521, 238, 67);
        private readonly Rectangle tex_slot1 = new Rectangle(441, 529, 150, 63);
        private readonly Rectangle tex_slot2 = new Rectangle(590, 527, 171, 68);
        private readonly Rectangle tex_slot3 = new Rectangle(842, 445, 170, 68);

        private readonly Rectangle box_newGameBox = new Rectangle(618, 0, 382, 200);
        private readonly Rectangle box_loadGameBox = new Rectangle(618, 200, 382, 200);
        private readonly Rectangle box_saveGameBox = new Rectangle(618, 400, 382, 200);
        private readonly Rectangle box_emptyBox = new Rectangle(618, 600, 382, 200);
        private readonly Rectangle box_endGameBox = new Rectangle(618, 800, 382, 200);

        private readonly Rectangle box_square = new Rectangle(0, 0, 618, 1000);

        private readonly Rectangle box_newGameText = new Rectangle(688, 60, 242, 80);
        private readonly Rectangle box_loadGameText = new Rectangle(708, 260, 202, 80);
        private readonly Rectangle box_saveGameText = new Rectangle(678, 460, 262, 80);
        private readonly Rectangle box_endGameText = new Rectangle(708, 860, 202, 80);
        private readonly Rectangle box_slot1 = new Rectangle(708, 60, 202, 80);
        private readonly Rectangle box_slot2 = new Rectangle(708, 260, 202, 80);
        private readonly Rectangle box_slot3 = new Rectangle(708, 460, 202, 80);

        public void Render(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();

            if (CurrentState == State.Default)
            {
                _spriteBatch.Draw(texture, scale(box_newGameBox, windowSize), tex_box1, Color.White);
                _spriteBatch.Draw(texture, scale(box_newGameText, windowSize), tex_newGame, Color.White);

                _spriteBatch.Draw(texture, scale(box_loadGameBox, windowSize), tex_box2, Color.White);
                _spriteBatch.Draw(texture, scale(box_loadGameText, windowSize), tex_loadGame, Color.White);

                _spriteBatch.Draw(texture, scale(box_saveGameBox, windowSize), tex_box3, Color.White);
                _spriteBatch.Draw(texture, scale(box_saveGameText, windowSize), tex_saveGame, Color.White);

                _spriteBatch.Draw(texture, scale(box_emptyBox, windowSize), tex_box2, Color.White);

                _spriteBatch.Draw(texture, scale(box_endGameBox, windowSize), tex_box1, Color.White);
                _spriteBatch.Draw(texture, scale(box_endGameText, windowSize), tex_endGame, Color.White);
            }
            else if (CurrentState == State.Load || CurrentState == State.Save)
            {
                string _path1 = Path.Combine(Environment.CurrentDirectory, "save", "slot1");
                if (Directory.Exists(_path1))
                {
                    _spriteBatch.Draw(texture, scale(box_newGameBox, windowSize), tex_box1, Color.White);
                    _spriteBatch.Draw(texture, scale(box_slot1, windowSize), tex_slot1, Color.White);
                    DateTime _lastWriteTime = File.GetLastWriteTime(Path.Combine(_path1, "state.conf"));
                    string _lastWriteString = _lastWriteTime.ToShortDateString() + " - " + _lastWriteTime.ToLongTimeString();
                    Vector2 _size = font.MeasureString(_lastWriteString);
                    _spriteBatch.DrawString(font, _lastWriteString,
                        new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, 140 / 1000f * windowSize.Y), Color.Black);
                }
                else
                {
                    _spriteBatch.Draw(texture, scale(box_newGameBox, windowSize), tex_box1, new Color(0.8f, 0.8f, 0.8f));
                    _spriteBatch.Draw(texture, scale(box_slot1, windowSize), tex_slot1, new Color(0.8f, 0.8f, 0.8f));
                    Vector2 _size = font.MeasureString("Leer");
                    _spriteBatch.DrawString(font, "Leer",
                        new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, 140 / 1000f * windowSize.Y), Color.Black);
                }

                string _path2 = Path.Combine(Environment.CurrentDirectory, "save", "slot2");
                if (Directory.Exists(_path2))
                {
                    _spriteBatch.Draw(texture, scale(box_loadGameBox, windowSize), tex_box2, Color.White);
                    _spriteBatch.Draw(texture, scale(box_slot2, windowSize), tex_slot2, Color.White);
                    DateTime _lastWriteTime = File.GetLastWriteTime(Path.Combine(_path2, "state.conf"));
                    string _lastWriteString = _lastWriteTime.ToShortDateString() + " - " + _lastWriteTime.ToLongTimeString();
                    Vector2 _size = font.MeasureString(_lastWriteString);
                    _spriteBatch.DrawString(font, _lastWriteString,
                        new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, (200 + 140) / 1000f * windowSize.Y), Color.Black);
                }
                else
                {
                    _spriteBatch.Draw(texture, scale(box_loadGameBox, windowSize), tex_box2, new Color(0.8f, 0.8f, 0.8f));
                    _spriteBatch.Draw(texture, scale(box_slot2, windowSize), tex_slot2, new Color(0.8f, 0.8f, 0.8f));
                    Vector2 _size = font.MeasureString("Leer");
                    _spriteBatch.DrawString(font, "Leer",
                        new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, (200 + 140) / 1000f * windowSize.Y), Color.Black);
                }

                string _path3 = Path.Combine(Environment.CurrentDirectory, "save", "slot3");
                if (Directory.Exists(_path3))
                {
                    _spriteBatch.Draw(texture, scale(box_saveGameBox, windowSize), tex_box3, Color.White);
                    _spriteBatch.Draw(texture, scale(box_slot3, windowSize), tex_slot3, Color.White);
                    DateTime _lastWriteTime = File.GetLastWriteTime(Path.Combine(_path3, "state.conf"));
                    string _lastWriteString = _lastWriteTime.ToShortDateString() + " - " + _lastWriteTime.ToLongTimeString();
                    Vector2 _size = font.MeasureString(_lastWriteString);
                    _spriteBatch.DrawString(font, _lastWriteString,
                        new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, (400 + 140) / 1000f * windowSize.Y), Color.Black);
                }
                else
                {
                    _spriteBatch.Draw(texture, scale(box_saveGameBox, windowSize), tex_box3, new Color(0.8f, 0.8f, 0.8f));
                    _spriteBatch.Draw(texture, scale(box_slot3, windowSize), tex_slot3, new Color(0.8f, 0.8f, 0.8f));
                    Vector2 _size = font.MeasureString("Leer");
                    _spriteBatch.DrawString(font, "Leer",
                        new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, (400 + 140) / 1000f * windowSize.Y), Color.Black);
                }

                _spriteBatch.Draw(texture, scale(box_emptyBox, windowSize), tex_box2, Color.White);

                _spriteBatch.Draw(texture, scale(box_endGameBox, windowSize), tex_box1, Color.White);
                _spriteBatch.Draw(texture, scale(box_endGameText, windowSize), tex_endGame, Color.White);
            }

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
        void SaveGame(string _path);
    }
}
