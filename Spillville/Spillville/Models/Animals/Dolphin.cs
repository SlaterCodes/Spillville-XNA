using System;   
using Microsoft.Xna.Framework;
using SkinnedModel;
using Spillville.MainGame.HUD;
using Spillville.Models.Animation;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Models.Boats;
using Spillville.MainGame;
using Spillville.MainGame.HUD;
using Spillville.Utilities;

namespace Spillville.Models.Animals
{
    class Dolphin : Animal
    {
        public SkinningData skinningData;
        public AnimatedDolphin animatedDolphin;

        public Dolphin()
        {
            this.CleaningTime = TimeSpan.FromSeconds(30);
            ModelScale = 1.0f;
            IsAnimated = true;
        }

        public new void Initialize(Vector2 pos)
        {
            ModelPosition = new Vector3(pos.X, -25, pos.Y);
            this.ModelObject = ModelFactory.Get(typeof(Dolphin).Name);            
            skinningData = ModelObject.Tag as SkinningData;

            animatedDolphin = new AnimatedDolphin(skinningData);

            AnimationClip clip = skinningData.AnimationClips["Take 001"];
            animatedDolphin.StartClip(clip);
            
            SinkConstant = 0;

            this.animalType = "dolphin";

            base.Initialize(pos);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                // Tell the animation player to compute the latest bone transform matrices.
                animatedDolphin.UpdateBoneTransforms(gameTime.ElapsedGameTime, true);

                // Copy the transforms into our own array, so we can safely modify the values.
                animatedDolphin.GetBoneTransforms().CopyTo(boneTransforms, 0);

                // Tell the animation player to recompute the world and skin matrices.
                //Matrix effectWorldMatrix = Matrix.CreateScale(ModelScale) * Matrix.CreateRotationY(ModelRotation) *
                //               Matrix.CreateTranslation(ModelPosition);

                if (health < 25.0f && !healthWarning)
                {
                    healthWarning = true;
                    AudioManager.PlayManaged3DSoundEffect("Dolphin", this.ModelPosition, false);
                    BulletinContainer.CallBulletin("Tutorial.SaveDolphin");
                }

                animatedDolphin.UpdateWorldTransforms(Matrix.Identity);
                animatedDolphin.UpdateSkinTransforms();

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

            //if (Dead)
            //{
                
                
            //}
            

            //if (Dead && SinkConstant >= 50)
            //{
            //    SendMeAway();
            //}
            base.Update(gameTime);
            
        }
    }
}
