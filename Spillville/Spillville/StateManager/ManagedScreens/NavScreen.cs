using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Spillville.StateManager.ManagedScreens
{
    class NavScreen : GameScreen
    {

        protected List<Texture2D> NavScreens;
        private Texture2D xboxB_button;
        private int _currentPage;
        private Rectangle buttonRect;
        private Vector2 textPos;
        protected bool ShowBackButton;
        protected int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = (int)MathHelper.Clamp(value, 1, NavScreens.Count);
            }
        }

        public NavScreen(Spillville game)
            : base(game)
        {
            NavScreens = new List<Texture2D>();
        }

        public override void Initialize()
        {
            _currentPage = 1;
            textPos = new Vector2(340, 5);
            buttonRect = new Rectangle(680, 0, 40, 40);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            xboxB_button = Game.Content.Load<Texture2D>(@"Icons\xboxControllerButtonB");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            PlayerIndex idxout;
            if (input.IsNewButtonPress(Buttons.RightShoulder, PlayerIndex.One, out idxout) ||
                input.IsNewButtonPress(Buttons.RightThumbstickRight, PlayerIndex.One, out idxout) ||
                input.IsNewButtonPress(Buttons.LeftThumbstickRight, PlayerIndex.One, out idxout) ||
                input.IsNewButtonPress(Buttons.DPadRight, PlayerIndex.One, out idxout)
                )
            {
                CurrentPage++;
            }
            if (input.IsNewButtonPress(Buttons.LeftShoulder, PlayerIndex.One, out idxout) ||
                input.IsNewButtonPress(Buttons.RightThumbstickLeft, PlayerIndex.One, out idxout) ||
                input.IsNewButtonPress(Buttons.LeftThumbstickLeft, PlayerIndex.One, out idxout) ||
                input.IsNewButtonPress(Buttons.DPadLeft, PlayerIndex.One, out idxout)
                )
            {
                CurrentPage--;
            }

            if (input.IsNewButtonPress(Buttons.B, PlayerIndex.One, out idxout) ||
                input.IsNewButtonPress(Buttons.Back, PlayerIndex.One, out idxout))
            {
                mainGame.ShowEarthView();
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
            
                ScreenSpriteBatch.Begin();
                ScreenSpriteBatch.Draw(NavScreens.ElementAt<Texture2D>(CurrentPage - 1), graphicsDevice.Viewport.Bounds, Color.White);
                if (ShowBackButton)
                {
                ScreenSpriteBatch.Draw(xboxB_button, buttonRect, xboxB_button.Bounds, Color.White);
                ScreenSpriteBatch.DrawString(ScreenFont, "Go To Main Menu", textPos, Color.Red);
                }
                ScreenSpriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
