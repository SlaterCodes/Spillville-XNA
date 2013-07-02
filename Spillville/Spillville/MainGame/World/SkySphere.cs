using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Spillville.MainGame;
using Spillville.Models;

namespace Spillville.MainGame.World
{
    class SkySphere : ModelObject
    {
        public void Initialize(Model model)
        {
            Model = model;
            ModelScale = 20.0f;
            ModelPosition = new Vector3(0.0f, 0.0f, 0.0f);
            ModelRotation = 0f;
            Initialize(false);
        }

        public new void Update(GameTime gameTime)
        {
            ModelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.002f);

            base.Update(gameTime);
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
