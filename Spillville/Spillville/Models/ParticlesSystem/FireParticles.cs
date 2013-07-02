using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

//4
namespace Spillville.Models.ParticlesSystem
{
    public class FireParticles : ParticlesSystem
    {
        public override void LoadTexture(ContentManager content)
        {
            texture = content.Load<Texture2D>(@"Particles\fire");
        }

        public override void InitializeSettings()
        {
            settings.MaxParticles = 600;

            settings.Duration = TimeSpan.FromSeconds(3);

            settings.MinHorizontalVelocity = -20;
            settings.MaxHorizontalVelocity = 20;

            settings.MinVerticalVelocity = 10;
            settings.MaxVerticalVelocity = 50;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(-5, 20, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 80;
            settings.MaxStartSize = 100;

            settings.MinEndSize = 80;
            settings.MaxEndSize = 100;

            settings.BlendState = BlendState.AlphaBlend;
        }
    }
}
