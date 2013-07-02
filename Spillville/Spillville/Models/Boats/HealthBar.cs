using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Spillville.Models.Boats
{
    public static class HealthBar
    {
        public static SpriteBatch _spriteBatch { private set;  get; }
        public static SpriteFont _font { private set;  get; }

        public static void Initialize(SpriteBatch sb, SpriteFont ft)
        {
            _spriteBatch = sb;
            _font = ft;
        }

        public static Texture2D CreateBox(int width, int height, Color color, Color BorderColor)
        {                       
            var box = new Texture2D(
                _spriteBatch.GraphicsDevice,
                width, height,
                false,
                SurfaceFormat.Color);

            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++)
            {
                if ((i % width) < 2 || (i % width) >= width - 2 || i < (width * 2) ||
                i >= (width * height) - (width * 2))
                {
                    colors[i] = BorderColor;
                }
                else
                {
                    colors[i] = color;
                }                
            }

            box.SetData(colors);
            //box.SetData(new Color[width*height] { color });

            return box;
        }        
    }
}
