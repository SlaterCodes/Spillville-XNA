using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Models;
using Spillville.MainGame;

namespace Spillville.Utilities
{
	/// <summary>
	/// Input Controller handles all functionality related to the user input and provides a level of abstraction
	/// between the UI controls and actions of various classes. It also has built in mechanisms for handling model
	/// selection and camera controls. Input controller is a singleton and should be accessed by the static Instance()
	/// method.
	/// </summary>
	internal class InputController
	{
		private const float ZMax = 6427f;
		private const float ZMin = -7065f;
		private const float XMax = 5732f;
		private const float XMin = -6891f;

		#region Class Variables

		private bool _cursorRegistered;
		public static bool Lock;

		//private static int _percent = 8;
		private static InputController _instance;

		private float _zoomDistance;

		public static List<InputSelectable> SelectedModels;


		private TimeSpan _destinationSelectionTime;

		private Indicator _moveIndicator;
		private Cursor _cursor;

		private TimeSpan _previousButtonPressTime;

		private Vector3 _normalDrawColor;
		private Vector3 _selectedColor;

		private GamePadState _previousGamePadState;
		#endregion

		#region SetUp Methods

		public void Initialize(Model cursorModel, Model indicatorModel, GamePlay gamePlay, Spillville spillville)
		{

			_previousButtonPressTime = TimeSpan.Zero;
			_moveIndicator = new Indicator();
			_cursor = new Cursor();


			_moveIndicator.Initialize(indicatorModel);
			_cursor.Initialize(cursorModel);

			_normalDrawColor = Color.Transparent.ToVector3();
			_selectedColor = Color.Green.ToVector3();

		}

		public void Register()
		{
			GameStatus.RegisterDrawableUnit(_cursor);
			_cursorRegistered = true;
		}

		private InputController()
		{
			SelectedModels = new List<InputSelectable>();
			Camera.Position.Y = 1000.0f;
			Camera.Position.Z = 3000.0f;
			Camera.Target.Y = 250;
			Camera.Target.Z = 2000;

			_zoomDistance = 0;
		}

		public static InputController Instance
		{
			get { return _instance ?? (_instance = new InputController()); }
		}

		#endregion

		private void ClearSelection()
		{
			foreach (var modelObject in SelectedModels)
			{
				modelObject.EmissiveColor = _normalDrawColor;
				modelObject.InputDeselected();
			}
			SelectedModels.Clear();
		}

		public void HandleInput(GamePadState currentGamePadState, KeyboardState currentKeyboardState, GameTime gameTime)
		{
			if (currentKeyboardState.IsKeyDown(Keys.F11))
			{
				Spillville.Graphics.ToggleFullScreen();
			}

			if (Lock)
			{
				if (_cursorRegistered)
				{
					GameStatus.UnRegisterDrawableUnit(_cursor);
					_cursorRegistered = false;
				}
				return;
			}

			if (!_cursorRegistered)
			{
				GameStatus.RegisterDrawableUnit(_cursor);
				_cursorRegistered = true;
			}

			#region Selection


			if ((currentGamePadState.Buttons.A == ButtonState.Pressed && _previousGamePadState.Buttons.A != ButtonState.Pressed) || currentKeyboardState.IsKeyDown(Keys.Q))
			{


				var inputSelection = (InputSelectable)ModelCollider.GetCollidedModels(_cursor).FirstOrDefault(selection => selection is InputSelectable);
				if (inputSelection != default(InputSelectable))
				{
					Debug.WriteLine("Model selected: " + inputSelection);
					inputSelection.EmissiveColor = _selectedColor;
					if (!SelectedModels.Contains(inputSelection))
					{
						ClearSelection();
						inputSelection.InputSelected();
						SelectedModels.Add(inputSelection);
					}
				}

				//			                foreach (var selection in ModelCollider.GetCollidedModels(_cursor).Where(selection => selection is InputSelectable))
				//			                {
				//								Debug.WriteLine("Model selected: " + selection);
				//			                	selection.EmissiveColor = _selectedColor;
				//                                var inputSelection = (InputSelectable)selection;
				//                                if (!SelectedModels.Contains(inputSelection))
				//			                	{
				//                                    inputSelection.InputSelected();
				//                                    SelectedModels.Add(inputSelection);
				//			                	}
				//			                }



			}




			//deselect
			if (currentGamePadState.Buttons.B == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.E))
			{
				ClearSelection();
			}

#if DEBUG
			if (currentKeyboardState.IsKeyDown(Keys.OemQuestion))
			{
				Debug.WriteLine("X:{0}\tZ:{1}", Camera.Target.X, Camera.Target.Z);
			}
#endif

			#endregion

			_previousButtonPressTime += gameTime.ElapsedGameTime;

			#region Move/Action

			if (currentGamePadState.Buttons.X == ButtonState.Pressed && _previousGamePadState.Buttons.X != ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.X))
			{
				foreach (var selectedModel in SelectedModels)
				{
					selectedModel.InputHandleActionX(Camera.Target);
				}
			}



			if (currentGamePadState.Buttons.Y == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Tab))
			{
				DisplayMenuForSelectedObjectWithMenues();
			}




			//			            if (currentGamePadState.Buttons.X == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.X))
			//			            {
			//			                //no objects were targeted for action, empty area clicked so move selection to that possition
			//			                if (GetCollision().Count == 0)
			//			                {
			//			                    foreach (var selectedModel in SelectedModels)
			//			                    {
			//			
			//			                        /*if (OilSpill.HasOil((int)ModelPosition.X, (int)ModelPosition.Z))
			//			                        {
			//			                            selectedModel.Value.PerformActionOnTarget(ModelPosition);
			//			                        }*/                        
			//			                        
			//			                        _moveIndicator.AutoDrawn = true;
			//			                        //_moveIndicator.ModelPosition.X = ModelPosition.X;
			//			                        //_moveIndicator.ModelPosition.Z = ModelPosition.Z;
			//			                        _moveIndicator.ModelPosition = new Vector3(ModelPosition.X, _moveIndicator.ModelPosition.Y, ModelPosition.Z);
			//			                        _destinationSelectionTime = TimeSpan.Zero;
			//			                        //selectedModel.Value.ModelDestination.X = ModelPosition.X;
			//			                        //selectedModel.Value.ModelDestination.Z = ModelPosition.Z;
			//			                        // We want to avoid manually changing class instance variables!!!
			//			                        // Use functions so you can tie into "actions" and avoid duplicate code
			//			                        // this also allows you to do error checking 1nce in the class
			//			                        selectedModel.Value.SetDestination(ModelPosition);
			//			                    }
			//			                }
			//			                    //pass the targetted object to the selection class
			//			                else
			//			                {
			//			                    foreach (var selectedModel in SelectedModels)
			//			                    {
			//			                        selectedModel.Value.PerformActionOnTarget(GetCollision()[0]);
			//			                    }
			//			                }
			//			            }

			#endregion

			#region Camera Control

			//Camera Movement
			float multiplier = (currentGamePadState.Triggers.Left + 1) * 3.0f;

			#region keyboardStuff


			if (currentGamePadState.DPad.Right == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Right))
			{
				TranslateCamera(1, 0, multiplier);
			}
			if (currentGamePadState.DPad.Left == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Left))
			{
				TranslateCamera(-1, 0, multiplier);
			}
			if (currentGamePadState.DPad.Down == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Down))
			{
				TranslateCamera(0, -1, multiplier);
			}
			if (currentGamePadState.DPad.Up == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Up))
			{
				TranslateCamera(0, 1, multiplier);
			}

			if (currentKeyboardState.IsKeyDown(Keys.R))
				Camera.Revolve(Matrix.Identity.Up, (float)(0.005f * Math.PI));
			if (currentKeyboardState.IsKeyDown(Keys.T))
				Camera.Revolve(Matrix.Identity.Up, (float)(-0.005f * Math.PI));


			#endregion keyboardStuff

			#region globechange

			if (currentKeyboardState.IsKeyDown(Keys.D0))
			{
				VisualHUD.Percentage = 0;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D1))
			{
				VisualHUD.Percentage = 1;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D2))
			{
				VisualHUD.Percentage = 2;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D3))
			{
				VisualHUD.Percentage = 3;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D4))
			{
				VisualHUD.Percentage = 4;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D5))
			{
				VisualHUD.Percentage = 5;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D6))
			{
				VisualHUD.Percentage = 6;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D7))
			{
				VisualHUD.Percentage = 7;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D8))
			{
				VisualHUD.Percentage = 8;
			}
			if (currentKeyboardState.IsKeyDown(Keys.D9))
			{
				VisualHUD.Percentage = 9;
			}

			#endregion


			TranslateCamera(currentGamePadState.ThumbSticks.Left.X, currentGamePadState.ThumbSticks.Left.Y, multiplier);


			//Zoom
			Zoom(currentGamePadState.ThumbSticks.Right.Y);

			if (currentKeyboardState.IsKeyDown(Keys.W))
			{
				Zoom(1.0f);
			}
			if (currentKeyboardState.IsKeyDown(Keys.S))
			{
				Zoom(-1.0f);
			}

			_cursor.SetPosition(Camera.Target);


			#endregion Camera Control

			if (_destinationSelectionTime.TotalSeconds < 1.0f)
			{
				_moveIndicator.Update(gameTime);
				_destinationSelectionTime += gameTime.ElapsedGameTime;
			}
			else
			{
				GameStatus.UnRegisterDrawableUnit(_moveIndicator);
			}

			_previousGamePadState = currentGamePadState;
		}

		private static void TranslateCamera(float x, float z, float multiplier)
		{
			var newTargetXValue = Camera.Target.X + x * 10.0f * multiplier;
			var newTargetZValue = Camera.Target.Z + z * -10.0f * multiplier;

			if (newTargetXValue > XMin && newTargetXValue < XMax)
			{
				Camera.Target.X = newTargetXValue;
				Camera.Position.X += x * 10.0f * multiplier;
			}

			if (newTargetZValue > ZMin && newTargetZValue < ZMax)
			{
				Camera.Target.Z = newTargetZValue;
				Camera.Position.Z += z * -10.0f * multiplier;
			}

		}

		private void Zoom(float multiplier)
		{
			var newCamTargetY = Camera.Target.Y + (multiplier * 5.0f);
			var newCamPosY = Camera.Position.Y - (multiplier * 5.0f);

			if ((newCamTargetY + 350.0f) < newCamPosY && (_zoomDistance < 1200 || multiplier > 0))
			{
				Camera.Target.Y = newCamTargetY;
				Camera.Position.Y = newCamPosY;
				_zoomDistance -= multiplier * 10.0f;
				if (Math.Abs(_zoomDistance) < 1000)
				{
					Camera.Position.Z -= multiplier * (10.0f - (10.0f * (_zoomDistance / 1000.0f)));
				}
			}
			_cursor.SetPosition(Camera.Target);
		}

		private void DisplayMenuForSelectedObjectWithMenues()
		{

			if (SelectedModels.Count > 0)
			{
				if (SelectedModels[0] is IObjectWithMenu)
				{
					VisualHUD.ShowMenu((IObjectWithMenu)SelectedModels[0]);
				}
			}
		}

	}
}