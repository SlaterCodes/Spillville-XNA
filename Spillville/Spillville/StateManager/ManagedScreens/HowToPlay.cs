using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Spillville.StateManager.ManagedScreens
{
    class HowToPlay : NavScreen
    {
        public HowToPlay(Spillville game)
            : base(game)
        {
            ShowBackButton = false;
        }
        protected override void LoadContent()
        {
            NavScreens.Add(Game.Content.Load<Texture2D>(@"screens\howto\NewHowTo1"));
            NavScreens.Add(Game.Content.Load<Texture2D>(@"screens\howto\NewHowTo2"));
            NavScreens.Add(Game.Content.Load<Texture2D>(@"screens\howto\NewHowTo3"));
            NavScreens.Add(Game.Content.Load<Texture2D>(@"screens\howto\NewHowTo4"));
            NavScreens.Add(Game.Content.Load<Texture2D>(@"screens\howto\NewHowTo5"));
            NavScreens.Add(Game.Content.Load<Texture2D>(@"screens\howto\gamecontrols"));
            base.LoadContent();
        }
    }
}
