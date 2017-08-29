using System;
using System.Linq;
using System.Diagnostics;
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

        Texture2D texture;

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
                this.IsMouseVisible = !value.HasFlag(GameFlags.GameRunning) || value.HasFlag(GameFlags.Menu);

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

        RenderTarget2D nearShadowMap, farShadowMap;
        Matrix nearLightMatrix, farLightMatrix;

        ShadowEffect shadowEffect;
        DefaultEffect preShadowEffect;

        Treasure Truhe;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreparingDeviceSettings += (_sender, _preparingDeviceSettingsEventArgs) =>
            {
                _preparingDeviceSettingsEventArgs.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.HiDef;
                _preparingDeviceSettingsEventArgs.GraphicsDeviceInformation.PresentationParameters.DepthStencilFormat = DepthFormat.Depth16;
            };
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            this.graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //this.graphics.SynchronizeWithVerticalRetrace = false;
            //this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 1);

            /*this.graphics.PreferMultiSampling = true;
            GraphicsDevice.PresentationParameters.MultiSampleCount = 2;
            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.CullClockwiseFace,
                MultiSampleAntiAlias = true,
            };*/
            this.graphics.ApplyChanges();

            nearShadowMap = new RenderTarget2D(GraphicsDevice, 512, 512, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            farShadowMap = new RenderTarget2D(GraphicsDevice, 512, 512, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            GameFlags = GameFlags.Menu | GameFlags.Debug;

            Window.AllowUserResizing = true;

            InputManager = new InputManager();
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            editor = new WorldEditor(new Vector3(24, 32, 24), Content);
            editor.Initialize(GraphicsDevice, Content);

            this.IsMouseVisible = true;

            SoundEffect.MasterVolume = 0.1f; //Diesen Paramenter sollte man in den Optionen einstellen Können

            //add Aufsammelbare Player Blöcke
            GetPlayerBlock.GetPlayerBlockList.Add(new GetPlayerBlock(new Vector3(169.5f, 34, 216.5f), 0));
            GetPlayerBlock.GetPlayerBlockList.Add(new GetPlayerBlock(new Vector3(176.5f, 36, 166.5f), 1));
            GetPlayerBlock.GetPlayerBlockList.Add(new GetPlayerBlock(new Vector3(136.5f, 39, 234.5f), 2));
            GetPlayerBlock.GetPlayerBlockList.Add(new GetPlayerBlock(new Vector3(305.5f, 43, 187.5f), 0));
            GetPlayerBlock.GetPlayerBlockList.Add(new GetPlayerBlock(new Vector3(178.5f, 42, 363.5f), 1));
            GetPlayerBlock.GetPlayerBlockList.Add(new GetPlayerBlock(new Vector3(294.5f, 52, 359.5f), 2));
            initializeList.Add(GetPlayerBlock.GetPlayerBlockList[0]);
            initializeList.Add(GetPlayerBlock.GetPlayerBlockList[1]);
            initializeList.Add(GetPlayerBlock.GetPlayerBlockList[2]);
            initializeList.Add(GetPlayerBlock.GetPlayerBlockList[3]);
            initializeList.Add(GetPlayerBlock.GetPlayerBlockList[4]);
            initializeList.Add(GetPlayerBlock.GetPlayerBlockList[5]);

            for (int i = 0; i < Enemy.EnemyList.Count; i++)
                initializeList.Add(Enemy.EnemyList[i]);

            Truhe = new Treasure(new Vector3(150f,33f,200f));
            initializeList.Add(Truhe);

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

            texture = Content.Load<Texture2D>(ReiseZumGrundDesSees.ContentRessources.TEXTURE);
            font = Content.Load<SpriteFont>(ReiseZumGrundDesSees.ContentRessources.FONT_ARIAL_20);

            shadowEffect = new ShadowEffect(Content);
            preShadowEffect = new DefaultEffect(Content.Load<Effect>(ContentRessources.EFFECT_PRESHADOW));

            // TODO: use this.Content to load your game content here
            MainMenu = new MainMenu(texture, Content, this);
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
        Stopwatch stopwatch = new Stopwatch();
        bool keyPressedPause = true;
        protected override void Update(GameTime gameTime)
        {
            //CollisionDetector.COUNTER = 0;
            if (GameFlags.HasFlag(GameFlags.Debug))
                stopwatch.Start();

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

            GameState.CollisionDetector?.Update();


            InputEventArgs _args = InputManager.Update(GameFlags, Window.ClientBounds);
            GameState.View _gameStateView = new GameState.View(GameState);
            List<UpdateDelegate> _updateList = new List<UpdateDelegate>();

            _updateList.Add(GameState.Player?.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
            _updateList.Add(GameState.Camera?.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
            //stopwatch.Restart();
            _updateList.Add(GameState.World?.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
            //stopwatch.Stop();
            //if (GameFlags.HasFlag(GameFlags.GameRunning))
                //Console.WriteLine("World update: " + stopwatch.Elapsed.ToString() + " FPS: " + (1.0 / stopwatch.Elapsed.TotalSeconds).ToString());
            _updateList.Add(editor.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            _updateList[_updateList.Count - 1]?.Invoke(ref GameState);
            // _updateList.Add(testBlock.Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
            // _updateList[_updateList.Count - 1]?.Invoke(ref GameState);

            if (GameFlags.HasFlag(GameFlags.GameRunning))
            {
                //double _sum = 0;
                for (int i = 0; i < Enemy.EnemyList.Count; i++)//Update Enemies
                {
                    //stopwatch.Restart();
                    _updateList.Add(Enemy.EnemyList[i].Update(_gameStateView, GameFlags, _args, gameTime.ElapsedGameTime.TotalMilliseconds));
                    _updateList[_updateList.Count - 1]?.Invoke(ref GameState);

                    //stopwatch.Stop();
                    //_sum += stopwatch.Elapsed.TotalSeconds;
                    //Console.WriteLine("Enemy " + i + " Art: " + Enemy.EnemyList[i].Gegnerart.ToString() + "-" + stopwatch.Elapsed.ToString() + " FPS: " + (1.0 / stopwatch.Elapsed.TotalSeconds) + " sum: " + _sum);
                }
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

            if (kb.IsKeyDown(Keys.OemComma))
            {
                shadowRotation += 0.0001f;
                Console.WriteLine(shadowRotation);
            }
            else if (kb.IsKeyDown(Keys.OemPeriod))
            {
                shadowRotation -= 0.0001f;
                Console.WriteLine(shadowRotation);
            }

            if (GameFlags.HasFlag(GameFlags.GameLoaded))
            {
                nearLightMatrix =
                    Matrix.CreateLookAt(new Vector3((float)Math.Floor(GameState.Player.Position.X) - 10,
                                                    GameState.World.Blocks.Size.Y + 1,
                                                    (float)Math.Floor(GameState.Player.Position.Z) - 10),
                                        new Vector3((float)Math.Floor(GameState.Player.Position.X),
                                                    (float)Math.Floor(GameState.Player.Position.Y),
                                                    (float)Math.Floor(GameState.Player.Position.Z)),
                                        Vector3.Forward) *
                    Matrix.CreateOrthographic(30, 30, 0, GameState.World.Blocks.Size.Y);

                farLightMatrix =
                    Matrix.CreateTranslation(-GameState.Camera.Center.Position) *
                    //Matrix.CreateRotationY(MathHelper.PiOver4 + MathHelper.Pi + GameState.Camera.Azimuth) *
                    Matrix.CreateRotationY(-0.011f) * // magic number. gives best shadow results.
                    Matrix.CreateTranslation(GameState.Camera.Center.Position) *
                    //Matrix.CreateTranslation(-50f, 0, -50f) *
                    Matrix.CreateLookAt(new Vector3((float)Math.Floor(GameState.Player.Position.X) - (float)(Math.Sin(MathHelper.PiOver4 + 0.011f) * Math.Sqrt(200)),
                                                    GameState.World.Blocks.Size.Y + 1,
                                                    (float)Math.Floor(GameState.Player.Position.Z) - (float)(Math.Cos(MathHelper.PiOver4 + 0.011f) * Math.Sqrt(200))),
                                        new Vector3((float)Math.Floor(GameState.Player.Position.X),
                                                    (float)Math.Floor(GameState.Player.Position.Y),
                                                    (float)Math.Floor(GameState.Player.Position.Z)),
                                        Vector3.Forward) *
                    Matrix.CreateOrthographic(120, 120, 0, GameState.World.Blocks.Size.Y);
            }

            stopwatch.Stop();


            DebugHelper.Information.updateTime = stopwatch.Elapsed.TotalSeconds;

            stopwatch.Reset();

            base.Update(gameTime);
        }

        float shadowRotation = 0f;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            DebugHelper.Information.FPS = 1000.0 / gameTime.ElapsedGameTime.TotalMilliseconds;
            DebugHelper.Information.RenderedOtherVertices = 0;

            if (GameFlags.HasFlag(GameFlags.Debug))
                stopwatch.Start();

            
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

            foreach (var i in initializeList)
                i.Initialize(GraphicsDevice, Content);
            initializeList.Clear();

            // Draw Shadows ----------------------------------------------------------------------------------------------------
            GraphicsDevice.SetRenderTarget(nearShadowMap);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Red, 1f, 0);

            if (GameFlags.HasFlag(GameFlags.GameLoaded))
            {
                preShadowEffect.ViewMatrix = Matrix.Identity;
                preShadowEffect.ProjectionMatrix = nearLightMatrix;
                GameState.Player.Render(GameFlags, preShadowEffect, GraphicsDevice);
                GameState.World.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in GameState.World.specialBlocks)
                    if (Vector2.Distance(
                            new Vector2(_obj.Key.X, _obj.Key.Z),
                            new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                        < GameState.World.viewDistance)

                        _obj.Value.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in GameState.World.Objects)
                    if (Vector2.Distance(
                            new Vector2(_obj.Position.X, _obj.Position.Z),
                            new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                        < GameState.World.viewDistance)

                        _obj.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in Enemy.EnemyList) //Draw Enemy
                    if (Vector2.Distance(
                           new Vector2(_obj.Position.X, _obj.Position.Z),
                           new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                       < GameState.World.viewDistance)

                        _obj.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in Geschoss.GeschossList)//Draw Geschosse
                    if (Vector2.Distance(
                         new Vector2(_obj.Position.X, _obj.Position.Z),
                         new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                     < GameState.World.viewDistance)

                        _obj.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in GetPlayerBlock.GetPlayerBlockList)//Draw GetPlayerBlock
                    if (Vector2.Distance(
                     new Vector2(_obj.Position.X, _obj.Position.Z),
                     new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                 < GameState.World.viewDistance)

                        _obj.Render(GameFlags, preShadowEffect, GraphicsDevice);

                if (Vector2.Distance(
                     new Vector2(Truhe.Position.X, Truhe.Position.Z),
                     new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                 < GameState.World.viewDistance)

                        Truhe.Render(GameFlags, preShadowEffect, GraphicsDevice);

                shadowEffect.NearLightMatrix = nearLightMatrix;
                shadowEffect.NearLightTexture = nearShadowMap;
            }
            
            GraphicsDevice.SetRenderTarget(farShadowMap);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Red, 1f, 0);

            if (GameFlags.HasFlag(GameFlags.GameLoaded))
            {
                preShadowEffect.ViewMatrix = Matrix.Identity;
                preShadowEffect.ProjectionMatrix = farLightMatrix;
                GameState.Player.Render(GameFlags, preShadowEffect, GraphicsDevice);
                GameState.World.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in GameState.World.specialBlocks)
                    if (Vector2.Distance(
                            new Vector2(_obj.Key.X, _obj.Key.Z),
                            new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                        < GameState.World.viewDistance)

                        _obj.Value.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in GameState.World.Objects)
                    if (Vector2.Distance(
                            new Vector2(_obj.Position.X, _obj.Position.Z),
                            new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                        < GameState.World.viewDistance)

                        _obj.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in Enemy.EnemyList) //Draw Enemy
                    if (Vector2.Distance(
                           new Vector2(_obj.Position.X, _obj.Position.Z),
                           new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                       < GameState.World.viewDistance)

                        _obj.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in Geschoss.GeschossList)//Draw Geschosse
                    if (Vector2.Distance(
                         new Vector2(_obj.Position.X, _obj.Position.Z),
                         new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                     < GameState.World.viewDistance)

                        _obj.Render(GameFlags, preShadowEffect, GraphicsDevice);

                foreach (var _obj in GetPlayerBlock.GetPlayerBlockList)//Draw GetPlayerBlock
                    if (Vector2.Distance(
                     new Vector2(_obj.Position.X, _obj.Position.Z),
                     new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                 < GameState.World.viewDistance)

                        _obj.Render(GameFlags, preShadowEffect, GraphicsDevice);

                if (Vector2.Distance(
                 new Vector2(Truhe.Position.X, Truhe.Position.Z),
                 new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
             < GameState.World.viewDistance)

                    Truhe.Render(GameFlags, preShadowEffect, GraphicsDevice);

                shadowEffect.FarLightMatrix = farLightMatrix;
                shadowEffect.FarLightTexture = farShadowMap;
            }


            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

            if (GameFlags.HasFlag(GameFlags.GameLoaded))
            {
                Matrix _viewMatrix = GameState.Camera.CalculateViewMatrix();
                Matrix _perspectiveMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), Window.ClientBounds.Width * 1f / Window.ClientBounds.Height, 1f, 1000f);

                shadowEffect.ViewMatrix = _viewMatrix;
                shadowEffect.ProjectionMatrix = _perspectiveMatrix;

                GameState.Player.Render(GameFlags, shadowEffect, GraphicsDevice);

                GameState.World.Render(GameFlags, shadowEffect, GraphicsDevice);

                foreach (var _obj in GameState.World.specialBlocks)
                    if (Vector2.Distance(
                            new Vector2(_obj.Key.X, _obj.Key.Z),
                            new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                        < GameState.World.viewDistance)

                        _obj.Value.Render(GameFlags, shadowEffect, GraphicsDevice);

                foreach (var _obj in GameState.World.Objects)
                    if (Vector2.Distance(
                            new Vector2(_obj.Position.X, _obj.Position.Z),
                            new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                        < GameState.World.viewDistance)

                        _obj.Render(GameFlags, shadowEffect, GraphicsDevice);

                foreach (var _obj in Enemy.EnemyList) //Draw Enemy
                    if (Vector2.Distance(
                           new Vector2(_obj.Position.X, _obj.Position.Z),
                           new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                       < GameState.World.viewDistance)

                        _obj.Render(GameFlags, shadowEffect, GraphicsDevice);

                foreach (var _obj in Geschoss.GeschossList)//Draw Geschosse
                    if (Vector2.Distance(
                         new Vector2(_obj.Position.X, _obj.Position.Z),
                         new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                     < GameState.World.viewDistance)

                        _obj.Render(GameFlags, shadowEffect, GraphicsDevice);

                foreach (var _obj in GetPlayerBlock.GetPlayerBlockList)//Draw GetPlayerBlock
                    if (Vector2.Distance(
                     new Vector2(_obj.Position.X, _obj.Position.Z),
                     new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
                 < GameState.World.viewDistance)

                        _obj.Render(GameFlags, shadowEffect, GraphicsDevice);

                if (Vector2.Distance(
               new Vector2(Truhe.Position.X, Truhe.Position.Z),
               new Vector2(GameState.Camera.Center.Position.X, GameState.Camera.Center.Position.Z))
           < GameState.World.viewDistance)

                    Truhe.Render(GameFlags, preShadowEffect, GraphicsDevice);

                GraphicsDevice.SetRenderTarget(null);
                editor.Render(GameFlags, _viewMatrix, _perspectiveMatrix, GraphicsDevice);

                IGamer.Render(spriteBatch);
            }
            

            GraphicsDevice.SetRenderTarget(null);
            // Render shadow map
            spriteBatch.Begin(0, BlendState.Opaque, SamplerState.AnisotropicClamp);
            spriteBatch.Draw(nearShadowMap, new Rectangle(0, 756, 256, 256), Color.White);
            spriteBatch.Draw(farShadowMap, new Rectangle(0, 500, 256, 256), Color.White);
            //spriteBatch.Draw(realRenderTarget, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.End();

            if (GameFlags.HasFlag(GameFlags.Menu))
                MainMenu.Render(spriteBatch);

            stopwatch.Stop();
            DebugHelper.Information.renderTime = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();

            if (GameFlags.HasFlag(GameFlags.Debug))
                DebugHelper.RenderOverlay(spriteBatch, font);



            base.Draw(gameTime);
        }

        // Menu Callbacks

        public void StartNewGame()
        {
            RenderableWorld _world = CreateWorld();
            _world.Initialize(GraphicsDevice, Content);

            worldRenderables.Clear();
            //worldRenderables.Add(_world);
            foreach (var _renderable in worldRenderables)
                initializeList.Add(_renderable);

            GameState = new GameState(_world, new Player(new Vector3(_world.SpawnPos.X, _world.SpawnPos.Y, _world.SpawnPos.Z)), new Camera());
            GameState.Player.Initialize(GraphicsDevice, Content);
            //initializeList.Add(GameState.Player);

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
            //worldRenderables.Add(GameState.World as IRenderable);
            GameState.World.Initialize(GraphicsDevice, Content);

            initializeList.Clear();
            foreach (var _renderable in worldRenderables)
                initializeList.Add(_renderable);

            //initializeList.Add(GameState.Player);
            GameState.Player.Initialize(GraphicsDevice, Content);

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

        public void ShowOptions()
        {

        }
    }
}
