using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.MainGame;
using Spillville.MainGame.HUD;
using Spillville.Models.Boats;
using Spillville.Utilities;


namespace Spillville.Models.Animals
{
    public class Bird : Animal
    {
        public Bird()
        {
            this.CleaningTime = TimeSpan.FromSeconds(30);
            ModelScale = 8.0f;
            IsAnimated = false;
        }

        public void Initialize(Vector2 position)
        {
            this.ModelObject = ModelFactory.Get(typeof(Bird).Name);
            SinkConstant = 0;
            this.animalType = "bird";
            base.Initialize(position);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {

                if (health < 25.0f && !healthWarning)
                {
                    healthWarning = true;
                    AudioManager.PlayManaged3DSoundEffect("birdScreams", this.ModelPosition, false);
                    BulletinContainer.CallBulletin("Tutorial.SaveBird");
                }

                if (CurrentTile.HasOil)
                {
                    timeCounter++;
                    if (timeCounter >= 10)
                    {
                        timeCounter = 0;
                        if (health > 1)
                            _GreenBox = HealthBar.CreateBox((int)health, 10, Color.LightGreen, Color.Black);
                        else
                            _GreenBox = HealthBar.CreateBox(1, 10, Color.Red, Color.Black);

                        Hurt(0.1f);
                    }
                    if (health <= 0)
                    {
                        Dead = true;
                    }
                }
            }

            base.Update(gameTime);
        }
    }
}
