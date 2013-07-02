using Microsoft.Xna.Framework.Graphics;

namespace Spillville
{
	interface IObjectWithMenu
	{
		MenuItem GetTopMenu();
		MenuItem GetRightMenu();
		MenuItem GetBottomMenu();
		MenuItem GetLeftMenu();
		void HandleSelection(int selection);
	}


	public class MenuItem
	{
		public string Name;
		public float Percent;
		public bool Enabled;
		public bool Selectable;
		public Texture2D Image;
	}
}
