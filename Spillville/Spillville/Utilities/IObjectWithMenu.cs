using Microsoft.Xna.Framework.Graphics;
using Spillville.MainGame.World;

namespace Spillville.Utilities
{
	interface IObjectWithMenu
	{
		MenuItem GetUpperMenu();
		MenuItem GetRightMenu();
		MenuItem GetLowerMenu();
		MenuItem GetLeftMenu();
		void HandleSelection(MenuSelection selection, GridTile placementLocation);
	}


	public class MenuItem
	{
		public static readonly MenuItem BlankMenu = new MenuItem();
		public string Name;
		public bool Enabled;
		public bool Selectable;
		public Texture2D Image;
		public Model PlacementModel;
		public float PlacementModelScale;
		public GridTile BuildPoint;
		public string InfoTitle;
		public string InfoMessage;
		public string InfoSubTitle;
	}


	public enum MenuSelection
	{
		UpperMenu,
		LowerMenu,
		LeftMenu,
		RightMenu,
		None
	}
}
