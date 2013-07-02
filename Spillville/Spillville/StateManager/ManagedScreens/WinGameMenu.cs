using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville.StateManager.ManagedScreens
{
    class WinGameMenu : MenuScreen
    {
        private Texture2D background;
        public WinGameMenu(Spillville game)
            : base(game, "You have successfully contained the oil spill!")
        {

            //MenuEntry newEntry = new MenuEntry("New Game");
            //MenuEntries.Add(newEntry);
            //newEntry.Selected += NewGameAction;

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

        void NewGameAction(object sender, PlayerIndexEventArgs e)
        {
            this.ExitScreen();
            //mainGame.StartGame();
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
