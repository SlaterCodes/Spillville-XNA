using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Spillville.Models.Boats
{
    class Tanker : Boat
    {
        public const float MAXSPEED = 15;
        public const float UPMAXSPEED = 25;

        public Tanker()
        {
            this.BoatType = "Tanker";
        }

        public new void Initialize(Vector2 pos)
        {
            this.ModelObject = ModelFactory.Get(typeof(Tanker).Name);
            //Icon = "Icons\\Tanker";
            ModelScale = 14f;

            MaxSpeed = MAXSPEED;

            base.Initialize( pos);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime,spriteBatch);
        }

        public void UpgradeSpeed()
        {
            SpeedUpgrade = true;
            MaxSpeed = UPMAXSPEED;
        }
    }
}
