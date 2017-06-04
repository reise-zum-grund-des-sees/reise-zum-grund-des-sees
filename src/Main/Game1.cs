using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReiseZumGrundDesSees.State;
using ReiseZumGrundDesSees.Entities;

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
            editor.Initialize(GraphicsDevice);
            otherRenderables.Add(editor);
            
            this.IsMouseVisible = true;
            //Startposition in der Mitte, damit kein Out of Bounds Error erzeugt wird
            Enemy a = new Enemy(Content, new Vector3(20,34,20), Enemy.Art.Jumping); //Create Test Enemy
            //Enemy b = new Enemy(Content, new Vector3(30, 32, 30), Enemy.Art.Climbing); //Create Test Enemy
            //Enemy c = new Enemy(Content, new Vector3(30, 32, 25), Enemy.Art.Moving); //Create Test Enemy
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
            IGamer = new IGamer(Content);
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
            _updateList.Add(GameState.Camera?.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList.Add(GameState.World?.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList.Add(editor.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
     
        if(GameFlags.HasFlag(GameFlags.GameRunning))
                for (int i = 0; i < Enemy.EnemyList.Count; i++)//Update Enemies
                    _updateList.Add(Enemy.EnemyList[i].Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
           

            foreach (UpdateDelegate u in _updateList)
                u?.Invoke(ref GameState);

            if (GameFlags.HasFlag(GameFlags.Menu))
                MainMenu.Update(_args, this, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

            if (GameFlags.HasFlag(GameFlags.GameRunning))
                IGamer.Update(_args, new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), GameState);
          
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
            DebugHelper.Information.CameraRotation = GameState.Camera?.Angle ?? 0;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

            if (GameFlags.HasFlag(GameFlags.GameLoaded))
            {
                Matrix _viewMatrix = GameState.Camera.CalculateViewMatrix();
                Matrix _perspectiveMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), Window.ClientBounds.Width * 1f / Window.ClientBounds.Height, 1f, 1000f);
                renderer.PlayerR((Player)GameState.Player, ref _viewMatrix, ref _perspectiveMatrix);
                renderer.LeichterBlock((List<PlayerBlock>)GameState.Player.Blocks, ref _viewMatrix, ref _perspectiveMatrix);

                for (int i = 0; i < Enemy.EnemyList.Count; i++)//Draw Enemies
                   Enemy.EnemyList[i].Render(GameFlags, _viewMatrix, _perspectiveMatrix);

                foreach (var _renderable in worldRenderables)
                    _renderable.Render(GameFlags, _viewMatrix, _perspectiveMatrix);
                foreach (var _renderable in otherRenderables)
                    _renderable.Render(GameFlags, _viewMatrix, _perspectiveMatrix);

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
                _renderable.Initialize(GraphicsDevice);

            GameState = new GameState(_world, new Player(Content, new Vector3(_world.SpawnPosX, _world.SpawnPosY, _world.SpawnPosZ)), new Camera());

            GameFlags |= GameFlags.GameRunning | GameFlags.GameLoaded;
            GameFlags &= ~GameFlags.Menu;
        }

        private RenderableWorld CreateWorld()
        {
            RenderableWorld w = new RenderableWorld(16, 64, 16, 16, 16, new Vector3(24, 32, 24), Content);
            w.GenerateTestWorld();
            return w;
        }

        public void LoadGame(string _path)
        {
            RenderableWorld w = new RenderableWorld(_path, Content);

            worldRenderables.Clear();
            worldRenderables.Add(w);
            foreach (var _renderable in worldRenderables)
                _renderable.Initialize(GraphicsDevice);

            GameState = new GameState(w, new Player(Content, new Vector3(w.SpawnPosX, w.SpawnPosY, w.SpawnPosZ)), new Camera());

            GameFlags |= GameFlags.GameRunning | GameFlags.GameLoaded;
            GameFlags &= ~GameFlags.Menu;
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
