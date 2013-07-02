using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.MainGame;
using Spillville.Utilities;
using Spillville.MainGame.World;
using System.Collections.Generic;
using Spillville.MainGame.HUD;
using Spillville.MainGame.Levels;

namespace Spillville.Models.Boats
{
	public abstract class Boat : IDrawableModel, InputSelectable, IObjectWithMenu //ModelObject
	{
		public string BoatType { get; protected set; }

		public Model ModelObject { get; protected set; }
		public Vector3 ModelRotation { get; protected set; }

		protected Vector3 _modelPosition;


		public Vector3 ModelPosition
		{
			get { return _modelPosition; }
			protected set
			{
				_modelPosition = value;
				IsBoundingBoxUpToDate = false;
			}
		}

		public Matrix[] boneTransforms { get; private set; }
		public float ModelScale { get; protected set; }
		public Vector3 EmissiveColor { get; set; }
		public BoundingBox boundingBox { get; protected set; }

		public int Capacity { get; protected set; }
		public int TankLevel { get; protected set; }

		public float CurrentSpeed { get; private set; }
		public float MaxSpeed { get; protected set; }
		public float MinSpeed { get; private set; }
		public bool IsMoving { get; private set; }
		protected short AccelerationDistanceThreshold;
		private short _accelerationFactor;
		private Vector3 _destinationPosition;
		public Vector3 DestinationPosition
		{
			get { return _destinationPosition; }
		}
		private TimeSpan _lastBoundingBoxUpdate;
		private TimeSpan _lastAccelerationTime;
		private TimeSpan _timeBetweenAcceleration;
		private Vector2 _tempPositionCalc;
		public GridTile CurrentTile { get; protected set; }
		protected Queue<GridTile> JobQueue;

		//Draw stuff
		protected Texture2D WhiteBox;
		protected Texture2D BlackBox;
		private Vector3 _screenCoords;
		protected bool SpeedUpgrade;
        protected Vector3 TargetLocation;

        protected int popularityTicker = 0;
        protected string currentMessage = "";
        protected Color currentMessageColor = Color.Red;

		protected virtual void RemoveBoat()
		{
			//TODO: possibly destroy boats better
			CurrentTile.OccupiedBoat = null;
			GameStatus.UnRegisterDrawableUnit(this);
			GameStatus.BuildUnits(-1);
		}

		public virtual void Initialize(Vector2 position)
		{
			boneTransforms = ModelDrawer.GetBoneTransforms(ModelObject);
			ModelPosition = new Vector3(position.X, 25, position.Y);
			_tempPositionCalc = position;
			DoesCollision = true;
			IsBoundingBoxUpToDate = false;
			SpeedUpgrade = false;
			JobQueue = new Queue<GridTile>();
			CurrentTile = WorldGrid.GridTileAtLocation(position);
			CurrentTile.OccupiedBoat = this;
			_destinationPosition = Vector3.Zero;
			_lastBoundingBoxUpdate = TimeSpan.Zero;
			CurrentSpeed = 0;
			MinSpeed = 3;
			IsMoving = false;
			_accelerationFactor = 1;
			// TODO, change this based on algorithm based on MaxSpeed and Distance
			AccelerationDistanceThreshold = 2000;
			_timeBetweenAcceleration = TimeSpan.FromMilliseconds(300);

			_screenCoords = Vector3.Zero;
			if (Capacity > 0)
			{
				WhiteBox = HealthBar.CreateBox(Capacity * 5, 10, Color.White, Color.Red);
				BlackBox = HealthBar.CreateBox(1, 10, Color.Black, Color.Red);
			}
			else
			{
				WhiteBox = HealthBar.CreateBox(1, 1, Color.White, Color.Red);
				BlackBox = HealthBar.CreateBox(1, 1, Color.Black, Color.Red);
			}
            UpdateBoundingBox();

            if (!Tutorials.TutorialCleanOilHelped)
            {
                if (this.BoatType == "Disperser" || this.BoatType == "Cleaner")
                {
                    BulletinContainer.CallBulletin("Tuturial.CleanOil");
                    Tutorials.TutorialCleanOilHelped = true;
                }
            }

		}

		protected void UpdateModelObject(String name)
		{
			ModelObject = ModelFactory.Get(name);
			boneTransforms = ModelDrawer.GetBoneTransforms(ModelObject);
		}

		public void Move(Vector3 newPosition)
		{
			Move(WorldGrid.GridTileAtLocation(newPosition));
		}

		public void Move(GridTile futureTile)
		{
			if (!futureTile.HasBarricade && !futureTile.HasBoat)
			{
				// TODO should we use the tile center point?
				//DestinationPosition = newPosition;
				_destinationPosition.X = futureTile.CenterPoint.X;
				_destinationPosition.Z = futureTile.CenterPoint.Y;
				IsMoving = true;
				_lastAccelerationTime = TimeSpan.Zero;
			}
		}

		public virtual void Update(GameTime gameTime)
		{
			//_modelPosition.Y += WaterShader.GetWaveHeight(_modelPosition.Z);
            _modelPosition.Y = MathHelper.Clamp(WaterShader.GetWaveHeight(_modelPosition.Z)+_modelPosition.Y , 10, 35);
			if (IsMoving)
			{
				MoveStep(gameTime);
			}

			if (!IsBoundingBoxUpToDate && gameTime.TotalGameTime - _lastBoundingBoxUpdate > TimeSpan.FromMilliseconds(800))
			{
				_lastBoundingBoxUpdate = gameTime.TotalGameTime;
				UpdateBoundingBox();
				//System.Diagnostics.Debug.WriteLine("BoudingBox updated");
			}
		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
		}

		public void DrawHealthBar(SpriteBatch spriteBatch)
		{
			RasterizerState prev = spriteBatch.GraphicsDevice.RasterizerState;
			_screenCoords = spriteBatch.GraphicsDevice.Viewport.Project(this.ModelPosition, Camera.Projection, Camera.View, Matrix.Identity);
			_screenCoords.X -= (float)(Capacity * 2.5);
			_screenCoords.Y -= 45;
			spriteBatch.Begin();
			spriteBatch.Draw(WhiteBox,
				new Vector2(_screenCoords.X, _screenCoords.Y),
				Color.White);
			spriteBatch.Draw(BlackBox,
				new Vector2(_screenCoords.X, _screenCoords.Y),
				Color.Wheat);
			spriteBatch.End();
			spriteBatch.GraphicsDevice.RasterizerState = prev;
		}

        public void DrawPopularityEffect(SpriteBatch spriteBatch, string message, Color color)
        {
            RasterizerState prev = spriteBatch.GraphicsDevice.RasterizerState;
            _screenCoords = spriteBatch.GraphicsDevice.Viewport.Project(this.ModelPosition, Camera.Projection, Camera.View, Matrix.Identity);
            _screenCoords.X -= (float)(Capacity * 2.5);
            _screenCoords.Y -= 70;
            spriteBatch.Begin();
            spriteBatch.DrawString(HealthBar._font,
                message,
                new Vector2(_screenCoords.X, _screenCoords.Y), color);
            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = prev;
        }

		public virtual bool DoesCollision { get; protected set; }

		public virtual bool IsBoundingBoxUpToDate { get; protected set; }

		public virtual void UpdateBoundingBox()
		{
			Spillville.ThreadPool.AddTask(DoBoundingBoxUpdate
				, delegate(Task task, Exception err) { }, null);
		}

		private void DoBoundingBoxUpdate()
		{
			BoundingBox tempbox = ModelDrawer.GetBoundingBoxUsingFindBoundary(this);
			lock (this)
			{
				boundingBox = tempbox;
				IsBoundingBoxUpToDate = true;
			}
		}

		public virtual void InputSelected()
		{
		}

		public virtual void InputDeselected()
		{

		}

		public virtual void InputHandleActionX(Vector3 target)
		{
			Move(target);
		}

		// TODO this will return something... Options for when boat is selected
		public void MenuOptions()
		{

		}

		private void MoveStep(GameTime gameTime)
		{
			var distance = Helper.Distance(DestinationPosition, ModelPosition);
			var atan =
				(float)Math.Atan2((DestinationPosition.Z - ModelPosition.Z), (DestinationPosition.X - ModelPosition.X));

			bool doAcceleration = false;

			// First Time MoveStep is called this move
			if (_lastAccelerationTime.Equals(TimeSpan.Zero))
			{
				_lastAccelerationTime = gameTime.TotalGameTime;
				doAcceleration = true;
				// Rotate to face right direction
				var rotation = ModelRotation;
				rotation.Y = (-atan - MathHelper.ToRadians(90.0f));
				this.ModelRotation = rotation;
			}

			// To accelerate
			if (CurrentSpeed < MaxSpeed && distance > AccelerationDistanceThreshold && (gameTime.TotalGameTime - _lastAccelerationTime).TotalMilliseconds > _timeBetweenAcceleration.TotalMilliseconds)
			{
				doAcceleration = true;
				_accelerationFactor = (short)(SpeedUpgrade ? 2 : 1);
			}

			// Check to see if at min speed && distance < AccelerationDistanceThreshold && Last Accel. has passed TimeBetweenAcceleration 
			if (CurrentSpeed > MinSpeed && distance <= AccelerationDistanceThreshold && (gameTime.TotalGameTime - _lastAccelerationTime).TotalMilliseconds > _timeBetweenAcceleration.TotalMilliseconds)
			{
				doAcceleration = true;
				_accelerationFactor = -1;
			}

			if (doAcceleration)
			{
				// Speed = (Speed + Acceleration) with constraints Max(MaxSpeed) and Min(MinSpeed)
				_lastAccelerationTime = gameTime.TotalGameTime;
				CurrentSpeed = MathHelper.Clamp(CurrentSpeed + _accelerationFactor, MinSpeed, MaxSpeed);
				//Console.WriteLine("Speed: " + CurrentSpeed);
			}

			// When it gets really close slow down even more
			if (distance < 60)
			{
				CurrentSpeed = MinSpeed / 2;
				//Console.WriteLine("Speed: " + CurrentSpeed);
			}

			_tempPositionCalc.X += CurrentSpeed * (float)Math.Cos(atan);
			_tempPositionCalc.Y += CurrentSpeed * (float)Math.Sin(atan);
			if (CurrentTile.ContainsPoint(_tempPositionCalc))
			{
				MovePositionToTemp();

			}
			else
			{
				var nextTile = WorldGrid.GridTileAtLocation(_tempPositionCalc);

				if (!nextTile.HasBarricade && !nextTile.HasBoat)
				{
					CurrentTile.OccupiedBoat = null;
					CurrentTile = nextTile;
					CurrentTile.OccupiedBoat = this;
					MovePositionToTemp();
				}
				else
				{
					// TODO should we path find around Object to get to Destination? I think no.
					// for now we will just stop movement, player must navigate boat around barricade or other objects
					IsMoving = false;
					_lastAccelerationTime = TimeSpan.Zero;
					CurrentSpeed = 0;
					_tempPositionCalc.X = _modelPosition.X;
					_tempPositionCalc.Y = _modelPosition.Z;
                    Collision();
				}

			}

		}

        protected virtual void DestinationReached()
        {
            
        }

        protected virtual void CheckOil()
        {

        }

        protected virtual void CleanUpOil()
        {
            if (!Tutorials.TutorialEmptyTankHelped || !Tutorials.TutorialCreateTankerHelped)
            {
                if (this.TankLevel ==  this.Capacity)
                {
                    Tutorials.TutorialEmptyTankHelped = true;
                    Tutorials.TutorialCreateTankerHelped = true;
                    BulletinContainer.CallBulletin("Tuturial.CreateTanker");
                    BulletinContainer.CallBulletin("Tuturial.EmptyTank");
                    
                }
            }
        }

        protected virtual void Collision()
        {
        }

        protected virtual void EmptyTank()
        {
            if (!Tutorials.TutorialUpgradeBoatsHelped)
            {
                Tutorials.TutorialUpgradeBoatsHelped = true;
                BulletinContainer.CallBulletin("Tuturial.UpgradeBoats");
            }
        }

		private void MovePositionToTemp()
		{
			// Move the Model
			_modelPosition.X = _tempPositionCalc.X;
			_modelPosition.Z = _tempPositionCalc.Y;
			// Bounding box is dirty
			IsBoundingBoxUpToDate = false;

			// Force ship into new position and stop movement
			if (Helper.Distance(this.ModelPosition, this.DestinationPosition) < 5)
			{
				IsMoving = false;
				_lastAccelerationTime = TimeSpan.Zero;
				ModelPosition = new Vector3(DestinationPosition.X, ModelPosition.Y, DestinationPosition.Z);
				CurrentSpeed = 0;
                DestinationReached();
			}
		}

		public virtual MenuItem GetLowerMenu()
		{
			return new MenuItem
			{
				Enabled = true,
				Selectable = true,
				Name = "",
				Image = VisualHUD.IconDictionary[@"Remove"],
				InfoTitle = @"Remove " + BoatType,
				InfoSubTitle = "",
				InfoMessage = "Removing a ship will subtract from your max ship cap which will allow you to build more ships. You will not get any money back for ships you remove."
			};
		}
		public virtual MenuItem GetRightMenu()
		{
			return MenuItem.BlankMenu;
		}
		public virtual MenuItem GetUpperMenu()
		{
			return MenuItem.BlankMenu;
		}
		public virtual MenuItem GetLeftMenu()
		{
			return MenuItem.BlankMenu;
		}

		public virtual void HandleSelection(MenuSelection selection, GridTile placementLocation)
		{
			if (selection.Equals(MenuSelection.LowerMenu))
			{
				RemoveBoat();
			}
		}
	}
}
