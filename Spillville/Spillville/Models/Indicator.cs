using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville.Models
{
	class Indicator : IDrawableModel
	{
		public Model ModelObject { get; private set; }
        public Vector3 ModelRotation { get; private set; }
		public Vector3 ModelPosition { get; set; }
		public Matrix[] boneTransforms { get; private set; }
		public float ModelScale { get; private set; }
		public Vector3 EmissiveColor { get; set; }
		public BoundingBox boundingBox { get; private set; }

		public void Update(GameTime gameTime)
		{
			//do nothing extra
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			//do nothing extra
		}

		public bool DoesCollision
		{
			//indicator never has collsion detection
			get { return false; }
		}

		public bool IsBoundingBoxUpToDate
		{
			get { return true; }
		}

		public void UpdateBoundingBox()
		{
			//do nothing
		}

		public bool IsSelectable
		{
			get { return false; }
		}

		public void Initialize(Model model)
		{
			ModelObject = model;
			ModelScale = 1.0f;
			ModelRotation = Vector3.Zero;
			EmissiveColor = new Vector3(255, 255, 255);
			boneTransforms = ModelDrawer.GetBoneTransforms(ModelObject);
		}


	}
}
