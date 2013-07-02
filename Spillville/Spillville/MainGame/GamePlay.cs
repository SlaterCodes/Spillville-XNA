using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spillville.MainGame.Levels;
using Spillville.Models.Boats;
using Spillville.Models.Animals;
using Spillville.Models;
using Spillville.Models.Objects;
using Spillville.Utilities;
using Spillville.MainGame.World;
using Spillville.MainGame.OilSpillContainer;
using Spillville.Models.ParticlesSystem;

namespace Spillville.MainGame
{
	class GamePlay : DrawableGameComponent, LoadableGameComponent
	{
		private SpriteBatch _spriteBatch;
		private SpriteFont _jingFont;
        private SpriteFont _spillvilleFont;
		private Vector2 _fontPos;

		private VisualHUD _hud;

		//private SkySphere skySphere;

		private readonly InputController _inputController;


		public bool IsLoaded { get; private set; }
		public bool GameInitiated { get; private set; }
		public event EventHandler DoneLoading;

		public bool GamePaused { get; private set; }


		private Spillville _spillville;

		public bool LevelSelectorEnabled;

		public Level CurrentLevel;

        //public ChemicalsParticlesSystem Particles;

		//public HealthBar HealthBar;

		//private ModelDrawer modelDrawer;

        //XboxAvatar avatar;

		public GamePlay(Spillville game)
			: base(game)
		{
			GameInitiated = false;
			_spillville = game;
			_inputController = InputController.Instance;
			_hud = VisualHUD.Instance();

			Enabled = false;
			Visible = false;
			IsLoaded = false;

            //avatar = new XboxAvatar();

			DoneLoading += LoadingDone;
            
            //Particles = new ChemicalsParticlesSystem(game, game.Content);

		}

		public override void Initialize()
		{
			_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			// initialize all game components
			Camera.Initialize(Game.GraphicsDevice.Viewport);
			WorldGrid.Initialize(Game.GraphicsDevice, 60);
			WaterShader.Initialize(Game.GraphicsDevice);
			OilSpillManager.Initialize(Game.GraphicsDevice);
            
            if (CurrentLevel == null)
                CurrentLevel = new Level1();

			Camera.Rotate(new Vector3(1, 0, 0), -MathHelper.ToRadians(35.0f));

			_hud.Initialize(Game.GraphicsDevice);

            //avatar.Initialize(new Vector3(0,40,1000));
            //avatar.AvatarScale = 200f;
            //avatar.RotationYValue = 170f;

			GameInitiated = true;
			base.Initialize();
			GC.Collect();
		}


		protected override void LoadContent()
		{
			_hud.LoadContent(Game.Content);
			VisualHUD.RegisterIcon(typeof(Dinghy).Name, @"Icons\dinghy");

			VisualHUD.RegisterIcon(typeof(Scraper).Name, @"Icons\scraper");
			VisualHUD.RegisterIcon(@"ScaperCapacityUpgrade", @"Icons\ScaperCapacityUpgrade");
			VisualHUD.RegisterIcon(@"TankerSpeedUpgrade", @"Icons\TankerSpeedUpgrade");
			VisualHUD.RegisterIcon(@"TankerSpeedUpgrade2", @"Icons\TankerSpeedUpgrade2");

			VisualHUD.RegisterIcon(typeof(Disperser).Name, @"Icons\disperseship");
			VisualHUD.RegisterIcon(@"CapacityDisperserUpgrade", @"Icons\CapacityDisperserUpgrade");
			VisualHUD.RegisterIcon(@"EnviromentalDisperserUpgrade", @"Icons\EnviromentalDisperserUpgrade");

			VisualHUD.RegisterIcon(typeof(Tanker).Name, @"Icons\tanker");
			VisualHUD.RegisterIcon(typeof(Barricade).Name, @"Icons\BarrierGroup");
			VisualHUD.RegisterIcon(typeof(CommandShip).Name, @"Icons\commandShip");
			VisualHUD.RegisterIcon(@"Remove", @"Icons\Remove");
			VisualHUD.RegisterIcon(@"FastDinghyUpgrade", @"Icons\FastDinghyUpgrade");
			VisualHUD.RegisterIcon(@"RescueDinghyUpgrade", @"Icons\RescueDinghyUpgrade");

			

			// Load All needed models
			ModelFactory.Add(typeof(Scraper).Name, Game.Content.Load<Model>(@"Models\Boats\scraper"));
			ModelFactory.Add(@"CapacityScraper", Game.Content.Load<Model>(@"Models\Boats\CapacityScraper"));
			ModelFactory.Add(@"FastestScraper", Game.Content.Load<Model>(@"Models\Boats\FastestScraper"));
			ModelFactory.Add(@"FastScraper", Game.Content.Load<Model>(@"Models\Boats\FastScraper"));

			ModelFactory.Add(typeof(Tanker).Name, Game.Content.Load<Model>(@"Models\Boats\tanker"));

			ModelFactory.Add(typeof(Dinghy).Name, Game.Content.Load<Model>(@"Models\Boats\dingy"));
			ModelFactory.Add(@"FastDinghy", Game.Content.Load<Model>(@"Models\Boats\FastDinghy"));
			ModelFactory.Add(@"RescueDinghy", Game.Content.Load<Model>(@"Models\Boats\RescueDinghy"));

			ModelFactory.Add(typeof(Disperser).Name, Game.Content.Load<Model>(@"Models\Boats\dispersership"));
			ModelFactory.Add(@"CapacityDisperserShip", Game.Content.Load<Model>(@"Models\Boats\CapacityDisperserShip"));
			ModelFactory.Add(@"EnviromentDisperserShip", Game.Content.Load<Model>(@"Models\Boats\EnviromentDisperserShip"));

			ModelFactory.Add(typeof(CommandShip).Name, Game.Content.Load<Model>(@"Models\Boats\commandship"));
			ModelFactory.Add(typeof(Dolphin).Name, Game.Content.Load<Model>(@"Models\Animals\dolphin"));
            ModelFactory.Add(typeof(Bird).Name, Game.Content.Load<Model>(@"Models\Animals\bird"));
			ModelFactory.Add(typeof(Barricade).Name, Game.Content.Load<Model>(@"Models\Objects\Barrier1"));
			ModelFactory.Add("Barricade.Arm", Game.Content.Load<Model>(@"Models\Objects\Barrier2"));
			ModelFactory.Add(typeof(OilRig).Name, Game.Content.Load<Model>(@"Models\Objects\OilRig"));

			_inputController.Initialize(Game.Content.Load<Model>(@"Models\cursor"), Game.Content.Load<Model>(@"Models\moveIndicator"), this, _spillville);

            ParticlesSystemsCollection.Initialize();

            ParticlesSystemsCollection.ParticlesSystems.Add(new ChemicalsParticlesSystem());
            ParticlesSystemsCollection.ParticlesSystems.Add(new AnimalsHeartParticles());
            ParticlesSystemsCollection.ParticlesSystems.Add(new AnimalsBrokenHeartParticles());
            ParticlesSystemsCollection.ParticlesSystems.Add(new SmokeParticles());
            ParticlesSystemsCollection.ParticlesSystems.Add(new FireParticles());

            for (int i = 0; i < ParticlesSystemsCollection.ParticlesSystems.Count; i++)
            {
                ParticlesSystemsCollection.ParticlesSystems[i].Initilialize();
                ParticlesSystemsCollection.ParticlesSystems[i].LoadContent(Game.Content, GraphicsDevice);
            }

            HealthBar.Initialize(_spriteBatch, Game.Content.Load<SpriteFont>("fonts\\Spillville"));

			_jingFont = _spillville.JingjingFont;


			_fontPos = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
								   Game.GraphicsDevice.Viewport.Height / 2);

			WaterShader.LoadContent(Game.Content);
			WorldGrid.LoadContent(WaterShader.WaterBox, WaterShader.scale);

			base.LoadContent();
			IsLoaded = true;
			GC.Collect();
			DoneLoading(this, EventArgs.Empty);
			DoneLoad();
		}

		protected override void UnloadContent()
		{
			System.Diagnostics.Debug.WriteLine("Unloading GamePlay Content");
			GameStatus.Reset();
			ModelFactory.Clear();
			OilSpillManager.ClearSpills();
			WorldGrid.Reset();
            AudioManager.Reset();
			GC.Collect();
			base.UnloadContent();
		}


		public override void Update(GameTime gameTime)
		{
            if (!GameStatus.LevelStarted)
                GameStatus.StartLevel(gameTime);
			/*PlayerIndex iddxp;
			if (inputState.IsNewButtonPress(Buttons.RightShoulder,PlayerIndex.One,out iddxp))
			{
				_game.MasterBackgroundColor = new Color(80, 180,245);
				WaterShader.WaveSize = 1;
				storming = true;
			}*/

			// Logic checks and calculations for while a level is playing
			if (GameStatus.LevelStarted)
			{
				if (GameStatus.WinGame)
				{
					// TODO WinGame logic
					_spillville.WinGame();
				}

				if (GameStatus.LoseGame)
				{
					_spillville.LoseGame();
					// TODO LoseGame logic
				}

				// TODO Achievement logic
			}

			_inputController.HandleInput(GamePad.GetState(PlayerIndex.One), Keyboard.GetState(), gameTime);

			Camera.Update(gameTime);
			WorldGrid.Update(gameTime);
			WaterShader.Update(gameTime);
			OilSpillManager.Update(gameTime);
			GameStatus.Update(gameTime);
            //avatar.Update(gameTime);
			_hud.Update(gameTime);

            for (int i = 0; i < ParticlesSystemsCollection.ParticlesSystems.Count; i++)
                ParticlesSystemsCollection.ParticlesSystems[i].Update(gameTime);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
            Game.GraphicsDevice.DepthStencilState = _spillville.DepthState;
			OilSpillManager.Draw(gameTime);
			WaterShader.Draw(gameTime);
			if (WorldGrid.DrawGrid)
				WorldGrid.Draw(gameTime);

            for (var i = 0; i < GameStatus.AnimatedDrawList.Count; i++)
            {

                Game.GraphicsDevice.DepthStencilState = _spillville.DepthState;
                ModelDrawer.DrawAnimatedModel(GameStatus.AnimatedDrawList[i]);
                GameStatus.AnimatedDrawList[i].Draw(gameTime, _spriteBatch);
            }

			for (var i = 0; i < GameStatus.DrawList.Count; i++)
			{
				// TODO get and set drawing options
                Game.GraphicsDevice.DepthStencilState = _spillville.DepthState;
				ModelDrawer.Draw(GameStatus.DrawList[i]);
				GameStatus.DrawList[i].Draw(gameTime, _spriteBatch);
			}


            for (int i = 0; i < ParticlesSystemsCollection.ParticlesSystems.Count; i++)
            {
                Game.GraphicsDevice.DepthStencilState = _spillville.DepthState;
                ParticlesSystemsCollection.ParticlesSystems[i].Draw(gameTime);
            }
            Game.GraphicsDevice.DepthStencilState = _spillville.DepthState;
			_hud.Draw();
            //avatar.Draw(gameTime);
			base.Draw(gameTime);
		}

		public void PauseGame()
		{
			GamePaused = true;
            GameStatus.LevelRunning = false;
			Enabled = false;
		}

		public void ResumeGame()
		{
			GamePaused = false;
            GameStatus.LevelRunning = true;
			Enabled = true;
		}

		public void EnableComponent()
		{
			Enabled = true;
			Visible = true;
		}
		public void DisableComponent()
		{
			Enabled = false;
			Visible = false;
		}

		public void DoneLoad()
		{
            AudioManager.PlaySong("Gameplay1");
			GameStatus.LoadLevel(CurrentLevel);
			_inputController.Register();

		}

		public void LoadingDone(object sender, EventArgs args)
		{
			IsLoaded = true;
		}

	}
}