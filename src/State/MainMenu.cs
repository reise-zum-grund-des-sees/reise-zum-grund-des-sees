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
        public float AnimationProgress = 1f;
        public readonly float AnimationDuration = 300f;
        public enum State
        {
            Default,
            Load,
            Save,
        }

        private Point windowSize;
        private GameFlags flags;
        private bool hide = false;

        private readonly IMenuCallback menuCallback;
        private Vector2 scalingFactor = new Vector2(1f, 1f);
        ContentManager content;

        private bool canSave = false;

        private readonly SpriteFont font;

        private Texture2D screenshot;

        public MainMenu(Texture2D _texture, ContentManager _content, IMenuCallback _menuCallback)
        {
            menuCallback = _menuCallback;
            texture = _texture;
            font = _content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_20);
            screenshot = _content.Load<Texture2D>(ContentRessources.TEXTURE_SCREENSHOT);
            content = _content;
        }

        public void Reset()
        {
            CurrentState = State.Default;
            AnimationProgress = 0;
        }

        public void Update(InputEventArgs _args, GameState _gameState, GameFlags _flags, Point _windowSize, GameTime _gameTime)
        {
            hide = !_flags.HasFlag(GameFlags.Menu);

            if (_flags.HasFlag(GameFlags.GameLoaded))
                canSave = true;

            if (hide)
            {
                if (AnimationProgress > 0)
                    AnimationProgress -= (float)(_gameTime.ElapsedGameTime.TotalMilliseconds / AnimationDuration);
                else
                    Reset();
                return;
            }

            if (AnimationProgress < 1)
                AnimationProgress += (float)(_gameTime.ElapsedGameTime.TotalMilliseconds / AnimationDuration);

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
                    {
                        if (_flags.HasFlag(GameFlags.GameLoaded))
                            CurrentState = State.Save;
                    }
                    else if (scale(box_endGameBox, windowSize).Contains(_args.MousePosition))
                        menuCallback.ExitGame();
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
                    else if (scale(box_endGameBox, windowSize).Contains(_args.MousePosition))
                        CurrentState = State.Default;
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
                    else if (scale(box_endGameBox, windowSize).Contains(_args.MousePosition))
                        CurrentState = State.Default;
                }


                scalingFactor = _windowSize.ToVector2() * 0.001f;
            }
        }

        private readonly Rectangle tex_box1 = new Rectangle(407, 3, 408, 138);
        private readonly Rectangle tex_box2 = new Rectangle(405, 138, 407, 141);
        private readonly Rectangle tex_box3 = new Rectangle(406, 276, 403, 137);
        private readonly Rectangle tex_square1 = new Rectangle(10, 7, 395, 404);

        private readonly Rectangle tex_newGame = new Rectangle(213, 440, 330, 79);
        private readonly Rectangle tex_loadGame = new Rectangle(545, 441, 293, 80);
        private readonly Rectangle tex_options = new Rectangle(9, 667, 368, 78);
        private readonly Rectangle tex_saveGame = new Rectangle(9, 748, 424, 79);
        private readonly Rectangle tex_endGame = new Rectangle(206, 521, 238, 67);
        private readonly Rectangle tex_zurueck = new Rectangle(10, 822, 202, 82);
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
            if (AnimationProgress <= 0)
                return;

            _spriteBatch.Begin();

            if (CurrentState == State.Default)
            {
                _spriteBatch.Draw(texture, animate(scale(box_newGameBox, windowSize), new Vector2(windowSize.X, box_newGameBox.Y / 1000f * windowSize.Y)), tex_box1, Color.White);
                _spriteBatch.Draw(texture, animate(scale(box_newGameText, windowSize), new Vector2(windowSize.X, box_newGameText.Y / 1000f * windowSize.Y)), tex_newGame, Color.White);

                _spriteBatch.Draw(texture, animate(scale(box_loadGameBox, windowSize), new Vector2(windowSize.X, box_loadGameBox.Y / 1000f * windowSize.Y)), tex_box2, Color.White);
                _spriteBatch.Draw(texture, animate(scale(box_loadGameText, windowSize), new Vector2(windowSize.X, box_loadGameText.Y / 1000f * windowSize.Y)), tex_loadGame, Color.White);

                _spriteBatch.Draw(texture, animate(scale(box_saveGameBox, windowSize), new Vector2(windowSize.X, box_saveGameBox.Y / 1000f * windowSize.Y)), tex_box3, Color.White);
                if (canSave)
                    _spriteBatch.Draw(texture, animate(scale(box_saveGameText, windowSize), new Vector2(windowSize.X, box_saveGameText.Y / 1000f * windowSize.Y)), tex_saveGame, Color.White);

                _spriteBatch.Draw(texture, animate(scale(box_emptyBox, windowSize), new Vector2(windowSize.X, box_emptyBox.Y / 1000f * windowSize.Y)), tex_box2, Color.White);

                _spriteBatch.Draw(texture, animate(scale(box_endGameBox, windowSize), new Vector2(windowSize.X, box_endGameBox.Y / 1000f * windowSize.Y)), tex_box1, Color.White);
                _spriteBatch.Draw(texture, animate(scale(box_endGameText, windowSize), new Vector2(windowSize.X, box_endGameText.Y / 1000f * windowSize.Y)), tex_endGame, Color.White);
            }
            else if (CurrentState == State.Load || CurrentState == State.Save)
            {
                string _path1 = Path.Combine(Environment.CurrentDirectory, "save", "slot1");
                if (Directory.Exists(_path1))
                {
                    _spriteBatch.Draw(texture, animate(scale(box_newGameBox, windowSize), new Vector2(windowSize.X, box_newGameBox.Y / 1000f * windowSize.Y)), tex_box1, Color.White);
                    _spriteBatch.Draw(texture, animate(scale(box_slot1, windowSize), new Vector2(windowSize.X, box_slot1.Y / 1000f * windowSize.Y)), tex_slot1, Color.White);
                    DateTime _lastWriteTime = File.GetLastWriteTime(Path.Combine(_path1, "state.conf"));
                    string _lastWriteString = _lastWriteTime.ToShortDateString() + " - " + _lastWriteTime.ToLongTimeString();
                    Vector2 _size = font.MeasureString(_lastWriteString);
                    Vector2 _position = new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, 140 / 1000f * windowSize.Y);
                    _spriteBatch.DrawString(font, _lastWriteString,
                        animate(_position, new Vector2(windowSize.X, _position.Y)), Color.Black);
                }
                else
                {
                    _spriteBatch.Draw(texture, animate(scale(box_newGameBox, windowSize), new Vector2(windowSize.X, box_newGameBox.Y / 1000f * windowSize.Y)), tex_box1, new Color(0.8f, 0.8f, 0.8f));
                    _spriteBatch.Draw(texture, animate(scale(box_slot1, windowSize), new Vector2(windowSize.X, box_slot1.Y / 1000f * windowSize.Y)), tex_slot1, new Color(0.8f, 0.8f, 0.8f));
                    Vector2 _size = font.MeasureString("Leer");
                    Vector2 _position = new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, 140 / 1000f * windowSize.Y);
                    _spriteBatch.DrawString(font, "Leer",
                        animate(_position, new Vector2(windowSize.X, _position.Y)), Color.Black);
                }

                string _path2 = Path.Combine(Environment.CurrentDirectory, "save", "slot2");
                if (Directory.Exists(_path2))
                {
                    _spriteBatch.Draw(texture, animate(scale(box_loadGameBox, windowSize), new Vector2(windowSize.X, box_loadGameBox.Y / 1000f * windowSize.Y)), tex_box2, Color.White);
                    _spriteBatch.Draw(texture, animate(scale(box_slot2, windowSize), new Vector2(windowSize.X, box_slot2.Y / 1000f * windowSize.Y)), tex_slot2, Color.White);
                    DateTime _lastWriteTime = File.GetLastWriteTime(Path.Combine(_path2, "state.conf"));
                    string _lastWriteString = _lastWriteTime.ToShortDateString() + " - " + _lastWriteTime.ToLongTimeString();
                    Vector2 _size = font.MeasureString(_lastWriteString);
                    _spriteBatch.DrawString(font, _lastWriteString,
                        new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, (200 + 140) / 1000f * windowSize.Y), Color.Black);
                }
                else
                {
                    _spriteBatch.Draw(texture, animate(scale(box_loadGameBox, windowSize), new Vector2(windowSize.X, box_loadGameBox.Y / 1000f * windowSize.Y)), tex_box2, new Color(0.8f, 0.8f, 0.8f));
                    _spriteBatch.Draw(texture, animate(scale(box_slot2, windowSize), new Vector2(windowSize.X, box_slot2.Y / 1000f * windowSize.Y)), tex_slot2, new Color(0.8f, 0.8f, 0.8f));
                    Vector2 _size = font.MeasureString("Leer");
                    Vector2 _position = new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, (200 + 140) / 1000f * windowSize.Y);
                    _spriteBatch.DrawString(font, "Leer",
                        animate(_position, new Vector2(windowSize.X, _position.Y)), Color.Black);
                }

                string _path3 = Path.Combine(Environment.CurrentDirectory, "save", "slot3");
                if (Directory.Exists(_path3))
                {
                    _spriteBatch.Draw(texture, animate(scale(box_saveGameBox, windowSize), new Vector2(windowSize.X, box_saveGameBox.Y / 1000f * windowSize.Y)), tex_box3, Color.White);
                    _spriteBatch.Draw(texture, animate(scale(box_slot3, windowSize), new Vector2(windowSize.X, box_slot3.Y / 1000f * windowSize.Y)), tex_slot3, Color.White);
                    DateTime _lastWriteTime = File.GetLastWriteTime(Path.Combine(_path3, "state.conf"));
                    string _lastWriteString = _lastWriteTime.ToShortDateString() + " - " + _lastWriteTime.ToLongTimeString();
                    Vector2 _size = font.MeasureString(_lastWriteString);
                    _spriteBatch.DrawString(font, _lastWriteString,
                        new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, (400 + 140) / 1000f * windowSize.Y), Color.Black);
                }
                else
                {
                    _spriteBatch.Draw(texture, animate(scale(box_saveGameBox, windowSize), new Vector2(windowSize.X, box_saveGameBox.Y / 1000f * windowSize.Y)), tex_box3, new Color(0.8f, 0.8f, 0.8f));
                    _spriteBatch.Draw(texture, animate(scale(box_slot3, windowSize), new Vector2(windowSize.X, box_slot3.Y / 1000f * windowSize.Y)), tex_slot3, new Color(0.8f, 0.8f, 0.8f));
                    Vector2 _size = font.MeasureString("Leer");
                    Vector2 _position = new Vector2((618f + 382f / 2f) / 1000f * windowSize.X - _size.X / 2f, (400 + 140) / 1000f * windowSize.Y);
                    _spriteBatch.DrawString(font, "Leer",
                        animate(_position, new Vector2(windowSize.X, _position.Y)), Color.Black);
                }

                _spriteBatch.Draw(texture, animate(scale(box_emptyBox, windowSize), new Vector2(windowSize.X, box_emptyBox.Y / 1000f * windowSize.Y)), tex_box2, Color.White);

                _spriteBatch.Draw(texture, animate(scale(box_endGameBox, windowSize), new Vector2(windowSize.X, box_endGameBox.Y / 1000f * windowSize.Y)), tex_box1, Color.White);
                _spriteBatch.Draw(texture, animate(scale(box_endGameText, windowSize), new Vector2(windowSize.X, box_endGameText.Y / 1000f * windowSize.Y)), tex_zurueck, Color.White);
            }

            if (flags.HasFlag(GameFlags.Credits)) //Credits
            {
                SpriteFont S1 = content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_32);
                SpriteFont S2 = content.Load<SpriteFont>(ContentRessources.FONT_ARIAL_20);
                _spriteBatch.Draw(texture, scale(box_square, windowSize), tex_box1, Color.White);
                _spriteBatch.DrawString(S1, "Glueckwunsch, du hast den Schatz geborgen!", new Vector2(30, 100) * scalingFactor, Color.Black);
                _spriteBatch.DrawString(S1, "Ein Spiel von:", new Vector2(30, 150) * scalingFactor, Color.Black);
                _spriteBatch.DrawString(S1, "Tom Heimbrodt\nSimon Nestrowicz\nSarah Fuchs", new Vector2(30, 200) * scalingFactor, Color.Black);
                _spriteBatch.DrawString(S1, "Erstellt mit Unterstuetzung von Acagamics", new Vector2(30, 350) * scalingFactor, Color.Black);
                _spriteBatch.DrawString(S1, "Soundeffekte von:", new Vector2(30, 425) * scalingFactor, Color.Black);
                _spriteBatch.DrawString(S2, " https://www.freesound.org/people/fins/sounds/172205/ \n https://www.freesound.org/people/Davidsraba/sounds/347171/ \n https://www.freesound.org/people/deleted_user_877451/sounds/76376/ \n https://www.freesound.org/people/Under7dude/sounds/163441/ \n https://www.freesound.org/people/Raggaman/sounds/25515/ \n https://www.freesound.org/people/CrazyFrog249/sounds/161628/ \n https://www.freesound.org/people/Autistic%20Lucario/sounds/142608/ \n https://www.freesound.org/people/Isaac200000/sounds/188013/ \n https://www.freesound.org/people/RunnerPack/sounds/87045/ \n https://freesound.org/people/D%20W/sounds/143606/ \n https://freesound.org/people/BMacZero/sounds/94127/ \n https://freesound.org/people/GameAudio/sounds/220184/", new Vector2(30, 475) * scalingFactor, Color.Black);
            }

            if (!flags.HasFlag(GameFlags.GameLoaded))
                _spriteBatch.Draw(screenshot, scale(box_square, windowSize), Color.White);

            _spriteBatch.End();
        }

        private Rectangle scale(Rectangle _box, Point _size)
        {
            return new Rectangle((int)Math.Round(_box.X * _size.X / 1000.0),
                                 (int)Math.Round(_box.Y * _size.Y / 1000.0),
                                 (int)Math.Round(_box.Width * _size.X / 1000.0),
                                 (int)Math.Round(_box.Height * _size.Y / 1000.0));
        }

        private Rectangle animate(Rectangle _box, Vector2 _origin)
        {
            float oneMinusAnimationProgress = 1 - AnimationProgress;
            float animationFactor = 1 - oneMinusAnimationProgress * oneMinusAnimationProgress * oneMinusAnimationProgress * oneMinusAnimationProgress;
            return new Rectangle((int)Math.Round(_origin.X + (_box.X - _origin.X) * animationFactor),
                                 (int)Math.Round(_origin.Y + (_box.Y - _origin.Y) * animationFactor),
                                 _box.Width, _box.Height);
        }
        private Vector2 animate(Vector2 _box, Vector2 _origin)
        {
            float oneMinusAnimationProgress = 1 - AnimationProgress;
            float animationFactor = 1 - oneMinusAnimationProgress * oneMinusAnimationProgress * oneMinusAnimationProgress * oneMinusAnimationProgress;
            return new Vector2(_origin.X + (_box.X - _origin.X) * animationFactor,
                               _origin.Y + (_box.Y - _origin.Y) * animationFactor);

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
