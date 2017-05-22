using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ReiseZumGrundDesSees
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game, IMenuCallback
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameState GameState;
        InputManager InputManager;

        Texture2D blocktexture;

        private Render renderer;

        MainMenu MainMenu;

        GameFlags __GameMode;
        GameFlags GameMode
        {
            get
            {
                return __GameMode;
            }
            set
            {
                __GameMode = value;
                DebugHelper.Log("GameMode changed to " + value);
            }
        }

        BasicEffect effect;
        WorldEditor editor;

        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.graphics.ToggleFullScreen();
            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.ApplyChanges();

            GameMode = GameFlags.Menu | GameFlags.Debug;

            // TEMP: remove to show main menu
            //StartNewGame();

            InputManager = new InputManager();
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            editor = new WorldEditor(new Vector3(24, 32, 24), GraphicsDevice, Content);

            effect = new BasicEffect(GraphicsDevice);
            this.IsMouseVisible = true;
            //Startposition in der Mitte, damit kein Out of Bounds Error erzeugt wird
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blocktexture = Content.Load<Texture2D>("blocktexture");
            font = Content.Load<SpriteFont>("font");

            // TODO: use this.Content to load your game content here
            MainMenu = new MainMenu(Content);
            renderer = new Render(GraphicsDevice, Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        bool keyPressedPause = true;
        protected override void Update(GameTime gameTime)
        {
            if (graphics.PreferredBackBufferHeight != Window.ClientBounds.Height ||
                graphics.PreferredBackBufferWidth != Window.ClientBounds.Width)
            {
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.ApplyChanges();
            }

            InputEventArgs _args = InputManager.Update(GameMode, Window.ClientBounds);
            DebugHelper.Log(_args.Events.ToString());

            if (GameMode.HasFlag(GameFlags.GameLoaded) && GameMode.HasFlag(GameFlags.GameRunning) && !GameMode.HasFlag(GameFlags.EditorMode))
            {
                GameState.View _gameStateView = new GameState.View(GameState);

                UpdateDelegate _playerUpdate = GameState.Player.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
                UpdateDelegate _cameraUpdate = GameState.Camera.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
                UpdateDelegate _worldUpdate = GameState.World.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);

                _playerUpdate(ref GameState);
                _cameraUpdate(ref GameState);
                _worldUpdate(ref GameState);

                for (int i = 0; i < Player.Blöcke.Count; i++)
                {
                    Player.Blöcke[i].Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
                }
            }
            if (GameMode.HasFlag(GameFlags.Menu))
            {
                MainMenu.Update(_args, this, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            }
            if (GameMode.HasFlag(GameFlags.EditorMode))
            {
                GameState.View _gameStateView = new GameState.View(GameState);
                UpdateDelegate _editorUpdate = editor.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
                _editorUpdate(ref GameState);
            }

            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.LeftControl))
            {
                if (kb.IsKeyDown(Keys.S))
                {
                    System.Windows.Forms.FolderBrowserDialog _dialog = new System.Windows.Forms.FolderBrowserDialog();
                    _dialog.SelectedPath = Environment.CurrentDirectory;
                    _dialog.ShowDialog();

                    GameState.World.Save(_dialog.SelectedPath);
                }
                else if (kb.IsKeyDown(Keys.L))
                {
                    System.Windows.Forms.FolderBrowserDialog _dialog = new System.Windows.Forms.FolderBrowserDialog();
                    _dialog.SelectedPath = Environment.CurrentDirectory;
                    _dialog.ShowDialog();

                    GameState = new GameState(new World(_dialog.SelectedPath, GraphicsDevice), GameState.Player, GameState.Camera);
                    GameState.World.GenerateVertices(GraphicsDevice);
                }
                else if (kb.IsKeyDown(Keys.E) && keyPressedPause)
                {
                    GameMode ^= GameFlags.EditorMode;
                }
            }
            else if (kb.IsKeyDown(Keys.Escape) && keyPressedPause)
            {
                GameMode ^= (GameFlags.Menu | GameFlags.GameRunning);
            }

            if ((kb.GetPressedKeys().Length == 0) || (kb.GetPressedKeys().Length == 1 && kb.IsKeyDown(Keys.LeftControl))) keyPressedPause = true;
            else keyPressedPause = false;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            DebugHelper.Information.TotalFrameCount++;
            DebugHelper.Information.FPS = DebugHelper.Information.FPS * 0.9 + 0.1 / gameTime.ElapsedGameTime.TotalSeconds;
            DebugHelper.Information.TotalGameTime = gameTime.TotalGameTime;
            DebugHelper.Information.PlayerPosition = GameState.Player?.Position ?? Vector3.Zero;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

            if (GameMode.HasFlag(GameFlags.GameLoaded))
            {
                Matrix _viewMatrix = GameState.Camera.CalculateViewMatrix();
                Matrix _perspectiveMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), Window.ClientBounds.Width * 1f / Window.ClientBounds.Height, 1f, 1000f);

                if (GameMode.HasFlag(GameFlags.EditorMode))
                    renderer.WorldEditor(editor, ref _viewMatrix, ref _perspectiveMatrix);

                renderer.PlayerR(GameState.Player, ref _viewMatrix, ref _perspectiveMatrix);
                renderer.World(GameState.World, ref _viewMatrix, ref _perspectiveMatrix, out DebugHelper.Information.RenderedWorldVertices, out DebugHelper.Information.RenderedWorldChunks);
                renderer.LeichterBlock(Player.Blöcke, ref _viewMatrix, ref _perspectiveMatrix);
                renderer.LeverR(Lever.LeverList, ref _viewMatrix, ref _perspectiveMatrix);
            }

            if (GameMode.HasFlag(GameFlags.Menu))
                MainMenu.Render(spriteBatch);

            if (GameMode.HasFlag(GameFlags.Debug))
                DebugHelper.RenderOverlay(spriteBatch, font);

            base.Draw(gameTime);
        }

        // Menu Callbacks

        public void StartNewGame()
        {
            GameMode |= GameFlags.GameRunning | GameFlags.GameLoaded;
            GameMode &= ~GameFlags.Menu;
            World _world = CreateWorld();
            GameState = new GameState(_world, new Player(Content, new Vector3(_world.SpawnPosX, _world.SpawnPosY, _world.SpawnPosZ)), new Camera());
        }

        private World CreateWorld()
        {
            World w = new World(16, 64, 16, 16, 16, new Vector3(24, 32, 24), GraphicsDevice);
            w.GenerateTestWorld();
            return w;
        }

        public void LoadGame(string _path)
        {
            GameMode |= GameFlags.GameRunning | GameFlags.GameLoaded;
            GameMode &= ~GameFlags.Menu;
            World w = new World(_path, GraphicsDevice);
            GameState = new GameState(w, new Player(Content, new Vector3(w.SpawnPosX, w.SpawnPosY, w.SpawnPosZ)), new Camera());
        }

        public void SaveGame(string _path)
        {
            GameState.World.Save(_path);
        }

        public void ShowCredits()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.MessageBox.Show(
                "Tom Heimbrodt\nSimon Nestrowicz\nSarah Fuchs",
                "Credits", System.Windows.Forms.MessageBoxButtons.OK);
        }

        public void ExitGame()
        {
            Exit();
        }
    }
}
