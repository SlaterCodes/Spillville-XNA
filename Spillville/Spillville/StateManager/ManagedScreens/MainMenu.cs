using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Spillville.Utilities;
using System.Threading;
namespace Spillville.StateManager.ManagedScreens
{
    class MainMenu : MenuScreen
    {
        private Texture2D backgroundTexture;
        

        public MainMenu(Spillville game) 
            : base(game,"")
        {

            MenuEntry newGameEntry = new MenuEntry("Play New Game");
            newGameEntry.Selected += PlayGameAction;
            MenuEntries.Add(newGameEntry);
            
            //MenuEntry loadGameEntry = new MenuEntry("Load Saved Game");
            //MenuEntries.Add(loadGameEntry);

            MenuEntry optionsEntry = new MenuEntry("Options");
            optionsEntry.Selected += OptionsAction;
            MenuEntries.Add(optionsEntry);

            MenuEntry helpEntry = new MenuEntry("How to Play");
            helpEntry.Selected += HelpAction;
            MenuEntries.Add(helpEntry);

            MenuEntry creditEntry = new MenuEntry("Credits");
            creditEntry.Selected += CreditsAction;
            MenuEntries.Add(creditEntry);

            MenuEntry exitEntry = new MenuEntry("Exit Game");
            exitEntry.Selected += ExitGameAction;
            MenuEntries.Add(exitEntry);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            this.backgroundTexture = Game.Content.Load<Texture2D>("menudemo3");
            base.LoadContent();
        }

        public void PlayGameAction(object sender, PlayerIndexEventArgs e)
        {
            Console.WriteLine("Play Game Pressed");
            //mainGame.StartGame();

            this.ExitScreen();
            mainGame.ShowEarthLevelSelectionView();
        }


        void OptionsAction(object sender, PlayerIndexEventArgs e)
        {
        }

        void HelpAction(object sender, PlayerIndexEventArgs e)
        {
            mainGame.LoadSequence(new HowToPlay(mainGame));
        }

        void CreditsAction(object sender, PlayerIndexEventArgs e)
        {
            mainGame.LoadSequence(new Credits(mainGame));
        }

        void ExitGameAction(object sender, PlayerIndexEventArgs e)
        {
            mainGame.ExitSequence();
        }

        public override void Draw(GameTime gameTime)
        {
            //ScreenSpriteBatch.Begin();
            //ScreenSpriteBatch.Draw(backgroundTexture, graphicsDevice.Viewport.Bounds, Color.White);
            //ScreenSpriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
