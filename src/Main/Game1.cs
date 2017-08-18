using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace ReiseZumGrundDesSees
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class Game1 : Game, IMenuCallback
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public GameState GameState;
        InputManager InputManager;

        List<IRenderable> initializeList = new List<IRenderable>();

        Texture2D blocktexture;

        private List<IRenderable> worldRenderables = new List<IRenderable>();
        private List<IRenderable> otherRenderables = new List<IRenderable>();

        MainMenu MainMenu;
        IGamer IGamer;

        GameFlags __GameFlags;
        GameFlags GameFlags
        {
            get
            {
                return __GameFlags;
            }
            set
            {
                if ((value & GameFlags.Fullscreen) != (__GameFlags & GameFlags.Fullscreen))
                {
                    graphics.IsFullScreen = (value & GameFlags.Fullscreen) != 0;
                    graphics.ApplyChanges();
                }
                if (value.HasFlag(GameFlags.EditorMode) && GameState.Camera != null)
                    GameState.Camera.Center = editor;
                else if (value.HasFlag(GameFlags.GameRunning) && GameState.Camera != null)
                    GameState.Camera.Center = GameState.Player;

                __GameFlags = value;
                DebugHelper.Log("GameMode changed to " + value);
            }
        }

        WorldEditor editor;
        SpriteFont font;

        MovingBlock testBlock;
       
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
            /*
            testBlock = new MovingBlock(new List<Vector3>{
                new Vector3(24, 34, 24),
                new Vector3(24, 35, 24),
                new Vector3(27, 34, 25),
                new Vector3(27, 33, 25)
            });
            */
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //this.graphics.ToggleFullScreen();
            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this.graphics.ApplyChanges();

            GameFlags = GameFlags.Menu | GameFlags.Debug;

            Window.AllowUserResizing = true;

            // TEMP: remove to show main menu
            //StartNewGame();

            InputManager = new InputManager();
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            editor = new WorldEditor(new Vector3(24, 32, 24), Content);
            initializeList.Add(editor);
            otherRenderables.Add(editor);

            this.IsMouseVisible = true;

            //Startposition in der Mitte, damit kein Out of Bounds Error erzeugt wird
            /*
            Enemy a = new Enemy(new Vector3(20, 32, 20), Enemy.Art.MandS); //Create Test Enemy
            Enemy b = new Enemy(Content, new Vector3(30, 32, 30), Enemy.Art.Shooting); //Create Test Enemy
            Enemy c = new Enemy(Content, new Vector3(30, 32, 25), Enemy.Art.Moving); //Create Test Enemy
            */
            SoundEffect.MasterVolume = 0.1f; //Diesen Paramenter sollte man in den Optionen einstellen Können
                                             //initializeList.Add(testBlock);

            //add Aufsammelbare Player Blöcke
            GetPlayerBlock.GetPlayerBlockList.Add(new GetPlayerBlock(new Vector3(169.5f, 34, 216.5f), 0));
            GetPlayerBlock.GetPlayerBlockList.Add(new GetPlayerBlock(new Vector3(176.5f, 36, 166.5f), 1));
            GetPlayerBlock.GetPlayerBlockList.Add(new GetPlayerBlock(new Vector3(136.5f, 39, 234.5f), 2));
            initializeList.Add(GetPlayerBlock.GetPlayerBlockList[0]);
            initializeList.Add(GetPlayerBlock.GetPlayerBlockList[1]);
            initializeList.Add(GetPlayerBlock.GetPlayerBlockList[2]);
            for (int i = 0; i < Enemy.EnemyList.Count; i++)
                initializeList.Add(Enemy.EnemyList[i]);
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

            blocktexture = Content.Load<Texture2D>(ReiseZumGrundDesSees.Content.TEXTURE_BLOCK);
            font = Content.Load<SpriteFont>(ReiseZumGrundDesSees.Content.FONT_ARIAL_20);

            // TODO: use this.Content to load your game content here
            MainMenu = new MainMenu(Content, this);
            IGamer = new IGamer(Content);
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
            if (!GameFlags.HasFlag(GameFlags.Fullscreen))
                if (graphics.PreferredBackBufferHeight != Window.ClientBounds.Height ||
                    graphics.PreferredBackBufferWidth != Window.ClientBounds.Width)
                {
                    graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                    graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                    graphics.ApplyChanges();
                }
                else
                if (graphics.PreferredBackBufferHeight != GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height ||
                    graphics.PreferredBackBufferWidth != GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.ApplyChanges();
                }


            InputEventArgs _args = InputManager.Update(GameFlags, Window.ClientBounds);
            GameState.View _gameStateView = new GameState.View(GameState);
            List<UpdateDelegate> _updateList = new List<UpdateDelegate>();

            _updateList.Add(GameState.Player?.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
            _updateList.Add(GameState.Camera?.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
            _updateList.Add(GameState.World?.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
            _updateList.Add(editor.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
           // _updateList.Add(testBlock.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
           // _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
  
            if (GameFlags.HasFlag(GameFlags.GameRunning))
                for (int i = 0; i < Enemy.EnemyList.Count; i++)//Update Enemies
                {
                    _updateList.Add(Enemy.EnemyList[i].Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
                    _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
                }
            if (GameFlags.HasFlag(GameFlags.GameRunning))
                for (int i = 0; i < Geschoss.GeschossList.Count; i++)//Update Geschosse
                {
                    _updateList.Add(Geschoss.GeschossList[i].Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
                    _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
                }

            //foreach (UpdateDelegate u in _updateList)
                //u?.Invoke(ref GameState);

            if (GameFlags.HasFlag(GameFlags.Menu))
                MainMenu.Update(_args, GameState, GameFlags, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

            if (GameFlags.HasFlag(GameFlags.GameRunning))
                IGamer.Update(_args, GameState, GameFlags, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.LeftControl))
            {
                if (kb.IsKeyDown(Keys.E) && keyPressedPause)
                {
                    GameFlags ^= GameFlags.EditorMode;
                }
            }
            else if (kb.IsKeyDown(Keys.F1) && keyPressedPause)
                GameFlags ^= GameFlags.Debug;
            else if (kb.IsKeyDown(Keys.F11) && keyPressedPause)
                GameFlags ^= GameFlags.Fullscreen;
            else if (kb.IsKeyDown(Keys.Escape) && keyPressedPause)
            {
                GameFlags ^= (GameFlags.Menu | GameFlags.GameRunning);
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
            DebugHelper.Information.FPS = 1 / gameTime.ElapsedGameTime.TotalSeconds;// DebugHelper.Information.FPS * 0.9 + 0.1 / gameTime.ElapsedGameTime.TotalSeconds;
            DebugHelper.Information.TotalGameTime = gameTime.TotalGameTime;
            DebugHelper.Information.PlayerPosition = GameState.Player?.Position ?? Vector3.Zero;
            DebugHelper.Information.EditorCursorPosition = editor?.Position ?? Vector3.Zero;
            DebugHelper.Information.CameraRotation = GameState.Camera?.Azimuth ?? 0;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

            foreach (var i in initializeList)
                i.Initialize(GraphicsDevice, Content);
            initializeList.Clear();

            if (GameFlags.HasFlag(GameFlags.GameLoaded))
            {
                Matrix _viewMatrix = GameState.Camera.CalculateViewMatrix();
                Matrix _perspectiveMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), Window.ClientBounds.Width * 1f / Window.ClientBounds.Height, 1f, 1000f);

                GameState.Player.Render(GameFlags, _viewMatrix, _perspectiveMatrix, GraphicsDevice);

               
                foreach (var _renderable in worldRenderables)
                    _renderable.Render(GameFlags, _viewMatrix, _perspectiveMatrix, GraphicsDevice);
                foreach (var _renderable in otherRenderables)
                    _renderable.Render(GameFlags, _viewMatrix, _perspectiveMatrix, GraphicsDevice);

                //  testBlock.Render(GameFlags, _viewMatrix, _perspectiveMatrix, GraphicsDevice);

              

                IGamer.Render(spriteBatch);
            }

            if (GameFlags.HasFlag(GameFlags.Menu))
                MainMenu.Render(spriteBatch);

            if (GameFlags.HasFlag(GameFlags.Debug))
                DebugHelper.RenderOverlay(spriteBatch, font);

            base.Draw(gameTime);
        }

        // Menu Callbacks

        public void StartNewGame()
        {
            RenderableWorld _world = CreateWorld();

            worldRenderables.Clear();
            worldRenderables.Add(_world);
            foreach (var _renderable in worldRenderables)
                initializeList.Add(_renderable);

            GameState = new GameState(_world, new Player(new Vector3(_world.SpawnPos.X, _world.SpawnPos.Y, _world.SpawnPos.Z)), new Camera());
            initializeList.Add(GameState.Player);

            GameFlags |= GameFlags.GameRunning | GameFlags.GameLoaded;
            GameFlags &= ~GameFlags.Menu;
        }

        private RenderableWorld CreateWorld()
        {
            RenderableWorld w = new RenderableWorld(16, 128, 16, 128, 128, new Vector3(24, 32, 24), Content);
            w.GenerateTestWorld();
            return w;
        }

        public void LoadGame(string _path)
        {
            GameState = GameState.Load(_path);

            worldRenderables.Clear();
            worldRenderables.Add(GameState.World as IRenderable);

            initializeList.Clear();
            foreach (var _renderable in worldRenderables)
                initializeList.Add(_renderable);

            initializeList.Add(GameState.Player);

            for (int i = 0; i < Enemy.EnemyList.Count; i++)
                initializeList.Add(Enemy.EnemyList[i]);
            GameFlags |= GameFlags.GameRunning | GameFlags.GameLoaded;
            GameFlags &= ~GameFlags.Menu;
        }

        public void SaveGame(string _path)
        {
            GameState.Save(_path);
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
