using Microsoft.Xna.Framework;
using Spillville.Models.Objects;
using Spillville.Utilities;
using Spillville.MainGame;
using Spillville.MainGame.World;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Spillville.MainGame.HUD;

namespace Spillville.Models.Boats
{
	class Dinghy : Boat
	{

		//private Queue<Vector3> _jobQueue;

        public const float MAXSPEED = 25;
        public const float UPMAXSPEED = 35;

		protected bool RescueUpgrade;
		private bool _hasUpdated;

		public Dinghy()
		{
			BoatType = "Dinghy";
		}

		public new void Initialize(Vector2 pos)
		{
			ModelObject = ModelFactory.Get(typeof(Dinghy).Name);
			//Icon = "Icons\\dingy";
			ModelScale = 12f;

			MaxSpeed = MAXSPEED;

			this.Capacity = 0;
			this.TankLevel = 0;

			//this.status = Status.Idle;
			base.Initialize(pos);
            this._modelPosition.Y += 5;
		}

		public override void Update(GameTime gameTime)
		{
            if (!IsMoving && RescueUpgrade)
            {
                var ajacentTiles = CurrentTile.GetAdjacentTiles();
                if (this.CurrentTile.HasAnimal)
                {
                    CurrentTile.OccupiedAnimal.Heal(0.05f);
                }
                else
                {
                    for (int i = 0; i < ajacentTiles.Count; i++)
                    {
                        if (ajacentTiles[i].HasAnimal)
                        {
                            ajacentTiles[i].OccupiedAnimal.Heal(0.02f);
                        }
                    }
                }
            }
			base.Update(gameTime);
		}


		public override MenuItem GetRightMenu()
		{
			return new MenuItem
			{
				Name = "",
				Enabled = true,
				Image = VisualHUD.IconDictionary["Barricade"],
				Selectable = true,//GameStatus.MaxUnits - GameStatus.TotalUnits > 0 && GameStatus.AvailableFunds - 100 >= 0
				PlacementModel = ModelFactory.Get(typeof(Barricade).Name),
				PlacementModelScale = 10.0f,
                BuildPoint = CurrentTile,
                InfoTitle = "Barricade",
                InfoSubTitle = "-$1,000",
			    InfoMessage = "Barricade's prevent oil from spreading further."
			};
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
				Image = VisualHUD.IconDictionary["FastDinghyUpgrade"],
				Selectable = true,//GameStatus.MaxUnits - GameStatus.TotalUnits > 0 && GameStatus.AvailableFunds - 100 >= 0
                InfoTitle = "Dinghy",
                InfoSubTitle = "-$7,000",
                InfoMessage = "Increases the speed of the Dinghy."
			};
		}

		public override MenuItem GetLeftMenu()
		{
			if (_hasUpdated)
			{
				return MenuItem.BlankMenu;
			}

			return new MenuItem
			{
				Name = "",
				Enabled = true,
				Image = VisualHUD.IconDictionary["RescueDinghyUpgrade"],
				Selectable = true,//GameStatus.MaxUnits - GameStatus.TotalUnits > 0 && GameStatus.AvailableFunds - 100 >= 0
                InfoTitle = "Rescue Dinghy Upgrade",
                InfoSubTitle = "-$7,000",
                InfoMessage = "Allows the Dinghy to save animals covered in oil which greatly helps your popularity rating."
			};
		}


		public override void HandleSelection(MenuSelection selection, GridTile placementLocation)
		{
			base.HandleSelection(selection, placementLocation);

			switch (selection)
			{
				case MenuSelection.RightMenu:
					if (placementLocation != null)
					{
						if (GameStatus.CanSpendMoney(1000))
						{
							GameStatus.SpendMoney(1000);
							placementLocation.CreateBarricade();
						}
						else
						{
							VisualHUD.DisplayMessage(@"Insufficient funds");
						}
					}
					break;
				case MenuSelection.LeftMenu:
					UpgradeRescue();
					break;
				case MenuSelection.UpperMenu:
					UpgradeSpeed();
					break;
			}
		
		}

        public void UpgradeSpeed()
        {
			if (GameStatus.CanSpendMoney(7000))
			{
				GameStatus.SpendMoney(7000);
				_hasUpdated = true;
				SpeedUpgrade = true;
				MaxSpeed = UPMAXSPEED;
				UpdateModelObject(@"FastDinghy");
                BulletinContainer.CallBulletin("Upgrade");
				VisualHUD.DisplayMessage(@"Upgraded to Fast Dinghy");
			}
			else
			{
				VisualHUD.DisplayMessage(@"Insufficient funds");
			}
        }

        public void UpgradeRescue()
        {
			
			if (GameStatus.CanSpendMoney(7000))
			{
				GameStatus.SpendMoney(7000);
				_hasUpdated = true;
				RescueUpgrade = true;
				UpdateModelObject(@"RescueDinghy");
                BulletinContainer.CallBulletin("Upgrade");
				VisualHUD.DisplayMessage(@"Upgraded to Rescue Dinghy");
			}
			else
			{
				VisualHUD.DisplayMessage(@"Insufficient funds");
			}

        }


	}
}
