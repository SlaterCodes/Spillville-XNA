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
		private SpriteBatch _spriteBatch;
		private Spillville game;

		private Vector2 _drawLocation;
		#endregion

		#region Initialization

		public FrameRateCounter(Spillville game)
			: base(game)
		{
			this.game = game;
		}

		public override void Initialize()
		{
			_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
			base.Initialize();

			_drawLocation = new Vector2(33, Game.GraphicsDevice.Viewport.Height - 40);
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

		public override void Draw(GameTime gameTime)
		{
            RasterizerState prev = _spriteBatch.GraphicsDevice.RasterizerState;
            _frameCounter++;
			var fps = string.Format("fps: {0}", _frameRate);
			_spriteBatch.Begin();
			_spriteBatch.DrawString(game.JingjingFont, fps, _drawLocation, Color.Black);
			_drawLocation.Y -= 1;
			_spriteBatch.DrawString(game.JingjingFont, fps, _drawLocation, Color.White);
			_drawLocation.Y += 1;
			_spriteBatch.End();
            _spriteBatch.GraphicsDevice.RasterizerState = prev;
		}

		#endregion
	}
}