using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Spillville.Models
{
    public abstract class BaseModel : IDrawableModel
    {
        // This is abstract because implementing classes should return their static model and not have their own copy
        abstract public Model ModelObject { get; }
        public Vector3 ModelRotation { get; protected set; }
        protected Vector3 _modelPosition;
        public Vector3 ModelPosition { get { return _modelPosition; } }
        public Matrix[] boneTransforms { get; protected set; }
        public float ModelScale { get; protected set; }
        public Vector3 EmissiveColor { get; set; }
        public BoundingBox boundingBox { get; protected set; }

    	protected BaseModel()
        {
            ModelRotation = Vector3.Zero;
            _modelPosition = Vector3.Zero;
            ModelScale = 1;
            EmissiveColor = Vector3.Zero;
        }

        public virtual void Initialize()
        {

        }

        
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    	public virtual bool DoesCollision { get; protected set; }
    	
    	public bool IsBoundingBoxUpToDate { get; protected set; }

    	public void UpdateBoundingBox()
    	{
    		throw new NotImplementedException();
    	}

    	public bool IsSelectable
    	{
			get { return true; }
    	}
    }
}
