#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Spillville.Utilities;
#endregion

//1
namespace Spillville.Models.ParticlesSystem
{
    /// <summary>
    /// Custom particle system for creating a giant plume of chemicals.
    /// </summary>
    public class AnimalsHeartParticles : ParticlesSystem
    {
        public override void LoadTexture(ContentManager content)
        {
            texture = content.Load<Texture2D>(@"Particles\heart");
        }

        public override void InitializeSettings()
        {
            settings.MaxParticles = 300;

            settings.Duration = TimeSpan.FromSeconds(8);

            settings.MinHorizontalVelocity = -20;
            settings.MaxHorizontalVelocity = 20;

            settings.MinVerticalVelocity = -30;
            settings.MaxVerticalVelocity = 30;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(0, -5, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 50;
            settings.MaxStartSize = 100;

            settings.MinEndSize = 50;
            settings.MaxEndSize = 100;

            settings.BlendState = BlendState.AlphaBlend;
        }
       
        /// <summary>
        /// Adds a new particle to the system.
        /// </summary>
        public override void AddParticle(Vector3 position, Vector3 velocity)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = firstFreeParticle + 1;

            if (nextFreeParticle >= settings.MaxParticles)
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == firstRetiredParticle)
                return;

            // Adjust the input velocity based on how much
            // this particle system wants to be affected by it.
            velocity *= settings.EmitterVelocitySensitivity;

            // Add in some random amount of horizontal velocity.
            float horizontalVelocity = MathHelper.Lerp(settings.MinHorizontalVelocity,
                                                       settings.MaxHorizontalVelocity,
                                                       (float)random.NextDouble());

            double horizontalAngle = random.NextDouble() * MathHelper.TwoPi;

            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

            // Add in some random amount of vertical velocity.
            velocity.Y += MathHelper.Lerp(settings.MinVerticalVelocity,
                                          settings.MaxVerticalVelocity,
                                          (float)random.NextDouble());

            // Choose four random control values. These will be used by the vertex
            // shader to give each particle a different size, rotation, and color.
            randomValues = new Color((byte)random.Next(50),
                                           (byte)random.Next(50),
                                           (byte)random.Next(50),
                                           (byte)random.Next(50));

            // Fill in the particle vertex structure.
            for (int i = 0; i < 4; i++)
            {
                particles[firstFreeParticle * 4 + i].Position = position;
                particles[firstFreeParticle * 4 + i].Velocity = velocity;
                particles[firstFreeParticle * 4 + i].Random = randomValues;
                particles[firstFreeParticle * 4 + i].Time = currentTime;
            }

            firstFreeParticle = nextFreeParticle;
        }

    }
}
