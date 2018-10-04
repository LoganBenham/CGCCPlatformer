using System;
using System.Threading;
using CGCCPlatformer.Helpers;
using CGCCPlatformer.Helpers.Graphics;
using CGCCPlatformer.UI;
using CGCCPlatformer.UI.Common;
using CGCCPlatformer.UI.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Cursor = CGCCPlatformer.UI.Cursor;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace CGCCPlatformer
{
    /// <summary>
    /// You just lost the game.
    /// </summary>
    public class TheGame : Game
    {
        //Singleton since only 1 game is running in this code assembly at a time
        //And now we can access it with TheGame.Game
        public static TheGame Game { get; private set; }
        public static Rectangle Bounds => Game.GraphicsDevice.Viewport.Bounds;

        private readonly Input input;

        public static readonly Point MinWindowSize = new Point(800, 600);
        public static readonly Point DefaultWindowSize = new Point(800, 800);

        public GraphicsDeviceManager Graphics { get; }

        public enum GameState
        {
            MainMenu,
            InGame,
            NewGame,
            Loading
        }
        
        private int fullscreenPrevHeight = 800;
        private int fullscreenPrevWidth = 1000;
        
        public GameState State { get; private set; }

        private bool escMenuBool;
        public bool EscMenuBool
        {
            get { return escMenuBool; }
            set
            {
                if (escMenuBool == value)
                    return;
                escMenuBool = value;
                //anything special we might need to do when escMenu is entered to exited
            }
        }

        //Screens/Windows
        public EscMenu EscMenu { get; private set; }
        public MainMenu MainMenu { get; private set; }
        public NewGame NewGame { get; private set; }
        public LoadingScreen LoadingScreen { get; set; }
        public FpsCounter FpsCounter { get; private set; }

        //Constructor
        public TheGame()
        {
            Logging.WriteLine(Logging.Level.Debug, "Constructing game...");

            Game = this;

            Content.RootDirectory = "Content";
            Window.Title = "CGCC Platformer [WIP]";
            Window.AllowUserResizing = true;

            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = MinWindowSize.X,
                PreferredBackBufferHeight = MinWindowSize.Y,
                PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = true
            };

            input = new Input();

            Window.ClientSizeChanged += ResizeWindow;
            Exiting += (sender, args) =>
            {
                Logging.WriteLine("Exiting.");
                LoadingScreen?.CancelHandler?.Invoke();
            };

            Activated += (sender, args) => Window.Title = "CGCC Platformer [WIP]";
            Deactivated += (sender, args) => Window.Title = "***CGCC Platformer [WIP]***";

            Logging.WriteLine(Logging.Level.Debug, "Game constructed.");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Logging.WriteLine("Initializing...");

            // TODO: Add your initialization logic here

            //this.Components.Add(new DrawableGameComponent(this));

            MainMenu = new MainMenu();
            EscMenu = new EscMenu();
            FpsCounter = new FpsCounter();
            NewGame = new NewGame();

            base.Initialize();

            Logging.WriteLine("Initialized.");
            Logging.WriteLine(Logging.Level.Info);

            var loadThread = new Thread(LoadAllContent);
            LoadingScreen = new LoadingScreen(null, loadThread, Gfx.Fonts.MediumFont, "Loading Graphics");
            SetState(GameState.Loading);
            loadThread.Start();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Gfx.SetSpriteBatch(GraphicsDevice);
            Gfx.LoadEssentials(Content);
        }

        private void LoadAllContent()
        {
            Logging.WriteLine("Loading content...");

            Gfx.Load(Content);

            MainMenu.LoadContent();
            EscMenu.LoadContent();
            FpsCounter.LoadContent();
            NewGame.LoadContent();
            
            ResizeWindow(this, null);
            SetState(GameState.MainMenu);

            Logging.WriteLine("Content loaded");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Gfx.Dispose();
        }

        public void SetState(GameState value)
        {
            switch (value)
            {

            }
            if (State == value)
            {
                EscMenuBool = false;
                return;
            }
            Logging.WriteLine(Logging.Level.Debug, "Changed from " + State + " to " + value, 2);
            State = value;
            EscMenuBool = false;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            input.Update(gameTime, IsActive);

            if (input.Ctrl && input.Shift && input.KeyPress(Keys.B))
                Logging.WriteLine(Logging.Level.Info, "Break Point");
            
            
            if (input.KeyPress(Keys.Escape) && Gfx.Loaded)
                EscMenuBool = !EscMenuBool;

            //Fullscreen
            if (input.KeyPress(Keys.F11))
            {
                if (!Graphics.IsFullScreen)
                {
                    fullscreenPrevHeight = GraphicsDevice.Viewport.Height;
                    fullscreenPrevWidth = GraphicsDevice.Viewport.Width;
                    Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                }
                else
                {
                    Graphics.PreferredBackBufferHeight = fullscreenPrevHeight;
                    Graphics.PreferredBackBufferWidth = fullscreenPrevWidth;
                }
                Graphics.ToggleFullScreen();
            }

            FpsCounter.Update(gameTime, input);
            
            if (EscMenuBool)
                EscMenu.Update(gameTime, input);
            else
            {
                switch (State)
                {
                    case GameState.MainMenu:
                        MainMenu.Update(gameTime, input);
                        break;
                    case GameState.Loading:
                        LoadingScreen.Update(gameTime, input);
                        break;
                    case GameState.InGame:
                        break;
                    case GameState.NewGame:
                        NewGame.Update(gameTime, input);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(State), State + " is not a valid game state");
                }
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Gfx.BackgroundColor);
            var mousePos = input.MousePos;
            Cursor.Reset();

            Gfx.SamplerState = SamplerState.AnisotropicClamp;
            Gfx.BeginSpriteBatch();


            if (EscMenuBool)
                EscMenu.Draw(gameTime, mousePos);
            else
            {
                switch (State)
                {
                    case GameState.MainMenu:
                        MainMenu.Draw(gameTime, mousePos);
                        break;
                    case GameState.Loading:
                        LoadingScreen.Draw(gameTime, mousePos);
                        break;
                    case GameState.InGame:
                        Gfx.SpriteBatch.DrawString(Gfx.Fonts.SmallFont, "Hello World.",
                            new Vector2(3, 0), Gfx.CgccGreen);
                        break;
                    case GameState.NewGame:
                        NewGame.Draw(gameTime, mousePos);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(State), State + " is an invalid state");
                }
            }

            FpsCounter.Draw(gameTime, mousePos);
            Cursor.Draw(mousePos);
            
            Gfx.SpriteBatch.End();
            base.Draw(gameTime);
        }

        private void ResizeWindow(object sender, EventArgs e)
        {
            if (Bounds.Size.X < MinWindowSize.X ||
                Bounds.Size.Y < MinWindowSize.Y)
            {
                Graphics.PreferredBackBufferWidth = Bounds.Size.X.ClampMin(MinWindowSize.X);
                Graphics.PreferredBackBufferHeight = Bounds.Size.Y.ClampMin(MinWindowSize.Y);
                Graphics.ApplyChanges();
            }
            Logging.WriteLine(Logging.Level.Debug, "Resized window to x=" + Bounds.Size.X + "  y=" + Bounds.Size.Y);

            GraphicsDevice.ScissorRectangle = Bounds;

            if (LoadingScreen != null)
                LoadingScreen.Bounds = Bounds;

            if (!Gfx.Loaded)
                return;

            MainMenu.Bounds = Bounds;
            FpsCounter.Bounds = Bounds;
            EscMenu.Bounds = Bounds;
            NewGame.Bounds = Bounds;
        }
    }
}
