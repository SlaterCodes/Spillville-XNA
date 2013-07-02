using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville.StateManager.ManagedScreens
{
    class Credits : NavScreen
    {
        public Credits(Spillville game)
            : base(game)
        {
            ShowBackButton = true;
        }

        protected override void LoadContent()
        {
            NavScreens.Add(Game.Content.Load<Texture2D>(@"screens\credits\credits1"));
            NavScreens.Add(Game.Content.Load<Texture2D>(@"screens\credits\credits2"));
            NavScreens.Add(Game.Content.Load<Texture2D>(@"screens\credits\CreditSplash"));
            base.LoadContent();
        }
    }
}
