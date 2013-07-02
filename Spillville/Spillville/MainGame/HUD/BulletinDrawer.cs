using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using SpillvilleDataTypes;

namespace Spillville.MainGame.HUD
{
	class BulletinDrawer
	{
		private static float IconSize { get { return RectangleHeight - (RectangleBorder * 2); } }
		private const int RectangleHeight = 100;
		private const int RectangleWidth = 650;
		private const int RectangleBorder = 1;
		private const float LeftPadding = 12;
		private const float RightPadding = 12;

		//		private const float FadeOutSeconds = 0;
		//		private const float FadeInSeconds = 0;
		private const float TimeInSeconds = 7;
		private const float BufferTimeInSeconds = 2.5f;

		public static float LineLength
		{
			get { return RectangleWidth - (LeftPadding + RightPadding) - (RectangleBorder * 2) - IconSize; }
		}

		private Texture2D _rectangle;
		private static Queue<Bulletin> _queue;
		private SpriteBatch _spriteBatch;
		private Bulletin _currentBulletin;
		private Vector2 _rectanglePossition;
		private Vector2 _iconPossition;

		private Color _iconDrawColor, _lightGrawDrawColor, _blackDrawColor;
		private Color _drawColor;

		private SpriteFont _fromFont;
		private static SpriteFont _messageFont;
		private Vector2 _textStartPossition;

		private static Random _random = new Random();

		private static GraphicsDevice _graphicsDevice;

		private TimeSpan _timeRemaining;
		private TimeSpan _gapTime = TimeSpan.Zero;

		public BulletinDrawer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
		{
			_graphicsDevice = graphicsDevice;
			_rectangle = HUDComponents.CreateRectangle(graphicsDevice, RectangleWidth, RectangleHeight, RectangleBorder, -1);
			_spriteBatch = spriteBatch;
			_rectanglePossition = new Vector2(7.0f, 7.0f);
			_iconPossition = new Vector2(8.0f, 8.0f);

			_textStartPossition = new Vector2(IconSize + LeftPadding, 7.5f);

			_lightGrawDrawColor = Color.LightGray;
			_blackDrawColor = Color.Black;

			_iconDrawColor = Color.White;
			_iconDrawColor.A = 175;

			_drawColor = Color.White;
			_queue = new Queue<Bulletin>();


		}

		public void LoadContent(ContentManager content)
		{
			SetFont(content.Load<SpriteFont>("fonts\\BulletinFromFont"), content.Load<SpriteFont>("fonts\\BulletinMessageFont"));
		}

		public static void AddBulletin(Bulletin bulletin)
		{
			_queue.Enqueue(bulletin);
		}

#if XBOX
		private static FriendGamer GetRandomFriend()
		{
			var friends = Spillville.Gamer.GetFriends();
			FriendGamer friend;
			int raceCheck = 0;
			do
			{
				friend = friends[_random.Next(friends.Count)];
				raceCheck++;
			} while (friend == default(FriendGamer) && raceCheck < 16);
			return friend;
		}
#endif

		public static void IntializeBulletinText(Bulletin bulletin)
		{
			var newMessage = bulletin.Message.Trim();
			var newFrom = bulletin.From.Trim();


#if XBOX

			if (Spillville.Gamer != default(SignedInGamer))
			{
				newMessage = newMessage.Replace("[gamerTag]", Spillville.Gamer.Gamertag);


				if (newFrom.Contains("[randomFriend]"))
				{
					var friend = GetRandomFriend();
					if (!friend.Gamertag.Equals(string.Empty))
					{
						var stream = friend.GetProfile().GetGamerPicture();
						var picture = Texture2D.FromStream(_graphicsDevice, stream);
						bulletin.UseLiveIDIcon(friend.Gamertag, picture);
						newFrom = newFrom.Replace("[randomFriend]", friend.Gamertag);
					}
					else
					{
						newFrom = newFrom.Replace("[randomFriend]", "FartBunnies");
					}
				}

				if(newMessage.Contains("[randomFriend]"))
				{
					var friend = GetRandomFriend();
					newFrom = newFrom.Replace("[randomFriend]", friend.Gamertag);
				}

			}
#else
			newMessage = newMessage.Replace("[gamerTag]", "ROHR");
			newMessage = newMessage.Replace("[randomFriend]", "Anonymous" );
			newFrom = newFrom.Replace("[randomFriend]", "Anonymous" );
#endif

			var regex = new Regex(@"[^\s]+", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var matches = regex.Matches(newMessage);

			var stringBuilder = new StringBuilder();

			var lineCount = 0f;

			foreach (Match match in matches)
			{
				var wordlength = _messageFont.MeasureString(match.Value + " ").X;

				if ((wordlength + lineCount) <= LineLength)
				{
					stringBuilder.Append(match + " ");
					lineCount += wordlength;
				}
				else
				{
					stringBuilder.Append("\n" + match + " ");
					lineCount = 0;
				}


			}

			bulletin.From = @"@" + newFrom.Trim();
			bulletin.Message = stringBuilder.ToString().Trim();

		}

		private void SetFont(SpriteFont fromFont, SpriteFont msgFont)
		{
			_fromFont = fromFont;
			_messageFont = msgFont;
		}

		private void ResetColors()
		{
			_iconDrawColor.A = 175;
			_lightGrawDrawColor.A = 255;
			_blackDrawColor.A = 255;
			_drawColor.A = 255;
		}

		private void AddBulletin(GameTime gameTime)
		{
			ResetColors();
			_currentBulletin = _queue.Dequeue();
			_timeRemaining = TimeSpan.FromSeconds(TimeInSeconds);
		}

		public void Update(GameTime gameTime)
		{
			if (_currentBulletin == null && _queue.Count > 0 && _gapTime.TotalSeconds > BufferTimeInSeconds)
			{
				_gapTime = TimeSpan.Zero;
				AddBulletin(gameTime);
			}
			//			else if (!FadeInSeconds.Equals(0) && TimeInSeconds - _timeRemaining.TotalSeconds <= FadeInSeconds)
			//			{
			//				var timeDif = TimeInSeconds - _timeRemaining.TotalSeconds;
			//				var color = (byte)(255 * (timeDif / FadeInSeconds));
			//				_iconDrawColor.A = (byte)(175 * (timeDif / FadeInSeconds));
			//				_lightGrawDrawColor.A = color;
			//				_blackDrawColor.A = color;
			//				_drawColor.A = color;
			//			}
			//			else if (!FadeOutSeconds.Equals(0) && _timeRemaining.TotalSeconds <= FadeOutSeconds && _timeRemaining.TotalSeconds > 0)
			//			{
			//				var color = (byte)(255 * (_timeRemaining.TotalSeconds / FadeOutSeconds));
			//				_iconDrawColor.A = (byte)(175 * (_timeRemaining.TotalSeconds / FadeOutSeconds));
			//				_lightGrawDrawColor.A = color;
			//				_blackDrawColor.A = color;
			//				_drawColor.A = color;
			//			}
			else if (_timeRemaining.TotalSeconds <= 0)
			{
				_iconDrawColor.A = 0;
				_lightGrawDrawColor.A = 0;
				_blackDrawColor.A = 0;
				_drawColor.A = 0;
				_currentBulletin = null;
			}
			_timeRemaining -= gameTime.ElapsedGameTime;


			if (_currentBulletin == null)
			{
				_gapTime += gameTime.ElapsedGameTime;
			}

		}

		public void Draw()
		{
			if (_currentBulletin != null)
			{
				_spriteBatch.Draw(_rectangle, _rectanglePossition, _drawColor);
				var scale = IconSize / _currentBulletin.BulletinIcon.Width;

				_spriteBatch.Draw(_currentBulletin.BulletinIcon, _iconPossition, null, _iconDrawColor, 0,
								  Vector2.Zero, scale, SpriteEffects.None, 0f);

				Vector2 textSize = _fromFont.MeasureString(_currentBulletin.From);

				//print from
				//_spriteBatch.DrawString(_fromFont, _currentBulletin.From, _textStartPossition, Color.Black);
				//_textStartPossition.Y -= 1;
				_spriteBatch.DrawString(_fromFont, _currentBulletin.From, _textStartPossition, _lightGrawDrawColor);
				//_textStartPossition.Y += 1;
				_textStartPossition.Y += textSize.Y;

				//print message line 1
				_spriteBatch.DrawString(_messageFont, _currentBulletin.Message, _textStartPossition, _blackDrawColor);
				_textStartPossition.Y -= 1;
				_spriteBatch.DrawString(_messageFont, _currentBulletin.Message, _textStartPossition, _drawColor);

				_textStartPossition.Y += 1;
				_textStartPossition.Y -= textSize.Y;
			}
		}
	}
}
