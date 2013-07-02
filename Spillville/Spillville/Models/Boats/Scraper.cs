using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.MainGame;
using Spillville.MainGame.World;
using Spillville.MainGame.OilSpillContainer;
using Spillville.Utilities;
using System;
using System.Threading;
using Spillville.MainGame.HUD;

namespace Spillville.Models.Boats
{
	class Scraper : Boat
	{
		//private Queue<Vector3> _jobQueue;
		//public short OilTankCurrent;
		//public short OilTankMaxCapacity;

		public const int CAPACITY = 10;
		public const int UPCAPACITY = 15;
		public const float MAXSPEED = 15;
		public const float UPMAXSPEED = 20;

		private OilSpill _selectedSpill;
		private bool CapacityUpgrade;
		private int _hasUpdated;

		public Scraper()
		{
			BoatType = "public static";
			_selectedSpill = null;
		}

		public override void Initialize(Vector2 pos)
		{
			//Icon = "Icons\\scraper";
			ModelObject = ModelFactory.Get(typeof(Scraper).Name);
			ModelScale = 2.1f;

			//ModelMaxSpeed = 80f;
			//ModelSpeed = 0f;
			//_jobQueue = new Queue<Vector3>();
			CapacityUpgrade = false;
			this.MaxSpeed = MAXSPEED;
			this.Capacity = CAPACITY;
			this.TankLevel = 0;
			base.Initialize(pos);

		}

        protected override void Collision()
        {
            foreach (var j in JobQueue)
            {
                j.UnReserved(this);
            }

            JobQueue.Clear();
            base.Collision();
            this.Move(TargetLocation);
        }

        protected override void CleanUpOil()
        {
            var tile = JobQueue.Dequeue();

            if (tile.OccupiedOilSpill.CleanOilAtGridTile(tile) && tile.UnReserved(this))
            {
                TankLevel++;
                this.BlackBox = HealthBar.CreateBox(TankLevel * 5, 10, Color.Black, Color.Red);
            }

            if (JobQueue.Count > 0)
            {
                tile = JobQueue.Peek();
                //temp.X += WorldGrid.VerticeLength / 2;
                //temp.Y += WorldGrid.VerticeLength;
                this.Move(tile);
            }
            else //allocate more
            {

                if (_selectedSpill != null && !_selectedSpill.IsOilSpillCleaned)
                {
                    AllocateOilBlocks(_selectedSpill.GetClosestGridTileInSpill(this.ModelPosition).CenterPoint);
                    if (JobQueue.Count > 0)
                    {
                        this.Move(JobQueue.Peek());                        
                    }
                }

            }

            if (TankLevel == this.Capacity)
            {
                //ToDo:
                //add message to HUD telling player they need to empty tank
            }
            base.CleanUpOil();
        }

        protected override void DestinationReached()
        {
            if (JobQueue.Count > 0 && JobQueue.Peek() == CurrentTile)
            {
                Spillville.ThreadPool.AddTask(CleanUpOil
                            , delegate(Task task, Exception err) { }, null);
            }
            else
                if (JobQueue.Count > 0 && JobQueue.Peek() != CurrentTile)
                {
                    Collision();
                }
                else
                if (TankLevel > 0) //empty tank
                {
                    var ajacentTiles = this.CurrentTile.GetAdjacentTiles();

                    for (int i = 0; i < ajacentTiles.Count; i++)
                    {
                        if (ajacentTiles[i].HasBoat && ajacentTiles[i].OccupiedBoat.GetType() == typeof(Tanker))
                        {
                            Spillville.ThreadPool.AddTask(EmptyTank
                            , delegate(Task task, Exception err) { }, null);
                        }
                    }
                    
                    
                }
                else
                    if (CurrentTile.HasOil && TankLevel<Capacity&& CurrentTile.ReservedBy==null)
                    {
                        AllocateOilBlocks(this.ModelPosition);
                    }            

            base.DestinationReached();
        }

        protected override void EmptyTank()
        {
            //todo: slow down the process, add wait or something                        
                while(!IsMoving && this.TankLevel > 0)
                {
                    Thread.Sleep(200);
                    TankLevel--;
                            if (TankLevel > 0)
                                this.BlackBox = HealthBar.CreateBox(TankLevel * 5, 10, Color.Black, Color.Red);
                            else
                            {
                                BlackBox = HealthBar.CreateBox(1, 10, Color.Black, Color.Red);
                            }
                }

                base.EmptyTank();
            
        }

		public override void Update(GameTime gameTime)
		{
            //if (JobQueue.Count > 0 && !IsMoving && this.ModelPosition.X != this.DestinationPosition.X && this.ModelPosition.Y != this.DestinationPosition.Y)
            //{

            //}
            //if (JobQueue.Count > 0 && !IsMoving)
            //{
				
            //}
            //else
				
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			DrawHealthBar(spriteBatch);
			base.Draw(gameTime, spriteBatch);
		}

		public override void InputHandleActionX(Vector3 target)
		{
            TargetLocation = target;
            if (IsMoving)
            {
                
                Spillville.ThreadPool.AddTask(Collision
                , delegate(Task task, Exception err) { }, null);
            }
            AllocateOilBlocks(target);

            if (JobQueue.Count == 0)
                base.InputHandleActionX(TargetLocation);
            //if (JobQueue.Count > 0)
            //{
            //    this.Move(JobQueue.Peek());
            //}
            //else
                //base.InputHandleActionX(target);
		}

        private void AllocateOilBlocks()
        {            
            lock (this)
            {
                var futureRotation = -(float)System.Math.Atan2((DestinationPosition.Z - ModelPosition.Z), (DestinationPosition.X - ModelPosition.X)) - MathHelper.ToRadians(90.0f);
                bool forward = futureRotation >= 0 && futureRotation < MathHelper.ToRadians(180);

                //think about immediate after rotation
                var currentGrid = WorldGrid.GridTileAtLocation(TargetLocation.X, TargetLocation.Z);
                if (currentGrid.HasOil)
                {
                    currentGrid = currentGrid.OccupiedOilSpill.GetClosestGridTileInSpill(this.ModelPosition);
                    _selectedSpill = currentGrid.OccupiedOilSpill;
                }
                else
                {
                    var ajacents = currentGrid.GetAdjacentTiles();
                    for (int i = 0; i < ajacents.Count; i++)
                    {
                        if (ajacents[i].HasOil && ajacents[i].ReservedBy==null)
                        {
                            currentGrid = ajacents[i];
                            _selectedSpill = currentGrid.OccupiedOilSpill;
                            break;
                        }
                    }
                }

                if (currentGrid.ReservedBy != null)
                {
                    currentGrid = null;
                    if (_selectedSpill != null)
                    {
                        var tiles = _selectedSpill.Tiles;
                        for (int i = 0; i < tiles.Count; i++)
                        {
                            if (tiles[i].HasOil && tiles[i].ReservedBy == null)
                                currentGrid = tiles[i];
                        }
                    }

                        if (currentGrid == null)
                        {
                            base.InputHandleActionX(TargetLocation);
                            return;
                        }
                    
                }

                //lock (_selectedSpill) 
                while ((currentGrid.HasOil && !currentGrid.HasBoat) || currentGrid.HasOil && currentGrid.OccupiedBoat==this )
                {
                    lock(currentGrid)
                    if (JobQueue.Count < Capacity - TankLevel && currentGrid.ReserveTile(this))
                    {
                        JobQueue.Enqueue(currentGrid);
                    }

                    if (forward)
                    {
                        currentGrid = currentGrid.N;
                    }
                    else
                        currentGrid = currentGrid.S;
                }

                if (JobQueue.Count > 0 && CurrentTile != JobQueue.Peek())
                    Move(JobQueue.Peek());
                else
                    if (JobQueue.Count > 0 && CurrentTile == JobQueue.Peek())
                    {
                        Spillville.ThreadPool.AddTask(CleanUpOil
                            , delegate(Task task, Exception err) { }, null);
                    }                  
                    //else
                    //    base.InputHandleActionX(TargetLocation);
                
                
            }
        }

		private void AllocateOilBlocks(Vector2 location)
		{
			AllocateOilBlocks(new Vector3(location.X, this.ModelPosition.Y, location.Y));
		}

		/// <summary>
		/// allocate a stright line of oil spills.
		/// </summary>
		/// <param name="location">The pivot location</param>
		private void AllocateOilBlocks(Vector3 location)
		{
            TargetLocation = location;

            Spillville.ThreadPool.AddTask(AllocateOilBlocks
                , delegate( Task task, Exception err) { }, null);

		}//end method                 

		public void UpgradeSpeed()
		{
			if (_hasUpdated == 0 && GameStatus.CanSpendMoney(7000))
			{
				GameStatus.SpendMoney(7000);
				_hasUpdated++;
				SpeedUpgrade = true;
				MaxSpeed = UPMAXSPEED;
				UpdateModelObject(@"FastScraper");
				VisualHUD.DisplayMessage(@"Upgraded to Fast Scraper");
                BulletinContainer.CallBulletin("Upgrade");
                
			}
			else if (_hasUpdated == 1 && GameStatus.CanSpendMoney(7000))
			{
				GameStatus.SpendMoney(7000);
				_hasUpdated++;
				SpeedUpgrade = true;
				MaxSpeed = UPMAXSPEED + 10;
				UpdateModelObject(@"FastestScraper");
				VisualHUD.DisplayMessage(@"Upgraded to Fastest Scraper");
                BulletinContainer.CallBulletin("Upgrade");
			}
			else
			{
				VisualHUD.DisplayMessage(@"Insufficient funds");
			}


		}

		public void UpgradeCapacity()
		{
			if (GameStatus.CanSpendMoney(7000))
			{
				GameStatus.SpendMoney(7000);
				_hasUpdated += 2;
				Capacity = UPCAPACITY;
				CapacityUpgrade = true;
				UpdateModelObject(@"CapacityScraper");
				VisualHUD.DisplayMessage(@"Upgraded to High Capacity Scraper");
                BulletinContainer.CallBulletin("Upgrade");
                WhiteBox = HealthBar.CreateBox(Capacity * 5, 10, Color.White, Color.Red);
			}
			else
			{
				VisualHUD.DisplayMessage(@"Insufficient funds");
			}
		}

		public override MenuItem GetUpperMenu()
		{
			if (_hasUpdated != 0)
			{
				return MenuItem.BlankMenu;
			}

			return new MenuItem
			{
				Name = "",
				Enabled = true,
				Image = VisualHUD.IconDictionary["ScaperCapacityUpgrade"],
				Selectable = true,
				InfoTitle = "Capacity Upgrade",
				InfoSubTitle = "-$7,000",
				InfoMessage = "Upgrading the Scraper's will increase the ammount of oil it can hold. It will then be able to go for a longer period of time before it has to empty at a tanker."
			};
		}

		public override MenuItem GetRightMenu()
		{
			switch (_hasUpdated)
			{
				case 0:
					return new MenuItem
					{
						Name = "",
						Enabled = true,
						Image = VisualHUD.IconDictionary["TankerSpeedUpgrade"],
						Selectable = true,
						InfoTitle = "Speed Upgrade 1",
						InfoSubTitle = "-$7,000",
						InfoMessage = "Upgrading the Scraper's speed will increase the rate in which the scraper collects oil and travels to new spills."
					};
				case 1:
					return new MenuItem
					{
						Name = "",
						Enabled = true,
						Image = VisualHUD.IconDictionary["TankerSpeedUpgrade2"],
						Selectable = true,
						InfoTitle = "Speed Upgrade 2",
						InfoSubTitle = "-$7,000",
						InfoMessage = "Upgrading the Scraper's speed will increase the rate in which the scraper collects oil and travels to new spills."
					};
					break;
			}

			return MenuItem.BlankMenu;
		}

		public override void HandleSelection(MenuSelection selection, GridTile placementLocation)
		{
			base.HandleSelection(selection, placementLocation);

			switch (selection)
			{
				case MenuSelection.UpperMenu:
					UpgradeCapacity();
					break;
				case MenuSelection.RightMenu:
					UpgradeSpeed();
					break;
			}

		}

	}//end class
}//end namespace
