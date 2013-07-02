using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
//#if XBOX
using Microsoft.Xna.Framework.GamerServices;
//#endif
using Spillville.MainGame.Levels;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spillville.MainGame.LevelSelect;
using Spillville.MainGame.World;
using Spillville.StateManager.ManagedScreens;
using Spillville.StateManager;
using Spillville.Utilities;
using Spillville.MainGame;

namespace Spillville
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Spillville : Game
	{

//#if XBOX
		public static SignedInGamer Gamer = default(SignedInGamer);
		public static AvatarDescription GamerAvatarDescription;
		public static AvatarRenderer GamerAvatarRenderer;
//#endif

		public GameScreen managedScreen { get; private set; }
		public LoadingScreen loadingScreen { get; private set; }

		private GamePlay _gamePlay;

		private FrameRateCounter fpsCount;

		//private MainMenu _mainMenu;
		public static GraphicsDeviceManager Graphics;

		// Load fonts that several components will use

		public SpriteFont JingjingFont { private set; get; }

		public InputState Input { get; private set; }

		public Color MasterBackgroundColor { get; set; }

		public LevelSelector levelSelector;

		public static ThreadPoolComponent ThreadPool;

		public DepthStencilState DepthState;

		public Spillville()
		{
			Graphics = new GraphicsDeviceManager(this);
			Graphics.PreferredBackBufferHeight = 720;
			Graphics.PreferredBackBufferWidth = 1280;
			Graphics.PreferMultiSampling = true;

			Content.RootDirectory = "Content";

			Input = new InputState(this);
			loadingScreen = new LoadingScreen(this);
			fpsCount = new FrameRateCounter(this);

			_gamePlay = new GamePlay(this);

			//MasterBackgroundColor = new Color(0, 90, 240);
			MasterBackgroundColor = new Color(10, 100, 250);

			levelSelector = new LevelSelector(this);


			ThreadPool = new ThreadPoolComponent(this);

			/*
			 * Set the screen size based on the recommendation from http://create.msdn.com/en-US/education/catalog/article/bestpractices_31
			 * This is needed as well because the font become grany at the lower resolution.
			 * If you have trouble viewing it on your laptop, press the F key to fullscreen
			 */

			IsMouseVisible = true;

			DepthState = new DepthStencilState();
			DepthState.DepthBufferEnable = true;
			DepthState.DepthBufferWriteEnable = true;

		}


		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Initialize here because don't want to reset for different levels.
			// Should call GameStatus.Clear() before loading a new level
#if XBOX
			GamerServicesDispatcher.Initialize(Services);
			SignedInGamer.SignedIn += SignedInGamer_SignedIn;
#endif
			GameStatus.Initialize();
			AudioManager.Initialize();
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			JingjingFont = Content.Load<SpriteFont>(@"fonts\JingJing");
			AudioManager.LoadContent(this.Content);
			base.LoadContent();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			Content.Unload();
            AudioManager.Unload();
            //ThreadPool.Dispose();
			ThreadPool.KillAnyRunningThreads();
			base.UnloadContent();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
#if XBOX
			GamerServicesDispatcher.Update();
			if (Gamer == default(SignedInGamer))
			{
				GetGamer();
			}
#endif
			PlayerIndex a;
			if (Input.IsNewKeyPress(Keys.K, null, out a))
			{
				Debug.WriteLine(WorldGrid.GridTileAtLocation(Camera.Target.X, Camera.Target.Z).ToString());
			}

			if (Input.IsPauseGame(PlayerIndex.One) && _gamePlay.GameInitiated)
			{
				if (_gamePlay.GamePaused)
					ResumeGame();
				else
					PauseGame();
			}
			AudioManager.Update(gameTime);
			base.Update(gameTime);
		}

		protected override void BeginRun()
		{
#if DEBUG
            StartGame(new Level3()); 
            //ShowEarthView();
#else
            //StartGame(new Level1()); 
            ShowEarthView();
#endif
            base.BeginRun();
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(MasterBackgroundColor);
			//GraphicsDevice.Clear(Color.Black);
			GraphicsDevice.DepthStencilState = DepthState;
			base.Draw(gameTime);
		}


		public void LoadSequence(LoadableGameComponent loadClass)
		{
			System.Diagnostics.Debug.WriteLine("LoadSequence()");
			Components.Clear();
            // Components that should be present with all screens
            ThreadPool = new ThreadPoolComponent(this);
			Components.Add(ThreadPool);
			Components.Add(Input);
			Components.Add(loadingScreen);

			loadingScreen.ShowLoadingScreen();
			// Subscribe to the event DoneLoading for when the Component indicates it has loaded
			loadClass.DoneLoading += loadingScreen.EnableOnLoad;

			// Delegate that gets called when the Loader has drawn and is ready to do loading process
			loadingScreen.LoadDelegateMethod = delegate
			{
				//loadClass.DisableComponent();
				Components.Add(loadClass);
#if DEBUG
				Components.Add(fpsCount);
#endif
			};
			// Take advantage of load screen and force a garbage collect
			// this will take place in its own thread
			System.GC.Collect();
		}

		public void StartGame(Level level)
		{
			Debug.WriteLine("StartGame() Called");
			// Resets everything
			if (_gamePlay != null)
				_gamePlay.Dispose();

			_gamePlay = new GamePlay(this);
            _gamePlay.CurrentLevel = level;
			LoadSequence(_gamePlay);
		}

		public void PauseGame()
		{
			Debug.WriteLine("PauseGame() Called");
			managedScreen = new PauseMenu(this);
			managedScreen.DisableComponent();
			Components.Add(managedScreen);
			_gamePlay.PauseGame();
			managedScreen.EnableComponent();
			System.GC.Collect();
		}

		public void WinGame()
		{
			System.Diagnostics.Debug.WriteLine("WinGame() Called");
			managedScreen = new WinGameMenu(this);
			managedScreen.DisableComponent();
			Components.Add(managedScreen);
			_gamePlay.PauseGame();
			managedScreen.EnableComponent();
            GameStatus.EndLevel();
			System.GC.Collect();
		}

		public void LoseGame()
		{
			Debug.WriteLine("LoseGame() Called");
			managedScreen = new LoseGameMenu(this);
			managedScreen.DisableComponent();
			Components.Add(managedScreen);
			_gamePlay.PauseGame();
			managedScreen.EnableComponent();
            GameStatus.EndLevel();
			System.GC.Collect();
		}

		public void ResumeGame()
		{
			System.Diagnostics.Debug.WriteLine("ResumeGame() Called");
			managedScreen.DisableComponent();
			_gamePlay.ResumeGame();
			Components.Remove(managedScreen);
		}

        public void MainMenuAttach()
        {
            Debug.WriteLine("ShowMainMenu() Called");
            if (Components.Contains(managedScreen))
                Components.Remove(managedScreen);
            if (managedScreen != null)
                managedScreen.Dispose();
            managedScreen = null;
            managedScreen = new MainMenu(this);
            managedScreen.DrawOrder = 10;
            managedScreen.EnableComponent();
            Components.Add(managedScreen);
        }

		public void ShowEarthView()
		{
            if (GameStatus.LevelStarted)
                GameStatus.EndLevel();
			levelSelector.Dispose();
            managedScreen = new MainMenu(this);
			levelSelector = new LevelSelector(this);
			Debug.WriteLine("ShowEarthView() called");
			AudioManager.PlaySong("MainMenuMusic");
			LoadSequence(levelSelector);
		}

		public void ShowEarthLevelSelectionView()
		{
			levelSelector.ActivateMainViewState(true);
		}

        public void ManagedScreenState(bool enabled)
        {
            if (managedScreen != null)
            {
                if (enabled)
                    managedScreen.EnableComponent();
                else
                    managedScreen.DisableComponent();
            }
        }

        public void ShowMainMenuView()
        {
            levelSelector.ActivateMainViewState(false);
        }

		public void EndGame()
		{
			AudioManager.Reset();
			ShowEarthView();
            if(GameStatus.LevelStarted)
                GameStatus.EndLevel();
		}

		public void LoadingFinished()
		{
			loadingScreen.DisableComponent();
		}

        public void RemoveManagedScreen()
        {
            if (managedScreen != null && Components.Contains(managedScreen))
            {
                Components.Remove(managedScreen);
                managedScreen.ExitScreen();
                managedScreen.Dispose();
                managedScreen = null;
                System.GC.Collect();
            }
        }

        public void ExitSequence()
        {
            //Environment.Exit(0);
            this.Exit();
        }

#if XBOX

		void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
		{
			// Only handle player one sign in
			if (e.Gamer.PlayerIndex == PlayerIndex.One)
			{
				GetGamer();
			}
		}

		private void GetGamer()
		{
			Debug.WriteLine("Signed-In Gamer Count: " + Microsoft.Xna.Framework.GamerServices.Gamer.SignedInGamers.Count);
			foreach (var gamer in Microsoft.Xna.Framework.GamerServices.Gamer.SignedInGamers)
			{
				if (gamer.PlayerIndex == PlayerIndex.One)
				{
					Gamer = gamer;
				}
			}

			AvatarDescription.BeginGetFromGamer(Gamer, LoadGamerAvatar, null);



		}

		void LoadGamerAvatar(IAsyncResult result)
		{
			// Get the AvatarDescription for the gamer
			GamerAvatarDescription = AvatarDescription.EndGetFromGamer(result);

			// Load the AvatarRenderer if description is valid
			if (GamerAvatarDescription.IsValid)
				GamerAvatarRenderer = new AvatarRenderer(GamerAvatarDescription);
		}


#endif

	}
}