using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville.StateManager.ManagedScreens
{
    class PauseMenu : MenuScreen
    {
        private Texture2D background;
        public PauseMenu(Spillville game):base(game,"Game Paused")
        {

            MenuEntry resumeEntry = new MenuEntry("Resume Game");
            MenuEntries.Add(resumeEntry);
            resumeEntry.Selected += ResumeGameAction;

            MenuEntry exitMainMenuEntry = new MenuEntry("Exit to Main Menu");
            MenuEntries.Add(exitMainMenuEntry);
            exitMainMenuEntry.Selected += ExitMainMenuAction;

            MenuEntry exitGameEntry = new MenuEntry("Exit Game");
            MenuEntries.Add(exitGameEntry);
            exitGameEntry.Selected += ExitGameAction;

        }

        protected override void LoadContent()
        {
            background = Game.Content.Load<Texture2D>(@"water");
            this.MenuItemColor = Color.White;
            this.MenuTitleColor = Color.White;
            this.MenuItemSelectedColor = Color.LightGreen;
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenSpriteBatch.Begin();
            // Draw a translucent image to darken the game in the background
            ScreenSpriteBatch.Draw(background,graphicsDevice.Viewport.Bounds, new Color(0,0,0,150));
            ScreenSpriteBatch.End();

            base.Draw(gameTime);
        }

        void ResumeGameAction(object sender, PlayerIndexEventArgs e)
        {
            this.ExitScreen();
            mainGame.ResumeGame();
        }
        void ExitMainMenuAction(object sender, PlayerIndexEventArgs e)
        {
            this.ExitScreen();
            mainGame.EndGame();
        }
        void ExitGameAction(object sender, PlayerIndexEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Game.Exit() called from Pause Menu");
            mainGame.ExitSequence();           
        }
    }
}
