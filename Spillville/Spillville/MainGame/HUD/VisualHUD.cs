using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spillville.MainGame.HUD;
using Spillville.Utilities;


namespace Spillville.MainGame
{
	internal class VisualHUD
	{
		#region Constants

		private const int HUDCenterSpacing = 1;
		private const int HUDCenterWidth = 65;
		private const int HUDCenterWingHeight = 65;
		private const int HUDCenterWingExpand = 65;

		private const int HUDBorderThickness = 1;

		private const float GlobeOffset = 5;
		private const float GlobeDownSet = 100;

		#endregion

		#region Variables

		private BulletinDrawer _bulletinDrawer;
		private BulletinContainer _bulletinContainer;

		private LocationSelectionHandler _locationSelectionHandler;

		private static IObjectWithMenu _menuObject;
		private static VisualHUD _instance;

		private SpriteBatch _spriteBatch;
		private static List<Message> _messages;
		private List<Message> _messageBox;

		private static SpriteFont _messageFont;
		private SpriteFont _selectionCountFont;
		private SpriteFont _spillvilleFont;

		private static bool _displayInfo;

		private int _previousPopularity;
		private int _popularityArrowDirection;

		private Color _messageColor;
		private Color _alphaWhiteColor;
		private Color _disabledColor;

		private Vector2 _viewportSize;
		private Vector2 _messageStartPos;

		private Texture2D _upperRightRectangle;
		private Vector2 _upperRight;
		private Vector2 _globeLocation;
		private Vector2 _availableFundLocation;

		private Texture2D _selectionIconRectangle;

		private Texture2D _upArrow;
		private Texture2D _downArrow;

		private Vector2 _selectionIconPossition;
		private Vector2 _countTextPossition;

		private static Texture2D _infoBox;
		private static String _infoMessage = string.Empty;
		private static Vector2 _infoLocation;
		private static String _infoTitle;
		private static string _infoSubTitle;

        private Vector2 _TimeRemainingLocation;

		#region CenterHUD

		private static Texture2D _centerMenuShape;
		private static Vector2 _centerMenuPossition;

		private static Texture2D _leftThumbstickUpIcon;
		private static Texture2D _leftThumbstickDownIcon;
		private static Texture2D _leftThumbstickRightIcon;
		private static Texture2D _leftThumbstickLeftIcon;
		private static Texture2D _leftThumbstickCenterIcon;

		private static Texture2D _wingMenuShape;
		private static Vector2 _upperMenuPossition;
		private static Vector2 _leftMenuPossition;
		private static Vector2 _lowerMenuPossition;
		private static Vector2 _rightMenuPossition;

		private static bool _showCenterHUD;

		#endregion

		private static ContentManager _content;
		private GraphicsDevice _graphicsDevice;

		private static AnimatedTexture _globeTexture;
		private const float GlobeRotation = 0;
		private float _globeWidth;
		private const float GlobeDepth = 0.5f;

		private static MenuSelection _menuSelection;

		public static readonly Dictionary<String, Texture2D> IconDictionary = new Dictionary<string, Texture2D>();

		private readonly Dictionary<String, int> _selectedModelsCount;

		private static Color _globeColor;

		public static int Percentage
		{
			set
			{
				_globeTexture.UpdateWaterColor(value);
			}
		}

		private int _rawSelectionCount = -1;

		#endregion

		#region Initialize

		public static VisualHUD Instance()
		{
			return _instance ?? (_instance = new VisualHUD());
		}

		private VisualHUD()
		{
			_selectedModelsCount = new Dictionary<string, int>();
		}

		public static void RegisterIcon(string type, string iconLocation)
		{
			if (IconDictionary.ContainsKey(type))
			{
				return;
			}
			IconDictionary.Add(type, _content.Load<Texture2D>(iconLocation));
		}

		public void Initialize(GraphicsDevice graphicsDevice)
		{
            _previousPopularity = 0;

			_graphicsDevice = graphicsDevice;
			_viewportSize = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

			_globeColor = Color.White;
			_globeColor.A = 190;

			_spriteBatch = new SpriteBatch(graphicsDevice);
			_messages = new List<Message>();
			_messageBox = new List<Message>();
			_messageColor = Color.Ivory;

			_alphaWhiteColor = Color.White;
			_alphaWhiteColor.A = 190;

			_disabledColor = Color.Gray;
			_disabledColor.A = 150;

			_upperRightRectangle = HUDComponents.CreateRectangle(_graphicsDevice, 120, 200, HUDBorderThickness, -1);
			_globeWidth = _upperRightRectangle.Width - (HUDBorderThickness*2);
			_upperRight = new Vector2((_viewportSize.X-7-_upperRightRectangle.Width), 7);
			_globeLocation = new Vector2((_viewportSize.X - GlobeOffset - _upperRightRectangle.Width), 10);
			_availableFundLocation = new Vector2((_viewportSize.X - _upperRightRectangle.Width), _upperRight.Y + GlobeDownSet);

			_globeTexture = new AnimatedTexture(Vector2.Zero,
												 GlobeRotation, _globeWidth, GlobeDepth);



			_selectionIconRectangle = HUDComponents.CreateRectangle(_graphicsDevice, 45, 45, 1, -1);
			_selectionIconPossition = new Vector2((_viewportSize.X / 2),
												  (_viewportSize.Y - _selectionIconRectangle.Height - 15));
			_countTextPossition = new Vector2((_viewportSize.X / 2),
											  (_viewportSize.Y - 15));

			_centerMenuShape = HUDComponents.CreateRectangle(_graphicsDevice, HUDCenterWidth, HUDCenterWidth, HUDBorderThickness, 201);
			_centerMenuPossition = new Vector2((_viewportSize.X / 2) - (HUDCenterWidth / 2),
											   (_viewportSize.Y / 2) - (HUDCenterWidth / 2));

			_wingMenuShape = CreateUpperMenu(HUDCenterWidth, HUDCenterWingHeight, HUDBorderThickness,
											 HUDCenterWingExpand);
			_upperMenuPossition = new Vector2(_centerMenuPossition.X - HUDCenterWingExpand,
											  _centerMenuPossition.Y - (HUDCenterWingHeight + HUDCenterSpacing));
			_lowerMenuPossition = new Vector2(_centerMenuPossition.X + HUDCenterWingExpand + HUDCenterWidth,
											  _centerMenuPossition.Y + HUDCenterWingHeight + HUDCenterSpacing +
											  HUDCenterWidth);


			_leftMenuPossition = new Vector2(_centerMenuPossition.X - HUDCenterWingHeight - HUDCenterSpacing,
											 _centerMenuPossition.Y + HUDCenterWidth + HUDCenterWingExpand);

			_rightMenuPossition =
				new Vector2(_centerMenuPossition.X + HUDCenterWingHeight + HUDCenterWidth + HUDCenterSpacing,
							_centerMenuPossition.Y - HUDCenterWingExpand);

			_infoBox = HUDComponents.CreateRectangle(_graphicsDevice, 300, 400, 1, -1);

			_infoLocation = new Vector2(graphicsDevice.Viewport.Width - 7 - _infoBox.Width, graphicsDevice.Viewport.Height - 7 - _infoBox.Height);

			_bulletinDrawer = new BulletinDrawer(_graphicsDevice,_spriteBatch);
			_bulletinContainer = new BulletinContainer();
			
			_locationSelectionHandler = LocationSelectionHandler.Instance;

            _TimeRemainingLocation = new Vector2(_graphicsDevice.Viewport.Width-250,7);
			
		}

		public void LoadContent(ContentManager content)
		{
			_content = content;
			_globeTexture.Load(content, "Textures\\Earth", 12, 6);
			_globeTexture.PaintColor.A = 190;
			_messageFont = content.Load<SpriteFont>("fonts\\MessageFont");
			_selectionCountFont = content.Load<SpriteFont>("fonts\\SelectionCountFont");
			_spillvilleFont = content.Load<SpriteFont>("fonts\\Spillville");
			_leftThumbstickUpIcon = content.Load<Texture2D>("Icons\\left-thumbstick-up");
			_leftThumbstickDownIcon = content.Load<Texture2D>("Icons\\left-thumbstick-down");
			_leftThumbstickRightIcon = content.Load<Texture2D>("Icons\\left-thumbstick-right");
			_leftThumbstickLeftIcon = content.Load<Texture2D>("Icons\\left-thumbstick-left");
			_leftThumbstickCenterIcon = content.Load<Texture2D>("Icons\\left-thumbstick-center");

			_upArrow = content.Load<Texture2D>("Icons\\UpArrow");
			_downArrow = content.Load<Texture2D>("Icons\\DownArrow");

			
			_bulletinDrawer.LoadContent(content);
			_bulletinContainer.LoadContent(content);

			//DisplayInfo("Title", "Subtitle", "This is a message. Hello little message. Why don't you be a good message and display properly.");

//			string[] bla = {
//			               	"Good", "Bad", "AnimalGood", "AnimalBad", "OilSpread", "ProperClean", "Upgrade",
//			               	"PurchaseDisperser"
//			               };
//
//			
//			
//			for(int i = 0; i < 4; i++)
//			{
//				foreach (var s in bla)
//				{
//					BulletinContainer.CallBulletin(s);
//				}
//			}

		}

		#endregion

        private void DrawRemainingTime()
        {
            String timeRemaining = string.Format("{0}:{1:00}", GameStatus.TimeRemaining.Minutes, GameStatus.TimeRemaining.Seconds);

            //var lineSize = _messageFont.MeasureString(timeRemaining);

            //_TimeRemainingLocation.X-=lineSize.X;

            if(GameStatus.TimeRemaining.Minutes<1 && GameStatus.TimeRemaining.Seconds>0)
                _spriteBatch.DrawString(_messageFont, timeRemaining, _TimeRemainingLocation, Color.Red, 0, Vector2.Zero, 2.0f, SpriteEffects.None, 0);
        }

		private void DrawInfo()
		{
			var lineSize = _messageFont.MeasureString(_infoTitle);

			_infoLocation.X += 4;

			_infoLocation.Y += 2;

			_spriteBatch.DrawString(_messageFont, _infoTitle, _infoLocation, Color.Black);
			_availableFundLocation.Y -= 1;
			_spriteBatch.DrawString(_messageFont, _infoTitle , _infoLocation, Color.White);
			_availableFundLocation.Y += 1;

			_infoLocation.Y += lineSize.Y;

			_spriteBatch.DrawString(_selectionCountFont, _infoSubTitle, _infoLocation, Color.Black);
			_availableFundLocation.Y -= 1;
			_spriteBatch.DrawString(_selectionCountFont, _infoSubTitle, _infoLocation, Color.White);
			_availableFundLocation.Y += 1;

			_infoLocation.Y += lineSize.Y;

			_spriteBatch.DrawString(_messageFont, _infoMessage, _infoLocation, Color.Black);
			_availableFundLocation.Y -= 1;
			_spriteBatch.DrawString(_messageFont, _infoMessage, _infoLocation, Color.White);
			_availableFundLocation.Y += 1;


			_infoLocation.Y -= (lineSize.Y * 2);

			_infoLocation.Y -= 2;
			_infoLocation.X -= 4;
		}
		
		public static void DisplayInfo(string title, string subtitle, string message)
		{
			if(message == null)
			{
				return;
			}

			if (!_infoMessage.Equals(message))
			{
				var regex = new Regex(@"[^\s]+", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				var matches = regex.Matches(message);

				var stringBuilder = new StringBuilder();

				var lineCount = 0f;

				foreach (Match match in matches)
				{
					var wordlength = _messageFont.MeasureString(match.Value + " ").X;

					if ((wordlength + lineCount) <= _infoBox.Width - 4)
					{
						stringBuilder.Append(match + " ");
						lineCount += wordlength;
					}
					else
					{
						stringBuilder.Append("\n" + match + " ");
						lineCount = wordlength;
					}


				}


				_infoMessage = stringBuilder.ToString().Trim();

			}

			_infoTitle = title;
			_infoSubTitle = subtitle;
			_displayInfo = true;
		}

		public static void StopDisplayingInfo()
		{
			_displayInfo = false;
		}

		public void Update(GameTime gameTime)
		{
			if(_previousPopularity != GameStatus.Populatity)
			{
                if (_previousPopularity == 0)
                    _previousPopularity = GameStatus.Populatity;

				if(_previousPopularity <= GameStatus.Populatity)
				{
					_popularityArrowDirection = 1;
				}
				else
				{
					_popularityArrowDirection = -1;
				}
				_previousPopularity = GameStatus.Populatity;
			}


			//TODO: remove old messaging system
			_messageBox.Clear();
			foreach (Message msg in _messages)
			{
				if (msg.StartTime == TimeSpan.Zero)
					msg.StartTime = gameTime.TotalGameTime;

				if ((msg.StartTime + msg.Duration) > gameTime.TotalGameTime)
				{
					_messageBox.Add(msg);
				}
			}
			_messages.Clear();

			foreach (var message in _messageBox)
			{
				_messages.Add(message);
			}

			_globeTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);

			if (_showCenterHUD)
			{
				HandleMenuInput();
			}
			_locationSelectionHandler.Update();
			_bulletinDrawer.Update(gameTime);

			if (!InputController.SelectedModels.Count.Equals(_rawSelectionCount))
			{
				UpdateSelectedModels();
				_rawSelectionCount = InputController.SelectedModels.Count;
			}
		}

		public static void DisplayMessage(String msg)
		{
			_messages.Add(new Message(msg));
		}

		public void Draw()
		{
            RasterizerState prev = _spriteBatch.GraphicsDevice.RasterizerState;
            _spriteBatch.Begin();

			DrawMessages();

			//draw upper right box
			_spriteBatch.Draw(_upperRightRectangle, _upperRight, Color.White);

			if (_showCenterHUD && _menuObject != null)
			{
				DrawCenterMenu();
			}

			if (_selectedModelsCount.Count > 0)
			{
				DrawSelectedIcons();
			}

			//draw globe
			_globeTexture.DrawFrame(_spriteBatch, _globeLocation);

			_spriteBatch.DrawString(_spillvilleFont, String.Format(@"A {0}/100", GameStatus.Populatity), _availableFundLocation, Color.Black);
			var offset = _spillvilleFont.MeasureString(String.Format(@"A {0}/100 ", GameStatus.Populatity)).X;
			
			_availableFundLocation.Y -= 1;
			_spriteBatch.DrawString(_spillvilleFont, String.Format(@"A {0}/100", GameStatus.Populatity), _availableFundLocation, Color.White);
			_availableFundLocation.Y += 1;

			_availableFundLocation.X += offset;
			if (_popularityArrowDirection == 1)
			{
				_spriteBatch.Draw(_upArrow, _availableFundLocation, null, Color.White, 0.0f,
				                  Vector2.Zero, 0.04f, SpriteEffects.None, 0f);
			}
			else if (_popularityArrowDirection == -1)
			{
				_spriteBatch.Draw(_downArrow, _availableFundLocation, null, Color.White, 0.0f,
				                  Vector2.Zero, 0.04f, SpriteEffects.None, 0f);
			}
			_availableFundLocation.X -= offset;

			_availableFundLocation.Y += 18;
			_spriteBatch.DrawString(_spillvilleFont, String.Format(@"B {0}/{1}", GameStatus.TotalUnits, GameStatus.MaxUnits), _availableFundLocation, Color.Black);
			_availableFundLocation.Y -= 1;
			_spriteBatch.DrawString(_spillvilleFont, String.Format(@"B {0}/{1}", GameStatus.TotalUnits, GameStatus.MaxUnits), _availableFundLocation, Color.White);
			_availableFundLocation.Y += 1;


			_availableFundLocation.Y += 18;
            _spriteBatch.DrawString(_spillvilleFont, String.Format(@"C {0}:{1}", GameStatus.TimeElapsed.Minutes, GameStatus.TimeElapsed.Seconds), _availableFundLocation, Color.Black);
			_availableFundLocation.Y -= 1;
            _spriteBatch.DrawString(_spillvilleFont, String.Format(@"C {0}:{1}", GameStatus.TimeElapsed.Minutes, GameStatus.TimeElapsed.Seconds), _availableFundLocation, Color.White);
			_availableFundLocation.Y += 1;

			_availableFundLocation.Y += 18;
			_spriteBatch.DrawString(_spillvilleFont, String.Format("${0:0,0}", GameStatus.AvailableFunds), _availableFundLocation, Color.Black);
			_availableFundLocation.Y -= 1;
			_spriteBatch.DrawString(_spillvilleFont, String.Format("${0:0,0}", GameStatus.AvailableFunds), _availableFundLocation, Color.White);
			_availableFundLocation.Y += 1;

			_availableFundLocation.Y -= 54;


			if (_displayInfo)
			{
				_spriteBatch.Draw(_infoBox, _infoLocation, null, Color.White, 0.0f,
				                  Vector2.Zero, 1, SpriteEffects.None, 0f);
			DrawInfo();

			}


			_bulletinDrawer.Draw();

            DrawRemainingTime();

			_spriteBatch.End();
            _spriteBatch.GraphicsDevice.RasterizerState = prev;

		}

		private void DrawMessages()
		{
			_messageStartPos.X = 20;
			_messageStartPos.Y = _viewportSize.Y - 10 - _messageFont.MeasureString(@"TestString").Y;

			foreach (Message msg in _messages)
			{
				Vector2 textSize = _messageFont.MeasureString(msg.Text);


				_spriteBatch.DrawString(_messageFont, msg.Text, _messageStartPos, Color.Black);
				_messageStartPos.Y -= 1;
				_spriteBatch.DrawString(_messageFont, msg.Text, _messageStartPos, _messageColor);
				_messageStartPos.Y -= textSize.Y;
			}
		}

		private void DrawCenterMenu()
		{
			_spriteBatch.Draw(_centerMenuShape, _centerMenuPossition, Color.White);
			var centerIconScale = HUDCenterWidth / (float)_leftThumbstickCenterIcon.Width;
			_centerMenuPossition.X += HUDBorderThickness;
			_centerMenuPossition.Y += HUDBorderThickness;
			switch (_menuSelection)
			{
				case MenuSelection.UpperMenu:
					_spriteBatch.Draw(_leftThumbstickUpIcon, _centerMenuPossition, null, Color.White, 0.0f,
									  Vector2.Zero, centerIconScale * 0.9f, SpriteEffects.None, 0f);
					break;
				case MenuSelection.RightMenu:
					_spriteBatch.Draw(_leftThumbstickRightIcon, _centerMenuPossition, null, Color.White, 0.0f,
									  Vector2.Zero, centerIconScale * 0.9f, SpriteEffects.None, 0f);
					break;
				case MenuSelection.LowerMenu:
					_spriteBatch.Draw(_leftThumbstickDownIcon, _centerMenuPossition, null, Color.White, 0.0f,
									  Vector2.Zero, centerIconScale * 0.9f, SpriteEffects.None, 0f);
					break;
				case MenuSelection.LeftMenu:
					_spriteBatch.Draw(_leftThumbstickLeftIcon, _centerMenuPossition, null, Color.White, 0.0f,
									  Vector2.Zero, centerIconScale * 0.9f, SpriteEffects.None, 0f);
					break;
				default:
					_spriteBatch.Draw(_leftThumbstickCenterIcon, _centerMenuPossition, null, Color.White, 0.0f,
									  Vector2.Zero, centerIconScale * 0.9f, SpriteEffects.None, 0f);
					StopDisplayingInfo();
					break;
			}

			_centerMenuPossition.X -= HUDBorderThickness;
			_centerMenuPossition.Y -= HUDBorderThickness;

			//draw upper
			if (_menuObject.GetUpperMenu() != null)
			{
				if (!_menuObject.GetUpperMenu().Enabled)
				{
					_spriteBatch.Draw(_wingMenuShape, _upperMenuPossition, _disabledColor);
				}
				else if (_menuSelection == MenuSelection.UpperMenu && _menuObject.GetUpperMenu().Selectable)
				{
					_spriteBatch.Draw(_wingMenuShape, _upperMenuPossition, Color.Blue);
					DisplayInfo(_menuObject.GetUpperMenu().InfoTitle, _menuObject.GetUpperMenu().InfoSubTitle, _menuObject.GetUpperMenu().InfoMessage);
				}
				else
				{
					_spriteBatch.Draw(_wingMenuShape, _upperMenuPossition, Color.White);
				}
				if (_menuObject.GetUpperMenu().Image != null)
				{
					var scale = HUDCenterWingHeight / (float)_menuObject.GetUpperMenu().Image.Width;
					var offset = (HUDCenterWidth - (_menuObject.GetUpperMenu().Image.Width * scale)) / 2.0f;
					_upperMenuPossition.X += HUDCenterWingExpand + offset;
					_spriteBatch.Draw(_menuObject.GetUpperMenu().Image, _upperMenuPossition, null, _alphaWhiteColor, 0.0f,
									  Vector2.Zero, scale, SpriteEffects.None, 0f);
					Vector2 textSize = _selectionCountFont.MeasureString(_menuObject.GetUpperMenu().Name);
					_upperMenuPossition.Y += HUDCenterWingHeight - textSize.Y - 5;
					_spriteBatch.DrawString(_selectionCountFont, _menuObject.GetUpperMenu().Name, _upperMenuPossition, Color.Black);
					_upperMenuPossition.Y -= 1;
					_spriteBatch.DrawString(_selectionCountFont, _menuObject.GetUpperMenu().Name, _upperMenuPossition, Color.White);
					_upperMenuPossition.X -= HUDCenterWingExpand + offset;
					_upperMenuPossition.Y -= HUDCenterWingHeight - textSize.Y - 6;
				}
			}

			//draw right
			if (_menuObject.GetRightMenu() != null)
			{
				if (!_menuObject.GetRightMenu().Enabled)
				{
					_spriteBatch.Draw(_wingMenuShape, _rightMenuPossition, null, _disabledColor, MathHelper.ToRadians(90),
									  Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
				}
				else if (_menuSelection == MenuSelection.RightMenu && _menuObject.GetRightMenu().Selectable)
				{
					_spriteBatch.Draw(_wingMenuShape, _rightMenuPossition, null, Color.Blue, MathHelper.ToRadians(90),
									  Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
					DisplayInfo(_menuObject.GetRightMenu().InfoTitle, _menuObject.GetRightMenu().InfoSubTitle, _menuObject.GetRightMenu().InfoMessage);
				}
				else
				{
					_spriteBatch.Draw(_wingMenuShape, _rightMenuPossition, null, Color.White, MathHelper.ToRadians(90),
									  Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
				}
				if (_menuObject.GetRightMenu().Image != null)
				{
					var scale = (HUDCenterWingHeight) / (float)_menuObject.GetRightMenu().Image.Width;
					var offset = (HUDCenterWidth - (_menuObject.GetRightMenu().Image.Height * scale)) / 2.0f;
					_rightMenuPossition.X -= HUDCenterWingExpand;
					_rightMenuPossition.Y += HUDCenterWingExpand + offset;

					if (_menuObject.GetRightMenu().Enabled)
					{
						_spriteBatch.Draw(_menuObject.GetRightMenu().Image, _rightMenuPossition, null, _alphaWhiteColor, 0.0f,
										  Vector2.Zero, scale, SpriteEffects.None, 0f);
					}
					else
					{
						_spriteBatch.Draw(_menuObject.GetRightMenu().Image, _rightMenuPossition, null, _disabledColor, 0.0f,
										  Vector2.Zero, scale, SpriteEffects.None, 0f);
					}

					_rightMenuPossition.Y -= offset;

					//draw text vertically
					var textVertOffset = 0.0f;
					_rightMenuPossition.Y += (_menuObject.GetRightMenu().Image.Height * scale) + 8.0f;
					foreach (var s in _menuObject.GetRightMenu().Name.Split())
					{
						var textSize = _selectionCountFont.MeasureString(s);
						_rightMenuPossition.X += (HUDCenterWingHeight - textSize.X) / 2.0f;
						_spriteBatch.DrawString(_selectionCountFont, s, _rightMenuPossition, Color.Black);
						_rightMenuPossition.Y -= 1;
						textVertOffset -= 1;
						_spriteBatch.DrawString(_selectionCountFont, s, _rightMenuPossition, Color.White);
						_rightMenuPossition.Y += textSize.Y - 2.0f;
						textVertOffset += textSize.Y - 2.0f;
						_rightMenuPossition.X -= (HUDCenterWingHeight - textSize.X) / 2.0f;
					}
					_rightMenuPossition.Y -= (_menuObject.GetRightMenu().Image.Height * scale) + 8.0f;
					_rightMenuPossition.Y -= textVertOffset;
					_rightMenuPossition.X += HUDCenterWingExpand;
					_rightMenuPossition.Y -= HUDCenterWingExpand;
				}
			}

			//draw lower
			if (_menuObject.GetLowerMenu() != null)
			{
				if (!_menuObject.GetLowerMenu().Enabled)
				{
					_spriteBatch.Draw(_wingMenuShape, _lowerMenuPossition, null, _disabledColor, MathHelper.ToRadians(180),
									  Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
				}
				else if (_menuSelection == MenuSelection.LowerMenu && _menuObject.GetLowerMenu().Selectable)
				{
					_spriteBatch.Draw(_wingMenuShape, _lowerMenuPossition, null, Color.Blue, MathHelper.ToRadians(180),
									  Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
					DisplayInfo(_menuObject.GetLowerMenu().InfoTitle, _menuObject.GetLowerMenu().InfoSubTitle, _menuObject.GetLowerMenu().InfoMessage);
				}
				else
				{
					_spriteBatch.Draw(_wingMenuShape, _lowerMenuPossition, null, Color.White, MathHelper.ToRadians(180),
									  Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
				}
				if (_menuObject.GetLowerMenu().Image != null)
				{
					var scale = HUDCenterWingHeight / (float)_menuObject.GetLowerMenu().Image.Width;
					var offset = (HUDCenterWidth - (_menuObject.GetLowerMenu().Image.Width * scale)) / 2.0f;
					_lowerMenuPossition.X -= HUDCenterWingExpand + offset + HUDCenterWidth;
					_lowerMenuPossition.Y -= HUDCenterWidth;
					if (_menuObject.GetLowerMenu().Enabled)
					{
						_spriteBatch.Draw(_menuObject.GetLowerMenu().Image, _lowerMenuPossition, null, _alphaWhiteColor, 0.0f,
										  Vector2.Zero, scale, SpriteEffects.None, 0f);
					}
					else
					{
						_spriteBatch.Draw(_menuObject.GetLowerMenu().Image, _lowerMenuPossition, null, _disabledColor, 0.0f,
										  Vector2.Zero, scale, SpriteEffects.None, 0f);
					}
					Vector2 textSize = _selectionCountFont.MeasureString(_menuObject.GetLowerMenu().Name);
					_lowerMenuPossition.Y += HUDCenterWingHeight - textSize.Y - 5;
					_spriteBatch.DrawString(_selectionCountFont, _menuObject.GetLowerMenu().Name, _lowerMenuPossition, Color.Black);
					_lowerMenuPossition.Y -= 1;
					_spriteBatch.DrawString(_selectionCountFont, _menuObject.GetLowerMenu().Name, _lowerMenuPossition, Color.White);
					_lowerMenuPossition.Y += HUDCenterWidth;
					_lowerMenuPossition.X += HUDCenterWingExpand + offset + HUDCenterWidth;
					_lowerMenuPossition.Y -= HUDCenterWingHeight - textSize.Y - 6;
				}
			}

			//draw left
			if (_menuObject.GetLeftMenu() != null)
			{
				if (!_menuObject.GetLeftMenu().Enabled)
				{
					_spriteBatch.Draw(_wingMenuShape, _leftMenuPossition, null, _disabledColor, MathHelper.ToRadians(-90),
									  Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
				}
				else if (_menuSelection == MenuSelection.LeftMenu && _menuObject.GetLeftMenu().Selectable)
				{
					_spriteBatch.Draw(_wingMenuShape, _leftMenuPossition, null, Color.Blue, MathHelper.ToRadians(-90),
									  Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
					DisplayInfo(_menuObject.GetLeftMenu().InfoTitle, _menuObject.GetLeftMenu().InfoSubTitle, _menuObject.GetLeftMenu().InfoMessage);
				}
				else
				{
					_spriteBatch.Draw(_wingMenuShape, _leftMenuPossition, null, Color.White, MathHelper.ToRadians(-90),
									  Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
				}
				if (_menuObject.GetLeftMenu().Image != null)
				{
					var scale = (HUDCenterWingHeight) / (float)_menuObject.GetLeftMenu().Image.Width;
					var offset = (HUDCenterWidth - (_menuObject.GetLeftMenu().Image.Height * scale)) / 2.0f;
					_leftMenuPossition.Y -= (HUDCenterWidth / 2.0f) + HUDCenterWingExpand + offset + offset;

					_spriteBatch.Draw(_menuObject.GetLeftMenu().Image, _leftMenuPossition, null, _alphaWhiteColor, 0.0f,
									  Vector2.Zero, scale, SpriteEffects.None, 0f);
					_leftMenuPossition.Y -= 10;

					//draw text vertically
					var textVertOffset = 0.0f;
					_leftMenuPossition.Y += (_menuObject.GetLeftMenu().Image.Height * scale) + 8.0f;
					foreach (var s in _menuObject.GetLeftMenu().Name.Split())
					{
						var textSize = _selectionCountFont.MeasureString(s);
						_leftMenuPossition.X += (HUDCenterWingHeight - textSize.X) / 2.0f;
						_spriteBatch.DrawString(_selectionCountFont, s, _leftMenuPossition, Color.Black);
						_leftMenuPossition.Y -= 1;
						textVertOffset -= 1;
						_spriteBatch.DrawString(_selectionCountFont, s, _leftMenuPossition, Color.White);
						_leftMenuPossition.Y += textSize.Y - 2.0f;
						textVertOffset += textSize.Y - 2.0f;
						_leftMenuPossition.X -= (HUDCenterWingHeight - textSize.X) / 2.0f;
					}
					_leftMenuPossition.Y += offset + offset + 10;
					_leftMenuPossition.Y -= (_menuObject.GetLeftMenu().Image.Height * scale) + 8.0f;
					_leftMenuPossition.Y -= textVertOffset;
					_leftMenuPossition.Y += (HUDCenterWidth / 2.0f) + HUDCenterWingExpand;
				}
			}
		}

		private void HandleMenuInput()
		{
			var currentGamePadState = GamePad.GetState(PlayerIndex.One);

			if (currentGamePadState.Buttons.B == ButtonState.Pressed)
			{
				_showCenterHUD = false;
				InputController.Lock = false;
                StopDisplayingInfo();
				return;
			}

			_menuSelection = MenuSelection.None;

			if (!currentGamePadState.ThumbSticks.Left.Y.Equals(0.0f) || !currentGamePadState.ThumbSticks.Left.X.Equals(0.0f))
			{
				if (Math.Abs(currentGamePadState.ThumbSticks.Left.Y) >= Math.Abs(currentGamePadState.ThumbSticks.Left.X))
				{
					if (currentGamePadState.ThumbSticks.Left.Y > 0)
					{
						_menuSelection = MenuSelection.UpperMenu;
					}
					else
					{
						_menuSelection = MenuSelection.LowerMenu;
					}
				}
				else
				{
					if (currentGamePadState.ThumbSticks.Left.X > 0)
					{
						_menuSelection = MenuSelection.RightMenu;
					}
					else
					{
						_menuSelection = MenuSelection.LeftMenu;
					}
				}
			}

			if (currentGamePadState.Buttons.A == ButtonState.Pressed)
			{
                StopDisplayingInfo();
				_locationSelectionHandler.HandleSelection(_menuObject,_menuSelection);
				
				_showCenterHUD = false;
				InputController.Lock = false;
			}
		}

		private void DrawSelectedIcons()
		{
			_selectionIconPossition.X = _viewportSize.X / 2.0f;
			_selectionIconPossition.X -= ((_selectionIconRectangle.Width * _selectedModelsCount.Count) +
										  (HUDCenterSpacing * _selectedModelsCount.Count)) / 2.0f;
			foreach (var selectedModel in _selectedModelsCount)
			{
				_spriteBatch.Draw(_selectionIconRectangle, _selectionIconPossition, Color.White);

				var scale = _selectionIconRectangle.Width / (float)IconDictionary[selectedModel.Key].Width;
				var offset = (_selectionIconRectangle.Height - (IconDictionary[selectedModel.Key].Height * scale)) / 2.0f;
				_selectionIconPossition.Y += offset;
				_spriteBatch.Draw(IconDictionary[selectedModel.Key], _selectionIconPossition, null, _alphaWhiteColor, 0,
								  Vector2.Zero, scale, SpriteEffects.None, 0f);
				_selectionIconPossition.Y -= offset;
				if (selectedModel.Value > 1)
				{
					string countString = "x" + selectedModel.Value;
					Vector2 textSize = _selectionCountFont.MeasureString(countString);
					_countTextPossition.X = _selectionIconPossition.X + _selectionIconRectangle.Width - textSize.X - 3;
					_countTextPossition.Y = _selectionIconPossition.Y + _selectionIconRectangle.Height - textSize.Y - 1;
					_spriteBatch.DrawString(_selectionCountFont, countString, _countTextPossition, Color.Black);
					_countTextPossition.Y -= 1;
					_spriteBatch.DrawString(_selectionCountFont, countString, _countTextPossition, Color.White);
				}

				_selectionIconPossition.X += _selectionIconRectangle.Width + HUDCenterSpacing;
			}
		}

		private void UpdateSelectedModels()
		{
			_selectedModelsCount.Clear();
			foreach (var selectedModel in InputController.SelectedModels)
			{
				var name = selectedModel.GetType().Name;
				if (!IconDictionary.ContainsKey(name))
				{
					continue;
				}

				if (_selectedModelsCount.ContainsKey(name))
				{
					_selectedModelsCount[name]++;
				}
				else
				{
					_selectedModelsCount.Add(name, 1);
				}
			}
		}

		public static void ShowMenu(IObjectWithMenu objectWithMenu)
		{
			_menuObject = objectWithMenu;
			_showCenterHUD = true;
			InputController.Lock = true;
		}


		private Texture2D CreateUpperMenu(int width, int height, int border, int cutOffset)
		{
			width += cutOffset * 2;
			Texture2D rect = new Texture2D(_graphicsDevice, width, height);

			Color[] data = new Color[width * height];
			float clear = 0.0f;
			float gradient = width * 1.4f;

			for (int i = 0; i < data.Length; ++i)
			{
				if ((i % width) < (int)clear || (i % width) > width - (int)clear)
				{
					data[i].A = 0;
				}
				//draw border
				else if ((i % width) < (int)clear + border || (i % width) > width - (int)clear - border || i < width * border ||
						 i > (width * height) - (width * border))
				{
					data[i] = Color.Black;
					data[i].A = 175;
				}
				else
				{
					data[i] = Color.Green;
					data[i].A = 175;
					data[i].G = (byte)(128 - gradient);
				}
				if ((i % width) == 0)
				{
					clear += (cutOffset / (float)height);
					gradient -= 1.4f;
				}
			}
			rect.SetData(data);

			return rect;
		}
	}
}