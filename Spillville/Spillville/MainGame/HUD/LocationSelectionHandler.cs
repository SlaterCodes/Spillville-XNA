using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Spillville.MainGame.World;
using Spillville.Models;
using Spillville.Utilities;

namespace Spillville.MainGame.HUD
{
	class LocationSelectionHandler
	{

		private static LocationSelectionHandler _instance;
		private MenuSelection _menuSelection;

		private bool _doDrawAndUpdate;
		private IObjectWithMenu _objectWithMenu;
		private MenuSelection _selection;

		private SimpleModel _simpleModel;
		private GridTile _gridTile;
		private GridTile _buildPoint;

		private GamePadState _previousGamePadState;

		private LocationSelectionHandler()
		{
			_simpleModel = SimpleModel.Instance();
		}

		public static LocationSelectionHandler Instance
		{
			get { return _instance ?? (_instance = new LocationSelectionHandler()); }
			private set { _instance = value; }
		}

		public void HandleSelection(IObjectWithMenu objectWithMenu, MenuSelection selection)
		{
			try
			{
				float scale;

				switch (selection)
				{
					case MenuSelection.UpperMenu:
						if (objectWithMenu.GetUpperMenu().PlacementModel == null)
						{
							objectWithMenu.HandleSelection(selection, null);
							Off();
							return;
						}
						_doDrawAndUpdate = true;
						_simpleModel.SetModel(objectWithMenu.GetUpperMenu().PlacementModel);
						_simpleModel.SetPostion(objectWithMenu.GetUpperMenu().BuildPoint.N.CenterPoint);
						scale = objectWithMenu.GetUpperMenu().PlacementModelScale;
						_buildPoint = objectWithMenu.GetUpperMenu().BuildPoint;
						if (scale <= 0.0f)
						{
							scale = 1;
						}
						_simpleModel.SetScale(scale);
						GameStatus.RegisterDrawableUnit(_simpleModel);
						break;
					case MenuSelection.LowerMenu:
						if (objectWithMenu.GetLowerMenu().PlacementModel == null)
						{
							objectWithMenu.HandleSelection(selection, null);
							Off();
							return;
						}
						_doDrawAndUpdate = true;
						_simpleModel.SetModel(objectWithMenu.GetLowerMenu().PlacementModel);
						_simpleModel.SetPostion(objectWithMenu.GetLowerMenu().BuildPoint.N.CenterPoint);
						scale = objectWithMenu.GetLowerMenu().PlacementModelScale;
						_buildPoint = objectWithMenu.GetLowerMenu().BuildPoint;
						if (scale <= 0.0f)
						{
							scale = 1;
						}
						_simpleModel.SetScale(scale);
						GameStatus.RegisterDrawableUnit(_simpleModel);
						break;
					case MenuSelection.LeftMenu:
						if (objectWithMenu.GetLeftMenu().PlacementModel == null)
						{
							objectWithMenu.HandleSelection(selection, null);
							Off();
							return;
						}
						_doDrawAndUpdate = true;
						_simpleModel.SetModel(objectWithMenu.GetLeftMenu().PlacementModel);
						_simpleModel.SetPostion(objectWithMenu.GetLeftMenu().BuildPoint.N.CenterPoint);
						scale = objectWithMenu.GetLeftMenu().PlacementModelScale;
						_buildPoint = objectWithMenu.GetLeftMenu().BuildPoint;
						if (scale <= 0.0f)
						{
							scale = 1;
						}
						_simpleModel.SetScale(scale);
						GameStatus.RegisterDrawableUnit(_simpleModel);
						break;
					case MenuSelection.RightMenu:
						if (objectWithMenu.GetRightMenu().PlacementModel == null)
						{
							objectWithMenu.HandleSelection(selection, null);
							Off();
							return;
						}
						_doDrawAndUpdate = true;
						_simpleModel.SetModel(objectWithMenu.GetRightMenu().PlacementModel);
						_simpleModel.SetPostion(objectWithMenu.GetRightMenu().BuildPoint.N.CenterPoint);
						scale = objectWithMenu.GetRightMenu().PlacementModelScale;
						_buildPoint = objectWithMenu.GetRightMenu().BuildPoint;
						if (scale <= 0.0f)
						{
							scale = 1;
						}
						_simpleModel.SetScale(scale);
						GameStatus.RegisterDrawableUnit(_simpleModel);
						break;
				}

				_objectWithMenu = objectWithMenu;
				_selection = selection;
				InputController.Lock = true;

				_previousGamePadState = GamePad.GetState(PlayerIndex.One);
			}
			catch(NullReferenceException)
			{
				Off();
			}
		}

		private void Off()
		{
			_doDrawAndUpdate = false;
			_objectWithMenu = null;
			GameStatus.UnRegisterDrawableUnit(_simpleModel);
			InputController.Lock = false;
		}


		public void Update()
		{
			if(!_doDrawAndUpdate)
			{
				return;
			}

			InputController.Lock = true;

			var currentGamePadState = GamePad.GetState(PlayerIndex.One);
			if (currentGamePadState.Buttons.B == ButtonState.Pressed)
			{
				Off();
				return;
			}

			_menuSelection = MenuSelection.None;

			if (!currentGamePadState.ThumbSticks.Left.Y.Equals(0.0f) || !currentGamePadState.ThumbSticks.Left.X.Equals(0.0f))
			{
				if (Math.Abs(currentGamePadState.ThumbSticks.Left.Y) >= Math.Abs(currentGamePadState.ThumbSticks.Left.X))
				{
					if (currentGamePadState.ThumbSticks.Left.Y > 0)
					{
						_simpleModel.SetPostion(_buildPoint.N.CenterPoint);
						_gridTile = _buildPoint.N;
					}
					else
					{
						_simpleModel.SetPostion(_buildPoint.S.CenterPoint);
						_gridTile = _buildPoint.S;
					}
				}
				else
				{
					if (currentGamePadState.ThumbSticks.Left.X > 0)
					{
						_simpleModel.SetPostion(_buildPoint.E.CenterPoint);
						_gridTile = _buildPoint.E;
					}
					else
					{
						_simpleModel.SetPostion(_buildPoint.W.CenterPoint);
						_gridTile = _buildPoint.W;
					}
				}
			}

			if (currentGamePadState.Buttons.A == ButtonState.Pressed && _previousGamePadState.Buttons.A == ButtonState.Released)
			{

				_objectWithMenu.HandleSelection(_selection, _gridTile);
				Off();
				return;
			}

			_previousGamePadState = currentGamePadState;

		}



	}

}
