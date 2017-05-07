﻿using System;
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

		MainMenu MainMenu;

		GameFlags GameMode;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			GameMode = GameFlags.Menu;
			InputManager = new InputManager();

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

			// TODO: use this.Content to load your game content here
			MainMenu = new MainMenu(this.Content);
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
		protected override void Update(GameTime gameTime)
		{
			InputEventArgs _args = InputManager.Update();

			/*if (GameMode.HasFlag(GameFlags.GameRunning))
			{
				GameState.View _gameStateView = new GameState.View(GameState);

				UpdateDelegate _cameraUpdate = GameState.Camera.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
				UpdateDelegate _playerUpdate = GameState.Player.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);
				UpdateDelegate _worldUpdate = GameState.World.Update(_gameStateView, _args, gameTime.ElapsedGameTime.TotalMilliseconds);

				_cameraUpdate(ref GameState);
				_playerUpdate(ref GameState);
				_worldUpdate(ref GameState);
			}
			else if (GameMode.HasFlag(GameFlags.Menu))
			{
				MainMenu.Update(_args, this);
			}*/

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

			/*

			Matrix m = GameState.Camera.CalculateViewMatrix;

			Render.Player(m, GameState.Player);
			Render.World(m, GameState.World);

			*/

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
