using System;
using Spillville.Models;
using Spillville.Utilities;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville.MainGame.LevelSelect
{
	class EarthModel : IDrawableModel
	{
		public Model ModelObject { get; private set; }
        public Vector3 ModelRotation { get; set; }
		public Vector3 ModelPosition { get; set; }
		public float ModelScale { get; private set; }
		public Vector3 EmissiveColor { get; set; }
		public Matrix[] boneTransforms { get; private set; }
		public BoundingBox boundingBox { get; private set; }
        //public BoundingSphere boundingSphere {get; private set;}

		public void Initialize(Model m)
		{
			ModelObject = m;
            ModelRotation = Vector3.Zero;
			ModelScale = 1f;

			boneTransforms = ModelDrawer.GetBoneTransforms(ModelObject);
            UpdateBoundingBox();
            
            System.Diagnostics.Debug.WriteLine("earth radius: " + boundingBox.Max);
		}

		public void Update(GameTime gameTime)
		{
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
		}

        public void Rotate(float rotationValuex)
        {
            var rotation = ModelRotation;
            rotation.Y += rotationValuex;
            ModelRotation = rotation;
        }

		public bool DoesCollision
		{
            get { return false; }
		}

        public float Radius
        {
            get
            {
                // TODO verify this b4 final publish
                return 66.42612f;
            }
        }

        public bool IsBoundingBoxUpToDate
        {
            get;
            set;
        }

		public void UpdateBoundingBox()
		{
            this.boundingBox = ModelDrawer.GetBoundingBoxUsingFindBoundary(this);
            IsBoundingBoxUpToDate = true;
		}
	}
}
