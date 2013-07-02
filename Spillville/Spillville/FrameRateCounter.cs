using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville
{
    /// <summary>/// A reusable component for tracking the frame rate./// </summary>
    public class FrameRateCounter : DrawableGameComponent
    {
        #region Fields

        private int _frameRate;
        private int _frameCounter;
        private TimeSpan _elapsedTime = TimeSpan.Zero;

        #endregion

        #region Initialization

        public FrameRateCounter(Game game) : base(game)
        {
        }

        #endregion

        #region Update and Draw   

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            _frameCounter++;
            string fps = string.Format("fps: {0}", _frameRate);

            spriteBatch.DrawString(spriteFont, fps, new Vector2(33, 33), Color.Black);
            spriteBatch.DrawString(spriteFont, fps, new Vector2(32, 32), Color.White);
        }

        #endregion
    }
}