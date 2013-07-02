using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.MainGame;
using Spillville.MainGame.OilSpillContainer;
using Spillville.MainGame.World;
using Spillville.Models.ParticlesSystem;
using Spillville.Utilities;
using System.Threading;
using System;
using Spillville.MainGame.HUD;

namespace Spillville.Models.Boats
{
    class Disperser : Boat
    {
        private Queue<job> _jobQueue;
        private OilSpill _selectedSpill;
        //private ChemicalsParticlesSystem _particles;
        //private ParticleEmitter _particlesEmitter;
        private Vector3 particleVelocity;
        private Vector3 particlePosition;
        private Vector3 particlePosition2;

    	private bool _hasUpdated;

        public const float MAXSPEED = 25;
        public const float UPMAXSPEED = 35;

        public const int UPCAPACITY = 15;
        public const int CAPACITY = 10;

        private bool CapacityUpgrade;
        private bool BioremediationUpgrade;
        private bool _displayChemical;
        private bool _displayBioAgent;

        struct job
        {
            public GridTile centerTile;
            public GridTile leftTile;
            public GridTile rightTile;

            public job(GridTile center, GridTile left, GridTile right)
            {
                centerTile = center;
                leftTile = left;
                rightTile = right;
                
            }

            public bool HasOil()
            {
                return centerTile.HasOil || leftTile.HasOil || rightTile.HasOil;
            }
        }
        
        public Disperser()
        {
            this.BoatType = "Disperser";
            _selectedSpill = null;
            particleVelocity = new Vector3(0, -1, 0);
            particlePosition = Vector3.Zero;
            particlePosition2 = Vector3.Zero;
            _displayBioAgent = false;
            _displayChemical = false;
        }

        public new void Initialize(Vector2 pos)
        {
            this.ModelObject = ModelFactory.Get(typeof(Disperser).Name);
            //Icon = "Icons\\Disperser";
            ModelScale = 30f;
            this.TankLevel = 0;
            this.Capacity = CAPACITY;
            this.CapacityUpgrade = false;

            MaxSpeed = MAXSPEED;
           

            //_particles = new ChemicalsParticlesSystem(base.
            //ModelMaxSpeed = 80f;
            //ModelSpeed = 0f;

            //_particlesEmitter = new ParticleEmitter((float)30, new Vector3(this.ModelPosition.X, this.ModelPosition.Y+50, this.ModelPosition.Z));
            //_particlesEmitter = new ParticleEmitter((float)50, new Vector3(this.ModelPosition.X - 20, this.ModelPosition.Y + 50, this.ModelPosition.Z));


            _jobQueue = new Queue<job>();

            base.Initialize(pos);
        }

        protected override void Collision()
        {
            foreach (var j in _jobQueue)
            {
                j.centerTile.UnReserved(this);
                j.leftTile.UnReserved(this);
                j.rightTile.UnReserved(this);
            }

            _jobQueue.Clear();
            base.Collision();
            this.Move(TargetLocation);
        }

        public override void Update(GameTime gameTime)
        {
            if (popularityTicker > 0)
                popularityTicker++;

            if (CurrentTile.HasOil&&TankLevel<Capacity)
            {
                
               // _particlesEmitter.Update(gameTime, new Vector3(this.ModelPosition.X, this.ModelPosition.Y + 50, this.ModelPosition.Z));
                particlePosition = this.ModelPosition;
                particlePosition.Y += 40;
                particlePosition.X += 40;
                particlePosition2 = this.ModelPosition;
                particlePosition2.Y += 40;
                particlePosition2.X -= 40;
                ParticlesSystemsCollection.ParticlesSystems[0].AddParticle(particlePosition, particleVelocity);
                ParticlesSystemsCollection.ParticlesSystems[0].AddParticle(particlePosition2, particleVelocity);
            }
                        

            base.Update(gameTime);
        }



        protected override void CleanUpOil()
        {
            var currentJob = _jobQueue.Dequeue();

            if (currentJob.HasOil())
            {
                if (currentJob.centerTile.UnReserved(this))
                {
                    currentJob.centerTile.OccupiedOilSpill.CleanOilAtGridTile(currentJob.centerTile);
                    //TankLevel++;
                }
                if (currentJob.leftTile.UnReserved(this))
                {
                    currentJob.leftTile.OccupiedOilSpill.CleanOilAtGridTile(currentJob.leftTile);
                    //TankLevel++;
                }
                if (currentJob.rightTile.UnReserved(this))
                {
                    currentJob.rightTile.OccupiedOilSpill.CleanOilAtGridTile(currentJob.rightTile);
                    //TankLevel++;
                }
                TankLevel++;
                this.BlackBox = HealthBar.CreateBox(TankLevel * 5, 10, Color.Black, Color.Red);

                //send out messages
                if (!BioremediationUpgrade)
                {
                    GameStatus.DecreasePopulatity(1);
                    popularityTicker=1;
                    currentMessage = "A -1";
                    currentMessageColor = Color.Red;
                    VisualHUD.DisplayMessage("Popularity decreased by 1");
                }
                else
                {
                    GameStatus.IncreasePopularity(1);
                    popularityTicker = 1;
                    currentMessage = "A +1";
                    currentMessageColor = Color.Green;
                    VisualHUD.DisplayMessage("Popularity increased by 1");
                }
            }

            if (_jobQueue.Count > 0)
            {
                this.Move(_jobQueue.Peek().centerTile);
            }
            else //allocate more
                if (OilSpillManager.OilSpills().Count > 0)
                {
                    if (!BioremediationUpgrade && !_displayChemical)
                    {
                        BulletinContainer.CallBulletin("Chemical");
                        _displayChemical = true;
                    }
                    else
                    {
                        if(!_displayBioAgent)
                            BulletinContainer.CallBulletin("ProperClean");
                        _displayBioAgent = true;
                    }
                    var oilSpills = OilSpillManager.OilSpills();
                    GridTile closest = null; // = oilSpills[0].GetClosestGridTileInSpill(this.ModelPosition); ;
                    //int dist = WorldGrid.GetLogicalTileDistance(closest, CurrentTile);

                    if (_selectedSpill == null || _selectedSpill.IsOilSpillCleaned)
                    {
                        closest = null;
                    }
                    else
                        closest = _selectedSpill.GetClosestGridTileInSpill(this.ModelPosition);

                    if (closest != null)
                        AllocateOilBlocks(closest.CenterPoint);


                    if (_jobQueue.Count > 0)
                    {
                        this.Move(_jobQueue.Peek().centerTile);
                    }

                }
            base.CleanUpOil();
        }

        protected override void EmptyTank()
        {
            while (!IsMoving && TankLevel > 0)
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

        protected override void DestinationReached()
        {
            //to clean up oil
            if (_jobQueue.Count > 0 && _jobQueue.Peek().centerTile == CurrentTile)
            {
                Spillville.ThreadPool.AddTask(CleanUpOil
                            , delegate(Task task, Exception err) { }, null);
            }
            else
                if (_jobQueue.Count > 0 && _jobQueue.Peek().centerTile != CurrentTile)
                {
                    Collision();
                }
                else //to empty the tank
                    if (!IsMoving && TankLevel > 0) //empty tank
                    {
                        //todo: slow down the process, add wait or something
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
                        if (CurrentTile.HasOil && TankLevel < Capacity && CurrentTile.ReservedBy == null)
                        {
                            AllocateOilBlocks(this.ModelPosition);
                        }     
            base.DestinationReached();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawHealthBar(spriteBatch);
            if (popularityTicker >1)
            {
                DrawPopularityEffect(spriteBatch, currentMessage, currentMessageColor);
                if (popularityTicker > 120)
                    popularityTicker = 0;
            }
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
            //if (_jobQueue.Count > 0)
            //{
            //    this.Move(_jobQueue.Peek().centerTile);
            //}
            //else
            //    base.InputHandleActionX(target);
        }

        private void AllocateOilBlocks(Vector3 location)
        {
            TargetLocation = location;

            Spillville.ThreadPool.AddTask(AllocateOilBlocks
                , delegate(Task task, Exception err) { }, null);
        }
        
        private void AllocateOilBlocks(Vector2 location)
        {
            AllocateOilBlocks(new Vector3(location.X, this.ModelPosition.Y, location.Y));
        }

        private void AllocateOilBlocks()
        {
            lock (this)
            {
                var futureRotation = -(float)System.Math.Atan2((DestinationPosition.Z - ModelPosition.Z), (DestinationPosition.X - ModelPosition.X)) - MathHelper.ToRadians(90.0f);
                bool forward = futureRotation >= 0 && futureRotation < MathHelper.ToRadians(180);

                var currentTile = WorldGrid.GetTileAtLocationByMath(TargetLocation.X, TargetLocation.Z);

                if (currentTile.HasOil)
                {
                    currentTile = currentTile.OccupiedOilSpill.GetClosestGridTileInSpill(this.ModelPosition);
                    _selectedSpill = currentTile.OccupiedOilSpill;
                }
                else
                {
                    var ajacents = currentTile.GetAdjacentTiles();
                    for (int i = 0; i < ajacents.Count; i++)
                    {
                        if (ajacents[i].HasOil && ajacents[i].ReservedBy == null)
                        {
                            currentTile = ajacents[i];
                            _selectedSpill = currentTile.OccupiedOilSpill;
                            break;
                        }
                    }
                }

                var leftTile = currentTile.W;
                var rightTile = currentTile.E;

                var reserved = false;

                //if all surrounding tiles are reserved, go clean somewhere else
                if (leftTile.ReservedBy != null || rightTile.ReservedBy != null || currentTile.ReservedBy != null)
                {
                    currentTile = null;
                    if(_selectedSpill!=null)
                    {
                        var tiles = _selectedSpill.Tiles;
                        for (int i = 0; i < tiles.Count; i++)
                        {
                            if (tiles[i].HasOil && tiles[i].ReservedBy == null)
                                currentTile = tiles[i];
                        }
                    }

                    if (currentTile == null)
                    {
                        base.InputHandleActionX(TargetLocation);
                        return;
                    }
                }

                //lock (_selectedSpill) 
                while (_jobQueue.Count < Capacity - TankLevel && (currentTile.HasOil || leftTile.HasOil || rightTile.HasOil))
                {
                    if (currentTile.HasOil)
                        reserved = currentTile.ReserveTile(this);
                    if (leftTile.HasOil)
                        reserved = leftTile.ReserveTile(this) || reserved;
                    if (rightTile.HasOil)
                        reserved = rightTile.ReserveTile(this) || reserved;

                    lock(currentTile)
                    if (reserved)
                        _jobQueue.Enqueue(new job(currentTile, leftTile, rightTile));

                    if (forward)
                    {
                        currentTile = currentTile.N;
                    }
                    else
                        currentTile = currentTile.S;

                    leftTile = currentTile.W;
                    rightTile = currentTile.E;

                }

                if (this._jobQueue.Count > 0 && CurrentTile != _jobQueue.Peek().centerTile)
                    this.Move(this._jobQueue.Peek().centerTile);
                else
                    if (_jobQueue.Count > 0 && CurrentTile == _jobQueue.Peek().centerTile)
                    {
                        Spillville.ThreadPool.AddTask(CleanUpOil
                              , delegate(Task task, Exception err) { }, null);
                    }
                    //else
                    //    base.InputHandleActionX(TargetLocation);

            }// end lock
            
            
        } 


		public override MenuItem GetUpperMenu()
		{
			if (_hasUpdated)
			{
				return MenuItem.BlankMenu;
			}

			return new MenuItem
			{
				Name = "",
				Enabled = true,
				Image = VisualHUD.IconDictionary["CapacityDisperserUpgrade"],
				Selectable = true,
				InfoTitle = "Capacity Upgrade",
				InfoSubTitle = "-$7,000",
				InfoMessage = "Upgrading the capacity of the disperser will allow it to clean oil for a longer time before having to refill its tank."
			};
		}

		public override MenuItem GetRightMenu()
		{
			if (_hasUpdated)
			{
				return MenuItem.BlankMenu;
			}

			return new MenuItem
			{
				Name = "",
				Enabled = true,
				Image = VisualHUD.IconDictionary["EnviromentalDisperserUpgrade"],
				Selectable = true,
				InfoTitle = "Biological Agents Upgrade",
				InfoSubTitle = "-$7,000",
				InfoMessage = "Biological agents promote the growth of micro-organisms to safely break down oil. This is safe for the environment. Ships using biological agents no longer hurt your popularity."
			};
		}


		public override void HandleSelection(MenuSelection selection, GridTile placementLocation)
		{
			base.HandleSelection(selection, placementLocation);

			switch (selection)
			{

				case MenuSelection.RightMenu:
					UpgradeToEnvironmentalFriendlyChemicals();
					break;
				case MenuSelection.UpperMenu:
					UpgradeCapacity();
					break;
			}

		}

    	private void UpgradeToEnvironmentalFriendlyChemicals()
		{
			if (GameStatus.CanSpendMoney(7000))
			{
				GameStatus.SpendMoney(7000);
				_hasUpdated = true;
                BioremediationUpgrade = true;
                BulletinContainer.CallBulletin("Upgrade");
				UpdateModelObject(@"EnviromentDisperserShip");
				VisualHUD.DisplayMessage(@"Upgraded to higher environmentally friendly chemicals");
			}
			else
			{
				VisualHUD.DisplayMessage(@"Insufficient funds");
			}
		}

    	private void UpgradeCapacity()
		{
			if (GameStatus.CanSpendMoney(7000))
			{
				GameStatus.SpendMoney(7000);
				_hasUpdated = true;
                Capacity = UPCAPACITY;
                CapacityUpgrade = true;
				UpdateModelObject(@"CapacityDisperserShip");
				VisualHUD.DisplayMessage(@"Upgraded to higher capacity disperser ship");
                WhiteBox = HealthBar.CreateBox(Capacity * 5, 10, Color.White, Color.Red);
			}
			else
			{
				VisualHUD.DisplayMessage(@"Insufficient funds");
			}
		}


    }
}
