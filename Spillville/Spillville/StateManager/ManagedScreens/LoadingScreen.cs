
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Utilities;

namespace Spillville.StateManager.ManagedScreens
{
    public class LoadingScreen : GameScreen
    {
        private Texture2D _loadingBackground;

        public delegate void LoadingDelegate();
        public LoadingDelegate LoadDelegateMethod;
        public string LoadingText;
        private bool _delegateFired;
        private bool _loadingDrawn;
        private Vector2 _loadingTextPosition;
        private TimeSpan _loadStarted;
        private readonly static TimeSpan _minLoadTime = TimeSpan.FromSeconds(1);
        Vector2 origin;

        //public event EventHandler LoadingScreenVisible;

        public LoadingScreen(Spillville game)
            : base(game)
        {
            // disable loading screen by default and set defaults
            this.ComponentLoadFinished();
            //LoadDelegateMethod.
        }

        public override void Initialize()
        {
            base.Initialize();
            _loadingTextPosition =
                new Vector2(
                    graphicsDevice.Viewport.Bounds.Center.X - ScreenFont.MeasureString(LoadingText).X/2
                    , graphicsDevice.Viewport.Bounds.Center.Y - ScreenFont.MeasureString(LoadingText).Y/2
                    );
        }

        protected override void LoadContent()
        {
            if (!isContentLoaded)
            {
                _loadingBackground = Game.Content.Load<Texture2D>("menudemo3");
                base.LoadContent();
            }
            origin = new Vector2(0, ScreenFont.LineSpacing / 2);
        }

        public override void Update(GameTime gameTime)
        {
            if (_loadStarted == TimeSpan.Zero)
                _loadStarted = gameTime.TotalGameTime;

            if (!_delegateFired && _loadingDrawn && gameTime.TotalGameTime - _loadStarted > _minLoadTime)
            {
                /*
                 * Once Loading screen draws for at least _minLoadTime
                 * It will call the delegate which should add the GameComponent to the game
                 * which will Initialize and LoadContent
                 * the GameComponent should then call DoneLoading()
                 */
                _delegateFired = true;
                LoadDelegateMethod();
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.1f;


            ScreenSpriteBatch.Begin();
            ScreenSpriteBatch.Draw(_loadingBackground, graphicsDevice.Viewport.Bounds, Color.White);
            ScreenSpriteBatch.DrawString(ScreenFont, LoadingText, _loadingTextPosition, Color.Black,0,origin,scale,SpriteEffects.None,0);
            ScreenSpriteBatch.End();
            base.Draw(gameTime);
            _loadingDrawn = true;      
        }

        public void ShowLoadingScreen()
        {
            this.Enabled = true;
            this.Visible = true;
        }

        public void ComponentLoadFinished()
        {
            this.Enabled = false;
            this.Visible = false;
            this._delegateFired = false;
            this._loadingDrawn = false;
            this._loadStarted = TimeSpan.Zero;
            LoadingText = "Loading...";
        }

        public void EnableOnLoad(object sender, EventArgs args)
        {
            LoadableGameComponent loadedClass = (LoadableGameComponent)sender;
            this.ComponentLoadFinished();
            // Enable Loaded Component
            loadedClass.EnableComponent();
            
        }

        

    }
}
