using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spillville.Utilities;
using Spillville.Models;

namespace Spillville.MainGame.LevelSelect
{
    class CursorModel : IDrawableModel
    {

        public Model ModelObject { get; private set; }
        public Vector3 ModelRotation { get; private set; }
        public Vector3 ModelPosition { get;  set; }
        public float ModelScale { get; private set; }
        public Vector3 EmissiveColor { get; set; }
        public Matrix[] boneTransforms { get; private set; }
        public BoundingBox boundingBox { get; private set; }

        public Rectangle boundaries { get; private set; }
        private Vector3 projectionCoords { get; set; }

        private float _earthBoundRadius;

        public CursorModel()
        {
            EmissiveColor = Vector3.Zero;
            ModelScale = .25f;
        }

        public void Initialize(Model m,Vector3 pos)
        {
            ModelObject = m;
            boneTransforms = ModelDrawer.GetBoneTransforms(ModelObject);
            ModelRotation = new Vector3(MathHelper.ToRadians(90), 0f, 0f);
            ModelPosition = pos;
            _earthBoundRadius = pos.Z;
            projectionCoords = Camera.Viewport.Project(this.ModelPosition, Camera.Projection, Camera.View, Matrix.Identity);
            boundaries = new Rectangle((int)projectionCoords.X - 15, (int)projectionCoords.Y + 15, 30, 30);
        }

        public void MoveCursor(float x,float y)
        {
            var pos = ModelPosition;
            //pos.X = MathHelper.Clamp(pos.X + x * .8f, -110f, 110f);
            //pos.Y = MathHelper.Clamp(pos.Y + y * .8f, -63f, 63f);
            pos.X += x;
            pos.Y += y;

            //var earthRotatex = .02f;

/*
            var xrot = x * .02f;
            var yrot = y * .02f;

            

            var rotMatrix = Matrix.CreateRotationY(xrot);
            var finalVect = Vector3.Transform(ModelPosition, rotMatrix);
            var rotMatrix2 = Matrix.CreateRotationX(yrot);
            var finalVect2 = Vector3.Transform(finalVect,rotMatrix2);

            var rotmat1 = Vector3.TransformNormal(ModelRotation, rotMatrix);
            var rotmat2 = Vector3.TransformNormal(rotmat1, Matrix.Invert(rotMatrix2));
            //Vector3.
            ModelRotation = rotmat2;

            //finalVect.Y += y;
            ModelPosition = finalVect2;*/
            ModelPosition = pos;
            //Debug.WriteLine("Cursor: " + pos.ToString());
            IsBoundingBoxUpToDate = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsBoundingBoxUpToDate)
            {
                projectionCoords = Camera.Viewport.Project(this.ModelPosition, Camera.Projection, Camera.View, Matrix.Identity);
                boundaries = new Rectangle((int)projectionCoords.X - 15, (int)projectionCoords.Y + 15, 30, 30);
                IsBoundingBoxUpToDate = true;
            }                        
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public bool DoesCollision
        {
            get{return true;}
        }

        public bool IsBoundingBoxUpToDate
        {
            get;
            set;
        }

        public void UpdateBoundingBox()
        {
            boundingBox = ModelDrawer.GetBoundingBoxUsingFindBoundary(this);
            //var newMinVect = boundingBox.Min;
            //newMinVect.Z = 0;
            //var minVec = new Vector3(ModelPosition.X-1,ModelPosition.Y-1,ModelPosition.Z - _earthBoundRadius);
            //var maxVec = new Vector3(ModelPosition.X + 1, ModelPosition.Y + 1, ModelPosition.Z);
            //boundingBox = new BoundingBox(minVec, maxVec);
            
            //boundingBox = new BoundingBox(newMinVect, boundingBox.Max);
            IsBoundingBoxUpToDate = true;
        }
    }
}

