using Microsoft.Xna.Framework;
using Spillville.MainGame;
using Spillville.MainGame.World;
using Spillville.Utilities;
using System.Collections.Generic;

namespace Spillville.Models.Boats
{
	public class CommandShip : Boat
	{
		public CommandShip()
		{
			BoatType = "Command Ship";
		}

		public override void Initialize(Vector2 pos)
		{
            ModelObject = ModelFactory.Get(typeof(CommandShip).Name);

			ModelScale = 29.0f;

			base.Initialize(pos);
		}

		#region Implementation of IObjectWithMenu

		public override MenuItem GetUpperMenu()
		{
			return new MenuItem
				    {
				       	Name = "",
				       	Enabled = true,
				       	Image = VisualHUD.IconDictionary["Dinghy"],
                        Selectable = true,
						InfoTitle = "Dinghy",
						InfoSubTitle = "-$15,000",
						InfoMessage = "The dinghy is outfitted to deploy barriers around oil spills to keep it from spreading. You can also upgrade the dinghy to save animals covered in oil."
				    };
		}

		public override MenuItem GetRightMenu()
		{

			return new MenuItem
				    {
				       	Name = "",
				       	Enabled = true,
				       	Image = VisualHUD.IconDictionary["Scraper"],
				       	Selectable = true,
                        InfoTitle = "Scraper",
						InfoSubTitle = "-$45,000",
						InfoMessage = "The Scraper skims the surface by using powerful vacuum hoses that suck the oil into storage tanks. The tanks are then emptied at a Tanker, and in turn will sell the collected crude oil for money."
				    };
		}

		public override MenuItem GetLowerMenu()
		{

            return new MenuItem
				    {
				       	Name = "",
				       	Enabled = true,
				       	Image = VisualHUD.IconDictionary["Tanker"],
                        Selectable = true,
                        InfoTitle = "Tanker",
                        InfoSubTitle = "-$55,000",
                        InfoMessage = "The Tanker is a large storage boat. Scrapers will unload collected oil in the tanker when they get full. The Disperser will refill chemicals from the Tanker when they run out."

				    };
		}

		public override MenuItem GetLeftMenu()
		{
                return new MenuItem
                {
                    Name = "",
                    Enabled = true,
                    Image = VisualHUD.IconDictionary["Disperser"],
                    Selectable = true,
                    InfoTitle = "Disperser",
                    InfoSubTitle = "-$30,000",
                    InfoMessage = "The Disperser uses chemicals to sink the oil into the ocean, this is toxic but effective. You can upgrade to biological agents which are much safer for the environment."
                };
		}

		public override void HandleSelection(MenuSelection selection, GridTile placementLocation)
		{
			if (!GameStatus.CanBuildUnits(1))
			{
				VisualHUD.DisplayMessage("Ship Limit Reached");
				return;
			}

			switch (selection)
			{
				case MenuSelection.UpperMenu:
					if (GameStatus.CanSpendMoney(15000))
					{
						var temp = new Dinghy();
                        var gridTile = GetFreeSpawnTile();
                        if(gridTile!=null)
                        {
							GameStatus.SpendMoney(15000);
                        	GameStatus.BuildUnits(1);
                            temp.Initialize(new Vector2(gridTile.CenterPoint.X, gridTile.CenterPoint.Y));
                            GameStatus.RegisterDrawableUnit(temp);
						    VisualHUD.DisplayMessage("New Dinghy Created.");
                            AudioManager.PlayManaged3DSoundEffect("DinghySound", ModelPosition, false);
                        }
					}
					else
					{
						VisualHUD.DisplayMessage("Insufficient funds");
					}
					break;
				case MenuSelection.RightMenu:
					if (GameStatus.CanSpendMoney(45000))
					{
						var temp = new Scraper();
                        var gridTile = GetFreeSpawnTile();
                        if (gridTile != null)
                        {
							GameStatus.SpendMoney(45000);
							GameStatus.BuildUnits(1);
                            temp.Initialize(new Vector2(gridTile.CenterPoint.X, gridTile.CenterPoint.Y));
                            GameStatus.RegisterDrawableUnit(temp);
                            VisualHUD.DisplayMessage("New Scraper Created.");
                            AudioManager.PlayManaged3DSoundEffect("DinghySound", ModelPosition, false);
                        }
					}
					else
					{
						VisualHUD.DisplayMessage("Insufficient funds");
					}
                    break;
                case MenuSelection.LowerMenu:
					if (GameStatus.CanSpendMoney(55000))
                    {
                        var temp = new Tanker();
                        var gridTile = GetFreeSpawnTile();
                        if (gridTile != null)
                        {
							GameStatus.SpendMoney(55000);
							GameStatus.BuildUnits(1);
                            temp.Initialize(new Vector2(gridTile.CenterPoint.X, gridTile.CenterPoint.Y));
                            GameStatus.RegisterDrawableUnit(temp);
                            VisualHUD.DisplayMessage("New Tanker Created.");
                            AudioManager.PlayManaged3DSoundEffect("DinghySound", ModelPosition, false);
                        }
                    }
                    else
                    {
                        VisualHUD.DisplayMessage("Insufficient funds");
                    }
                    break;
                case MenuSelection.LeftMenu:
					if (GameStatus.CanSpendMoney(30000))
                    {
                        var temp = new Disperser();
                        var gridTile = GetFreeSpawnTile();
                        if (gridTile != null)
                        {
							GameStatus.SpendMoney(15000);
							GameStatus.BuildUnits(1);
                            temp.Initialize(new Vector2(gridTile.CenterPoint.X, gridTile.CenterPoint.Y));
                            GameStatus.RegisterDrawableUnit(temp);
                            VisualHUD.DisplayMessage("New Disperser Created.");
                            AudioManager.PlayManaged3DSoundEffect("DinghySound", ModelPosition, false);
                        }
                    }
                    else
                    {
                        VisualHUD.DisplayMessage("Insufficient funds");
                    }
					break;
			}
		}

		private GridTile GetFreeSpawnTile()
        {
            List<GridTile> g = WorldGrid.GetAdjacentGridTiles(this.CurrentTile);
            for (var i = 0; i < g.Count; i++)
            {
                if (!g[i].HasBoat && !g[i].HasBarricade && !g[i].HasOil && !g[i].HasAnimal)
                    return g[i];
            }
            return null;
        }

		#endregion
	}
}