using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Spillville.Models
{

	public class Cursor : IDrawableModel
	{
		private Vector3 _modelPosition;

		public Model ModelObject { get; private set; }
        public Vector3 ModelRotation { get; private set; }
		public Vector3 ModelPosition
		{
			get { return _modelPosition; }
		}
		public Matrix[] boneTransforms { get; private set; }
		public float ModelScale { get; set; }
		public Vector3 EmissiveColor { get; set; }
		public BoundingBox boundingBox { get; private set; }

		public void SetPosition(Vector3 target)
		{
			_modelPosition.X = target.X;                        
			_modelPosition.Z = target.Z;
			IsBoundingBoxUpToDate = false;
		}

		public void Initialize(Model model)
		{
			_modelPosition = new Vector3(0, 18, 0);

			ModelObject = model;
			ModelScale = 5.0f;
			ModelRotation = Vector3.Zero;
			boneTransforms = ModelDrawer.GetBoneTransforms(ModelObject);
		}

		public void Update(GameTime gameTime)
		{
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
		}

		public bool DoesCollision
		{
			get { return true; }
		}

		public bool IsBoundingBoxUpToDate { get; private set; }

		public void UpdateBoundingBox()
		{
			boundingBox = ModelDrawer.GetBoundingBoxUsingFindBoundary(this);
			IsBoundingBoxUpToDate = true;
		}

		public bool IsSelectable
		{
			get { return false; }
		}
	}
}
