using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville.Models
{
    public interface IDrawableModel
    {
        Model ModelObject { get; }
        Vector3 ModelRotation { get; }
        Vector3 ModelPosition { get; }
        float ModelScale { get; }
		Vector3 EmissiveColor { get; set; }
        Matrix[] boneTransforms { get; }
        BoundingBox boundingBox { get; }
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime,SpriteBatch spriteBatch);


		/// <summary>
		/// Determine whether collsion detection takes place on this DrawableModel.
		/// For efficiency, this should be turned off when not needed.
		/// </summary>
    	bool DoesCollision { get; }

		/// <summary>
		/// If set to true, UpdateBoundingBox will be called before collision detection occurs.
		/// </summary>
    	bool IsBoundingBoxUpToDate { get; }



    	/// <summary>
		/// Each DrawableModel is resposible for creating their own BoundingBox in a manner
		/// that is most effiecient and effective for that object. UpdateBoundingBox should set
		/// the IsBoundingBoxUpToDate field to true, this field should be set to false once the
		/// DrawableModel has been transformed. The BoundingBox should only be transformed with
		/// the DrawableModel only when nessesary. DoesCollision should be set to false if the
		/// DrawableModel is exempt from collision detection (the model is drawn but is a ghost).
		/// </summary>
    	void UpdateBoundingBox();

    }
}
