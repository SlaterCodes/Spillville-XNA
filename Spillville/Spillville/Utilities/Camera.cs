using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Spillville.Utilities
{
    public static class Camera
    {
        public static Vector3 Position;
        public static Vector3 Target;
        public static Quaternion Rotation;

        private static Matrix World;
        public static Matrix View;
        public static Matrix Projection;
        public static Viewport Viewport;

        public static void Initialize(Viewport view)
        {
            Viewport = view;
            Viewport.MinDepth = 1.0f;
            Viewport.MaxDepth = 10000.0f;

            Target = new Vector3(0,150,1000);
            Position = new Vector3(0, 800, 2000);
            Rotation = new Quaternion(0, 0, 0, 1);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), Viewport.AspectRatio, Viewport.MinDepth, Viewport.MaxDepth);
        }

        public static void Update(GameTime gameTime)
        {
            World = Matrix.Identity;

            View = Matrix.Invert(Matrix.CreateFromQuaternion(Rotation)*
                                 Matrix.CreateTranslation(Position));
     
        }

        public static void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(Rotation));
            Rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * Rotation);
        }

        public static void Translate(Vector3 distance)
        {
            Position += Vector3.Transform(distance, Matrix.CreateFromQuaternion(Rotation));
        }

        public static void Revolve()
        {
        }

		//not so good
//		public static void Revolve(Vector3 axis, float angle)
//		{
//			Rotate(axis, angle);
//
//			var camRot = Matrix.CreateRotationY(angle) * Matrix.CreateFromAxisAngle(axis, angle);
//
//
//			Position = Vector3.Transform(Position, camRot);
//		}

		//better
        public static void Revolve(Vector3 axis, float angle)
        {
            Rotate(axis, angle);

            //version1
            //Matrix rotationMatrix = Matrix.CreateRotationY(3 * angle);
            //Position = Vector3.Transform(Position, rotationMatrix);
            //View = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            //version2
            //Position = Vector3.Transform(Position - Target, Matrix.CreateFromAxisAngle(axis, angle)) + Target;
            //View = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            //version3
            //Position = Vector3.Transform(Position-Target, Matrix.CreateRotationY(angle))+Target;
            //View = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            //version4
            Position = Vector3.Transform(Vector3.Backward, Matrix.CreateFromYawPitchRoll(angle,0,0));
            
            
            View = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            #region Chris's
            //Rotate(axis, angle);
            //Vector3 revolveAxis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(Rotation));
            //Quaternion rotate = Quaternion.CreateFromAxisAngle(revolveAxis, angle);


            //Position = Vector3.Transform(Position, Matrix.CreateFromQuaternion(rotate));

            //Position = Vector3.Transform(Position, Matrix.CreateRotationY(angle));
            #endregion
        }

		//backup
//		public static void Revolve(Vector3 target, Vector3 axis, float angle)
//		{
//			Rotate(axis, angle);
//			Vector3 revolveAxis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(Rotation));
//			Quaternion rotate = Quaternion.CreateFromAxisAngle(revolveAxis, angle);
//			Position = Vector3.Transform(target - Position, Matrix.CreateFromQuaternion(rotate));
//		}
    }
}