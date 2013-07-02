using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spillville.Utilities;
using Spillville.Models;
using Spillville.MainGame.Levels;

namespace Spillville.MainGame.LevelSelect
{
    class LevelIndicator : IDrawableModel
    {
        public Model ModelObject { get; private set; }
        public Vector3 ModelRotation { get; set; }
        public Vector3 ModelPosition { get; set; }
        public float ModelScale { get; private set; }
        public Vector3 EmissiveColor { get; set; }
        public Matrix[] boneTransforms { get; private set; }
        public BoundingBox boundingBox { get; private set; }
        public bool Visible;

    	public const float EarthSurfaceDistance = -2f;
        public Level LevelInfo { get; protected set; }

        public Rectangle boundaries { get; private set; }
        private Vector3 projectionCoords { get; set; }

        public LevelIndicator(Level level)
        {
            LevelInfo = level;
            ModelScale = .8f;
            Visible = false;
            projectionCoords = Camera.Viewport.Project(this.ModelPosition, Camera.Projection, Camera.View, Matrix.Identity);
            boundaries = new Rectangle((int)projectionCoords.X - 6, (int)projectionCoords.Y + 6, 12, 12);
            
        }

        public void Initialize(Vector3 pos, float rotation)
        {
            ModelObject = ModelFactory.Get(typeof(LevelIndicator).Name); ;
            boneTransforms = ModelDrawer.GetBoneTransforms(ModelObject);
            ModelPosition = pos;
        	var rotationMatrix = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));
        	ModelRotation = Vector3.Transform(Vector3.One, rotationMatrix);
            UpdateBoundingBox();
        }

        public void Update(GameTime gameTime)
        {
            if (!IsBoundingBoxUpToDate)
            {
                projectionCoords = Camera.Viewport.Project(this.ModelPosition, Camera.Projection, Camera.View, Matrix.Identity);
                boundaries = new Rectangle((int)projectionCoords.X - 6, (int)projectionCoords.Y + 6, 12, 12);
                IsBoundingBoxUpToDate = true;
            }                       
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
			for (int i = 0; i < ModelObject.Meshes.Count; i++)
			{
				ModelMesh mesh = ModelObject.Meshes[i];

				Matrix effectWorldMatrix = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(ModelScale) *
									  Matrix.CreateFromYawPitchRoll(ModelRotation.Y, ModelRotation.Y, ModelRotation.Z) *

										   Matrix.CreateTranslation(ModelPosition);

				//foreach (BasicEffect effect in mesh.Effects)
				for (int j = 0; j < mesh.Effects.Count; j++)
				{
					BasicEffect effect = (BasicEffect)mesh.Effects[j];
					effect.PreferPerPixelLighting = true;
					effect.EmissiveColor = EmissiveColor;

					effect.LightingEnabled = true;

					effect.DirectionalLight0.Enabled = true;
					effect.DirectionalLight0.Direction = new Vector3(0, 0, -1);
					effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
					effect.TextureEnabled = false;
					effect.DirectionalLight0.SpecularColor = new Vector3(.2f, .2f , .2f);

					effect.DirectionalLight1.Enabled = false;
					effect.DirectionalLight2.Enabled = false;

					effect.World = effectWorldMatrix;
					effect.View = Camera.View;
					effect.Projection = Camera.Projection;
					//foreach (EffectPass pass in effect.CurrentTechnique.Passes)
					for (int k = 0; k < effect.CurrentTechnique.Passes.Count; k++)
					{
						//pass.Apply();
						effect.CurrentTechnique.Passes[k].Apply();
					}
				}
				mesh.Draw();
			}
        }

        public bool DoesCollision
        {
            get { return true; }
        }

        public bool IsBoundingBoxUpToDate
        {
            get;
            set;
        }

        public void UpdateBoundingBox()
        {
            boundingBox = ModelDrawer.GetBoundingBoxUsingFindBoundary(this);
            IsBoundingBoxUpToDate = true;
        }
    }
}
