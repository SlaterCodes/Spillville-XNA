using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

using Spillville.Models;
using Spillville.MainGame.Levels;
using Spillville.Utilities;
using Spillville.MainGame.World;
using Spillville.Models.Objects;


namespace Spillville.MainGame.LevelSelect
{
    public class LevelSelector : DrawableGameComponent, LoadableGameComponent
    {

        public List<Level> Levels;
        private List<LevelIndicator> _levelIndicators;
        private Spillville _spillville;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private CursorModel  _cursor;
        private Texture2D _spaceBackground;
        private Texture2D _levelSelectImgDL;
        private Texture2D _levelSelectImgDR;
        private Texture2D _levelSelectImgUL;
        private Texture2D _levelSelectImgUR;
        private SpriteFont _levelInfoFont;
        private Rectangle _bgRect;
        private InputState _input;
        private bool _earthAutoSlowSpin;
        
        private bool MainViewState;
        private bool _allowButton;


        public bool SequencesComplete { get; private set; }

        private Vector3 SmallEarthVector;
        private Vector3 NormalEarthVector;


        private LevelIndicator _selectedIndicator;
        /*
         * Ideas:
         * Have an event that fires to Spillvile to indicate a Level has been loaded
         * and Spillville should load GamePlay
         * 
         * Actually should be able to call StartGame in Spillville to initiate the LoadSequence
         * for GamePlay and will also unload this component automatically.
         */

        private EarthModel _earth;
        private XboxAvatar _xboxAvatar;

        public LevelSelector(Spillville game)
            : base(game)
        {
            _spillville = game;
            _input = game.Input;
            Levels = new List<Level>();
            _levelIndicators = new List<LevelIndicator>();
            _tempLevelHold = new List<Level>();
            _earth = new EarthModel();
            _cursor = new CursorModel();
            MainViewState = false;
            _earthAutoSlowSpin = true;
            _allowButton = false;
            _xboxAvatar = new XboxAvatar();
        }

        public void LoadLevel(Level level)
        {
            GameStatus.Reset();
            _spillville.StartGame(level);
        }

        public void SetInitialMainViewState(bool on)
        {
            MainViewState = on;
            if (MainViewState)
            {
                _earth.ModelPosition = NormalEarthVector;
                _addLevelIndicatorSequenceComplete = true;
                _earthSequenceComplete = true;
                SequencesComplete = true;
            }
            else
            {
                _levelIndicators.Clear();
                _earth.ModelPosition = SmallEarthVector;
                _addLevelIndicatorSequenceComplete = true;
                _earthSequenceComplete = true;
                SequencesComplete = true;
            }
        }

        public void ActivateMainViewState(bool on)
        {
            if (on == MainViewState)
                return;
            SequencesComplete = false;
            MainViewState = on;
            Debug.WriteLine("MainViewState: " + MainViewState);
            if (MainViewState)
            {
                Debug.WriteLine("Activating Earth View");
                _spillville.Input.Enabled = false;
                _addLevelIndicatorSequenceComplete = false;
                _earthSequenceComplete = false;
                SequencesComplete = false;
                _selectedIndicator = null;
                _xboxAvatar.PlayAnimation("Celebrate",false);

            }
            else
            {
                _selectedIndicator = null;
                _spillville.Input.Enabled = false;
                _earthSequenceComplete = false;
                _addLevelIndicatorSequenceComplete = false;
                SequencesComplete = false;
                Debug.WriteLine("Deactivating Earth View");
                _xboxAvatar.PlayAnimation("Confused",true);
            }
        }



        public override void Initialize()
        {
            
            Camera.Initialize(Game.GraphicsDevice.Viewport);
            Camera.Target = new Vector3(0f,0f,0f);
            Camera.Position = new Vector3(0.0f, 1000f,160f);
            /*
#if XBOX
            _avatarDescription = Spillville.GamerAvatarDescription;
            _avatarRenderer = Spillville.GamerAvatarRenderer;
            _avatarAnimation = new AvatarAnimation(AvatarAnimationPreset.Stand0);
            _avatarScale = 30f;
            _avatarPosition = new Vector3(90f,940f,0);
#endif*/
            _xboxAvatar.Initialize(new Vector3(90f, 940f, 0));
            _xboxAvatar.AvatarScale = 30f;

            _bgRect = new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, 480);
            _graphicsDevice = Game.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);

            SmallEarthVector = new Vector3(100, 1040f, -150f);
            NormalEarthVector = new Vector3(0, 1000, 0f);

            SetInitialMainViewState(false);

            WaterShader.Initialize(_graphicsDevice);
            WaterShader.WaveSpeed = 1f;

            DoneLoading += LoadingDone;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spaceBackground = Game.Content.Load<Texture2D>(@"Earth\cloudmenubackground-title");
            _levelSelectImgUR = Game.Content.Load<Texture2D>(@"Earth\levelselect\LevelSelectUpRight");
            _levelSelectImgUL = Game.Content.Load<Texture2D>(@"Earth\levelselect\LevelSelectUpLeft");
            _levelSelectImgDR = Game.Content.Load<Texture2D>(@"Earth\levelselect\LevelSelectDownRight");
            _levelSelectImgDL = Game.Content.Load<Texture2D>(@"Earth\levelselect\LevelSelectDownLeft");

            ModelFactory.Add(typeof(LevelIndicator).Name, Game.Content.Load<Model>(@"Earth\EarthIndicatorCircle"));
			//ModelFactory.Add(typeof(LevelIndicator).Name, Game.Content.Load<Model>(@"Models\Objects\OilRig"));

            _earth.Initialize(Game.Content.Load<Model>(@"Earth\EarthHD"));
            _cursor.Initialize(Game.Content.Load<Model>(@"Earth\EarthCursor"), new Vector3(NormalEarthVector.X, NormalEarthVector.Y, _earth.Radius+1));

            _levelInfoFont = Game.Content.Load<SpriteFont>(@"fonts\MessageFont");

            WaterShader.LoadContent(_spillville.Content);

            base.LoadContent();
            IsLoaded = true;

            GC.Collect();
            DoneLoading(this, EventArgs.Empty);
            LoadDone();
        }
        protected override void UnloadContent()
        {
            IsLoaded = false;
            Levels.Clear();
            _levelIndicators.Clear();
             ModelFactory.Clear();
             AudioManager.Reset();
            GameStatus.Reset();
            base.UnloadContent();
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            WaterShader.Update(gameTime);
            PlayerIndex vx;
            if (_allowButton && MainViewState && _input.IsNewButtonPress(Buttons.B, PlayerIndex.One, out vx))
            {
                _allowButton = false;
                ActivateMainViewState(!MainViewState);
            }
            if (MainViewState)
            {
                if (SequencesComplete)
                {
                    MainViewNormalUpdate(gameTime);
                }
                else
                {
                    // These are "sequences" for when the earth comes into view
                    if (!_addLevelIndicatorSequenceComplete && _earthSequenceComplete)
                        AddLevelIndicatorSequence(gameTime);
                    if (!_earthSequenceComplete)
                        EarthZoomSequence(gameTime);

                    if (_addLevelIndicatorSequenceComplete && _earthSequenceComplete)
                    {
                        _spillville.ManagedScreenState(false);
                        _spillville.Input.Enabled = true;
                        SequencesComplete = true;
                        _allowButton = true;
//#if XBOX
//                        _avatarAnimation = new AvatarAnimation(AvatarAnimationPreset.Stand1);
//#endif 
                    }
                }
            }
            else
            {
                RotateEarth(.1f, false);
                if (!SequencesComplete)
                {
                    if (!_earthSequenceComplete)
                    {
                        EarthZoomSequence(gameTime);
                    }
                    if (!_addLevelIndicatorSequenceComplete)
                    {
                        AddLevelIndicatorSequence(gameTime);
                    }
                    if (_earthSequenceComplete && _addLevelIndicatorSequenceComplete)
                    {
                        SequencesComplete = true;
                        _allowButton = true;
                        _spillville.ManagedScreenState(true);
                        _spillville.Input.Enabled = true;
                    }
                }
            }

            Camera.Update(gameTime);
            _earth.Update(gameTime);
            GameStatus.Update(gameTime);
            /*
#if XBOX
            AvatarUpdate(gameTime);
#endif*/
            _xboxAvatar.Update(gameTime);
            base.Update(gameTime);
        }

        private void MainViewNormalUpdate(GameTime gameTime)
        {
            _earthAutoSlowSpin = true;
            HandleInput(gameTime);
            CursorHover(gameTime);

            if(_earthAutoSlowSpin)
                RotateEarth(.03f, false);

            for (var i = 0; i < _levelIndicators.Count; i++)
                _levelIndicators[i].Update(gameTime);

            _cursor.Update(gameTime);
        }
        /*
#if XBOX
        private void AvatarUpdate(GameTime gameTime)
        {
            if (_avatarRenderer.State == AvatarRendererState.Ready)
            {
                _avatarAnimation.Update(gameTime.ElapsedGameTime, true);
            }
        }
#endif*/

        private void HandleInput(GameTime gameTime)
        {
            PlayerIndex vx;
            if (_input.IsNewButtonPress(Buttons.A, PlayerIndex.One, out vx))
            {
                CursorSelect(gameTime);
            } 

            if (_input.CurrentGamePadStates[0].ThumbSticks.Right.X != 0 ||
                _input.CurrentGamePadStates[0].ThumbSticks.Right.Y != 0)
            {
                var directionx = _input.CurrentGamePadStates[0].ThumbSticks.Right.X;
                var leftTrig = _input.CurrentGamePadStates[0].Triggers.Left > 0;
                RotateEarth(directionx,  leftTrig);
            }

            _cursor.MoveCursor(_input.CurrentGamePadStates[0].ThumbSticks.Left.X,
                _input.CurrentGamePadStates[0].ThumbSticks.Left.Y);
        }

        public void RotateEarth(float amountX, bool mult)
        {
            var earthRotatex = .02f * amountX * (mult ? 2f : 1);

            _earth.Rotate(earthRotatex);

            for (var i = 0; i < _levelIndicators.Count; i++)
            {
                var levelIndicator = _levelIndicators[i];
                var rotMatrix = Matrix.CreateRotationY(earthRotatex);
                var finalVect = Vector3.Transform(levelIndicator.ModelPosition, rotMatrix);

                var finalRotationVector = Vector3.Transform(levelIndicator.ModelRotation, rotMatrix);


                //sin's stuff
                //rotMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(45));
                //finalVect = Vector3.Transform(levelIndicator.ModelPosition, rotMatrix);
                //finalRotationVector = Vector3.Transform(levelIndicator.ModelRotation, rotMatrix);

                levelIndicator.ModelPosition = finalVect;
                levelIndicator.ModelRotation = finalRotationVector;
                levelIndicator.IsBoundingBoxUpToDate = false;
            }
        }


        private void CursorHover(GameTime gameTime)
        {
            _selectedIndicator = null;
            for (var i = 0; i < _levelIndicators.Count; i++)
            {
                var ind = _levelIndicators[i];
                if (_cursor.boundaries.Intersects(ind.boundaries) && ind.ModelPosition.Z > 30)
                {
                    _selectedIndicator = ind;
                    _earthAutoSlowSpin = false;
                }
            }
        }
        private void CursorSelect(GameTime gameTime)
        {
            if (_selectedIndicator != null)
            {
                LoadLevel(_selectedIndicator.LevelInfo);
            }
        }

        #endregion

        #region Earth Sequence
        private bool _earthSequenceComplete;
        private TimeSpan _earthSequenceTime;
        private void EarthZoomSequence(GameTime gameTime)
        {
            // Whether zooming into view or out
            if(MainViewState)
            {
                if (gameTime.TotalGameTime - _earthSequenceTime > TimeSpan.FromMilliseconds(2))
                {
                    RotateEarth(.1f, false);
                    _earthSequenceTime = gameTime.TotalGameTime;
                    _earth.ModelPosition = Vector3.SmoothStep(_earth.ModelPosition, NormalEarthVector, .1f);
                    if (Vector3.Distance( _earth.ModelPosition,NormalEarthVector)<.3f)
                    {
                        _earthSequenceComplete = true;
                        _earth.ModelPosition = NormalEarthVector;
                        Debug.WriteLine("Earth Sequence Complete");
                    }
                }
            }
            else
            {
                if (gameTime.TotalGameTime - _earthSequenceTime > TimeSpan.FromMilliseconds(2))
                {
                    RotateEarth(.1f, false);
                    _earthSequenceTime = gameTime.TotalGameTime;
                    _earth.ModelPosition = Vector3.SmoothStep(_earth.ModelPosition, SmallEarthVector, .1f);
                    if (Vector3.Distance(_earth.ModelPosition, SmallEarthVector) < .3f)
                    {
                        _earthSequenceComplete = true;
                        _earth.ModelPosition = SmallEarthVector;
                        Debug.WriteLine("Earth Sequence Complete");
                    }
                }
            }
        }
        #endregion 

        #region LevelIndicator Sequence
        private bool _addLevelIndicatorSequenceComplete;
        private TimeSpan _timeBetweenIndicatorSequence;
        private List<Level> _tempLevelHold;
        private void AddLevelIndicatorSequence(GameTime gameTime)
        {
            if (MainViewState)
            {
                if (_levelIndicators.Count<LevelIndicator>((ind) => !ind.Visible) > 0)
                {
                    var ind = _levelIndicators.First<LevelIndicator>((i) => i.Visible == false);
                    ind.Visible = true;
                    Debug.WriteLine("Level Indicator Visible: " + ind.LevelInfo.Name);
                    // TODO AudioSound
                }
                else
                {
                    _addLevelIndicatorSequenceComplete = true;
                    Debug.WriteLine("Level Indicator Sequence Complete");
                }
            }
            else
            {
                for (var i = 0; i < _levelIndicators.Count; i++)
                {
                    _levelIndicators[i].Visible = false;
                }
                _addLevelIndicatorSequenceComplete = true;
            }
        }
        private void AddLevelIndicator(Level level)
        {
            Debug.WriteLine("Level Indicator Added: " + level.Name);
            var levelIndicator = new LevelIndicator(level);
            var startVector = GetPlacementVector(level.EarthLocation.X, level.EarthLocation.Y);
            levelIndicator.Initialize(startVector, level.EarthLocation.X * 2);
            _levelIndicators.Add(levelIndicator);
        }

        public Vector3 GetPlacementVector(float latitude, float longitude)
        {
            latitude = latitude * (-1);
            if (latitude > 45)
                latitude = 45;
            else if (latitude < -45)
                latitude = -45;

            if (longitude > 360)
                longitude = 360;
            else if (longitude < 0)
                longitude = 0;

            //var originalVector = NormalEarthVector;
            //originalVector.Z += _earth.Radius + LevelIndicator.EarthSurfaceDistance;

            var originalVector = new Vector3(0, 0, _earth.Radius + LevelIndicator.EarthSurfaceDistance);

            var rotationToBeDone = Matrix.CreateRotationY(MathHelper.ToRadians(longitude));
            var finalVector = Vector3.Transform(originalVector, rotationToBeDone);
            rotationToBeDone = Matrix.CreateRotationX(MathHelper.ToRadians(latitude));
            finalVector = Vector3.Transform(finalVector, rotationToBeDone);
            finalVector.X += NormalEarthVector.X;
            finalVector.Y += NormalEarthVector.Y;
            finalVector.Z += NormalEarthVector.Z;
            return finalVector;
        }
        #endregion

        #region Drawing
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_spaceBackground, _bgRect, Color.White);
            _spriteBatch.End();

            WaterShader.Draw(gameTime);

            for (var i = 0; i < _levelIndicators.Count; i++)
            {
                if (_levelIndicators[i].Visible)
                {
                    GraphicsDevice.DepthStencilState = _spillville.DepthState;
                    _levelIndicators[i].Draw(gameTime, _spriteBatch);
                }
            }

            GraphicsDevice.DepthStencilState = _spillville.DepthState;
            ModelDrawer.DrawEarth(_earth);
            _earth.Draw(gameTime,_spriteBatch);

            if (_selectedIndicator != null)
            {
                ShowLevelInfoBox(gameTime);
            }

            if (MainViewState && SequencesComplete)
            {
                GraphicsDevice.DepthStencilState = _spillville.DepthState;
                ModelDrawer.Draw(_cursor);
                _cursor.Draw(gameTime, _spriteBatch);
            }
            /*
#if XBOX
            DrawGamer(gameTime);
#endif*/
            _xboxAvatar.Draw(gameTime);

            base.Draw(gameTime);
        }

        public void ShowLevelInfoBox(GameTime gameTime)
        {

            var _screenCoords = _spriteBatch.GraphicsDevice.Viewport.Project
                (_selectedIndicator.ModelPosition, Camera.Projection, Camera.View, Matrix.Identity);

            var midWidth = _graphicsDevice.Viewport.Width / 2;
            var midHeight = _graphicsDevice.Viewport.Height / 2;

            // Up - Right is default

            var levelimg = _levelSelectImgDR;
            var size = levelimg.Bounds;
            var position = new Vector2(_screenCoords.X, _screenCoords.Y);

            var x10Percent = .1f * size.Width;
            var x3Percent = .03f * size.Width;
            var y1Percent = .01f * size.Height;
            var y38Percent = .38f * size.Height;
            var textPosition = new Vector2(_screenCoords.X, _screenCoords.Y);

            bool defaultCase = true;
            if (_screenCoords.X < midWidth && _screenCoords.Y > midHeight)
            {
                defaultCase = false;
                levelimg = _levelSelectImgUR;
                size = levelimg.Bounds;
                position.Y -= size.Height;

                textPosition.X += x10Percent;
                textPosition.Y -= size.Height;
            }

            if (_screenCoords.X > midWidth && _screenCoords.Y < midHeight)
            {
                defaultCase = false;
                levelimg = _levelSelectImgDL;
                size = levelimg.Bounds;
                position.X -= size.Width;

                textPosition.Y += y38Percent;
                textPosition.X -= size.Width - x3Percent;
            }

            if (_screenCoords.X > midWidth && _screenCoords.Y > midHeight)
            {
                defaultCase = false;
                levelimg = _levelSelectImgUL;
                size = levelimg.Bounds;
                position.X -= size.Width;
                position.Y -= size.Height;

                textPosition.X -= size.Width - x3Percent;
                textPosition.Y -= size.Height - y1Percent;
            }

            if (defaultCase)
            {
                textPosition.X += x10Percent;
                textPosition.Y += y38Percent;
            }

            var level = _selectedIndicator.LevelInfo;
            var fontsizeheight = _levelInfoFont.MeasureString(level.Name).Y*1.25f;
            var fontColor = Color.Black;

            _spriteBatch.Begin();
            _spriteBatch.Draw(levelimg, position, Color.White);
            _spriteBatch.DrawString(_levelInfoFont, level.Name, textPosition, fontColor);
            textPosition.Y += fontsizeheight;
            _spriteBatch.DrawString(_levelInfoFont,level.Description,textPosition,fontColor);
            textPosition.Y += fontsizeheight;
            _spriteBatch.DrawString(_levelInfoFont, "TimeLimit: " + level.TimeLimit, textPosition, fontColor);
            textPosition.Y += fontsizeheight;
            _spriteBatch.DrawString(_levelInfoFont, "Latitude: " + level.EarthLocation.X + " " + "Longitude: " + level.EarthLocation.Y, textPosition, fontColor);
            _spriteBatch.End();
        }
        /*
#if XBOX
        public void DrawGamer(GameTime gameTime)
        {
            _avatarRenderer.World = Matrix.CreateScale(_avatarScale) *
                Matrix.CreateRotationY(MathHelper.ToRadians(140)) *
                Matrix.CreateTranslation(_avatarPosition);
            _avatarRenderer.Projection = Camera.Projection;
            _avatarRenderer.View = Camera.View;
            _avatarRenderer.Draw(_avatarAnimation.BoneTransforms,_avatarAnimation.Expression);
        }
#endif*/
        #endregion

        public void LoadingDone(object sender, EventArgs args)
        {

        }
        public void LoadDone()
        {
            IsLoaded = true;
            _spillville.MainMenuAttach();
            //foreach (var level in Levels)
            AddLevelIndicator(new Level1());
            AddLevelIndicator(new Level2());
            AddLevelIndicator(new Level3());
            AddLevelIndicator(new Level4());
            _spillville.LoadingFinished();
        }



    	public bool IsLoaded { get; private set; }

        public void DisableComponent()
        {
            this.Enabled = false;
            this.Visible = false;
        }

        public void EnableComponent()
        {
            this.Enabled = true;
            this.Visible = true;
        }

        public event EventHandler DoneLoading;
    }
}
