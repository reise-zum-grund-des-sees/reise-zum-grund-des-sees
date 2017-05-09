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

		GameFlags GameMode;

        BasicEffect effect;
        WorldEditor editor;

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
            this.graphics.PreferredBackBufferHeight = 1080;
            this.graphics.PreferredBackBufferWidth = 1920;
            this.graphics.ApplyChanges();
            //this.graphics.ToggleFullScreen();
			// TODO: Add your initialization logic here
			GameMode = GameFlags.GameRunning;
			InputManager = new InputManager();
            GameState = new GameState(new World(16, 64, 16, 3, 3), new Player(Content, new Vector3(24, 32, 24)), new Camera(false));  //vorrübergehend GameState festsetzen
            GameState.World.GenerateTestWorld();
            GameState.World.GenerateVertices(GraphicsDevice);
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            editor = new WorldEditor(new Vector3(24, 32, 24), GraphicsDevice);

            effect = new BasicEffect(GraphicsDevice);
            

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
			InputEventArgs _args = InputManager.Update(Window.ClientBounds);

			if (GameMode.HasFlag(GameFlags.GameRunning) && !GameMode.HasFlag(GameFlags.EditorMode))
			{
				GameState.View _gameStateView = new GameState.View(GameState);

				UpdateDelegate _playerUpdate = GameState.Player.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
                UpdateDelegate _cameraUpdate = GameState.Camera.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
                // UpdateDelegate _worldUpdate = GameState.World.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
               
                _playerUpdate(ref GameState);
                _cameraUpdate(ref GameState);

                //_worldUpdate(ref GameState);
                for (int i = 0; i < Player.LeichteBlöcke.Count; i++)
                {
                    Player.LeichteBlöcke[i].Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
                }
            }
			if (GameMode.HasFlag(GameFlags.Menu))
			{
				MainMenu.Update(_args, this);
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

                    GameState = new GameState(new World(_dialog.SelectedPath), GameState.Player, GameState.Camera);
                    GameState.World.GenerateVertices(GraphicsDevice);
                }
                else if (kb.IsKeyDown(Keys.E) && keyPressedPause)
                {
                    GameMode ^= GameFlags.EditorMode;
                }
            }

            if ((kb.GetPressedKeys().Length == 0) || (kb.GetPressedKeys().Length == 1 && kb.IsKeyDown(Keys.LeftControl))) keyPressedPause = true;
            else keyPressedPause = false;

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here

			

			Matrix _viewMatrix = GameState.Camera.CalculateViewMatrix();
            Matrix _perspectiveMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), Window.ClientBounds.Width * 1f / Window.ClientBounds.Height, 1f, 50f);

            if (GameMode.HasFlag(GameFlags.EditorMode))
                renderer.WorldEditor(editor, ref _viewMatrix, ref _perspectiveMatrix);

            renderer.Player(GameState.Player, ref _viewMatrix, ref _perspectiveMatrix);
            renderer.World(GameState.World, ref _viewMatrix, ref _perspectiveMatrix);
            renderer.LeichterBlock(Player.LeichteBlöcke, ref _viewMatrix, ref _perspectiveMatrix);
            

            base.Draw(gameTime);
		}

		// Menu Callbacks

		public void StartNewGame()
		{
			throw new NotImplementedException();
		}

		public void LoadGame(string _path)
		{
			throw new NotImplementedException();
		}

		public void SaveGame(string _path)
		{
			throw new NotImplementedException();
		}

		public void ShowCredits()
		{
			throw new NotImplementedException();
		}

		public void ExitGame()
		{
			throw new NotImplementedException();
		}
	}
}
