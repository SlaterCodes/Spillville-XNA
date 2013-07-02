using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville.MainGame.HUD
{
	static class HUDComponents
	{

		public static Texture2D CreateRectangle(GraphicsDevice graphicsDevice, int width, int height, int border, int green)
		{
			var rect = new Texture2D(graphicsDevice, width, height);

			var data = new Color[width * height];


			for (var i = 0; i < data.Length; ++i)
			{
				//draw border
				if ((i % width) < border || (i % width) >= width - border || i < (width * border) ||
					i >= (width * height) - (width * border))
				{
					data[i] = Color.Black;
				}
				else
				{
					data[i] = Color.Green;
					if (green >= 0)
					{
						data[i].G = (byte)green;
					}
				}
				data[i].A = 175;
			}
			rect.SetData(data);

			return rect;
		}

		public static Texture2D CreateRectangle(GraphicsDevice graphicsDevice, int width, int height, int borderSize, Color rectangleColor, Color borderColor)
		{
			var rect = new Texture2D(graphicsDevice, width, height);

			var data = new Color[width * height];


			for (var i = 0; i < data.Length; ++i)
			{
				//draw border
				if ((i % width) < borderSize || (i % width) >= width - borderSize || i < (width * borderSize) ||
					i >= (width * height) - (width * borderSize))
				{
					data[i] = borderColor;
				}
				else
				{
					data[i] = rectangleColor;
				}
			}
			rect.SetData(data);

			return rect;
		}

	}
}
