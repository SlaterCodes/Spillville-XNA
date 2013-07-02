using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Utilities;
using Spillville.MainGame.LevelSelect;
using Spillville.Models.Animals;
using Spillville.MainGame;

namespace Spillville.Models
{
    class ModelDrawer
    {
        //will only work on Dolphin type right now
        //ToDo: make it more generic
        public static void DrawAnimatedModel(IDrawableModel model)
        {
            Matrix[] bones = ((Dolphin)model).animatedDolphin.GetSkinTransforms();
            Matrix effectWorldMatrix = Matrix.CreateScale(model.ModelScale) *
                  Matrix.CreateFromYawPitchRoll(model.ModelRotation.Y, model.ModelRotation.X, model.ModelRotation.Z) *
                /* Matrix.CreateRotationX(ModelRotation.X) *
                 Matrix.CreateRotationY(ModelRotation.Y) *
                 Matrix.CreateRotationZ(ModelRotation.Z) * */
                           Matrix.CreateTranslation(model.ModelPosition);

            //foreach (ModelMesh mesh in model.ModelObject.Meshes)
            //{
            //    foreach (SkinnedEffect effect in mesh.Effects)
            //    {
            //        effect.SetBoneTransforms(bones);
            //        effect.View = Camera.View;
            //        effect.Projection = Camera.Projection;
            //    }
            //    mesh.Draw();
            //}


            //// Render the skinned mesh.
            for (int i = 0; i < model.ModelObject.Meshes.Count; i++)
            {
                ModelMesh mesh = model.ModelObject.Meshes[i];
                for (int j = 0; j < mesh.Effects.Count; j++)
                {
                    SkinnedEffect effect = (SkinnedEffect)mesh.Effects[j];
                    effect.SetBoneTransforms(bones);

                    effect.View = Camera.View; ;
                    effect.Projection = Camera.Projection;
                    effect.World = effectWorldMatrix;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.EmissiveColor = model.EmissiveColor;
                    effect.SpecularPower = 16;

                    for (int k = 0; k < effect.CurrentTechnique.Passes.Count; k++)
                    {
                        //pass.Apply();
                        effect.CurrentTechnique.Passes[k].Apply();
                    }
                }

                mesh.Draw();
            }
        }

        public static void Draw(IDrawableModel model)
        {
            //foreach (ModelMesh mesh in model.ModelObject.Meshes)
            for(int i=0;i<model.ModelObject.Meshes.Count;i++)
            {
                ModelMesh mesh = model.ModelObject.Meshes[i];

                Matrix effectWorldMatrix = model.boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(model.ModelScale) *
                                      Matrix.CreateFromYawPitchRoll(model.ModelRotation.Y, model.ModelRotation.X, model.ModelRotation.Z) *
                                           Matrix.CreateTranslation(model.ModelPosition);

                //foreach (BasicEffect effect in mesh.Effects)
                for(int j=0;j<mesh.Effects.Count;j++)
                {
                    BasicEffect effect = (BasicEffect) mesh.Effects[j];
                	effect.PreferPerPixelLighting = true;
                    effect.EmissiveColor = model.EmissiveColor;
                    
					//effect.EnableDefaultLighting();
					effect.LightingEnabled = true;
                	effect.DirectionalLight0.Enabled = false;
                	effect.DirectionalLight1.Enabled = false;
                	effect.DirectionalLight2.Enabled = true;
                	effect.DirectionalLight2.Direction = GameConstants.MainDrawLight2Direction;
                	effect.DirectionalLight2.DiffuseColor = GameConstants.MainDrawLight2DiffuseColor;


                    effect.World = effectWorldMatrix;
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                    //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    for(int k=0;k<effect.CurrentTechnique.Passes.Count;k++)
                    {
                        //pass.Apply();
                        effect.CurrentTechnique.Passes[k].Apply();
                    }
                }
                mesh.Draw();
            }

        }

        public static Matrix[] GetBoneTransforms(Model model)
        {
            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            return boneTransforms;
        }


        public static BoundingBox GetBoundingBoxUsingFindBoundary(IDrawableModel model)
        {
            var boundingBox = new BoundingBox();
            boundingBox.Min = Vector3.Transform(Vector3.Zero, Matrix.CreateTranslation(model.ModelPosition));
            boundingBox.Max = Vector3.Transform(Vector3.Zero, Matrix.CreateTranslation(model.ModelPosition));

            Func<VertexElement, bool> elementPredicate =
                ve =>
                ve.VertexElementUsage == VertexElementUsage.Position &&
                ve.VertexElementFormat == VertexElementFormat.Vector3;

            for (int i = 0; i < model.ModelObject.Meshes.Count; i++)
            {
                ModelMesh mesh = model.ModelObject.Meshes[i];
                Matrix transform = model.boneTransforms[mesh.ParentBone.Index]*Matrix.CreateScale(model.ModelScale)*
                                                     Matrix.CreateFromYawPitchRoll(model.ModelRotation.Y, model.ModelRotation.X, model.ModelRotation.Z) *
                    /* Matrix.CreateRotationX(ModelRotation.X) *
                     Matrix.CreateRotationY(ModelRotation.Y) *
                     Matrix.CreateRotationZ(ModelRotation.Z) * */
                                   Matrix.CreateTranslation(model.ModelPosition);

                for (int index = 0; index < mesh.MeshParts.Count; index++)
                {
                    ModelMeshPart meshPart = mesh.MeshParts[index];
                    VertexDeclaration vd = meshPart.VertexBuffer.VertexDeclaration;
                    VertexElement[] elements = vd.GetVertexElements();

                    Vector3[] vertexData = new Vector3[meshPart.NumVertices];
                    meshPart.VertexBuffer.GetData(
                        (meshPart.VertexOffset*vd.VertexStride) + elements.First(elementPredicate).Offset,
                        vertexData, 0, vertexData.Length, vd.VertexStride);

                    Vector3[] transformedPositions = new Vector3[vertexData.Length];
                    Vector3.Transform(vertexData, ref transform, transformedPositions);

                    boundingBox = BoundingBox.CreateMerged(boundingBox,
                                                            BoundingBox.CreateFromPoints(transformedPositions));
                }
            }
            return boundingBox;
        }

        //http://gamedev.stackexchange.com/questions/2438/how-do-i-create-bounding-boxes-with-xna-4-0
        //protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        public static BoundingBox GetBoundingBoxUsingVertexStride(Model model)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
        
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), Camera.View);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                   } 
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }

        // My own method...(Michael)
        public static BoundingSphere GetBoundingBoxUsingSphere(Model model)
        {
            var box = new BoundingSphere();
            foreach(ModelMesh mesh in model.Meshes)
            {
                box = BoundingSphere.CreateMerged(box,mesh.BoundingSphere);
            }
            return box;
        }

        public static void DrawEarth(EarthModel model)
        {
            //foreach (ModelMesh mesh in model.ModelObject.Meshes)
            for (int i = 0; i < model.ModelObject.Meshes.Count; i++)
            {
                ModelMesh mesh = model.ModelObject.Meshes[i];

                Matrix effectWorldMatrix = model.boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(model.ModelScale) *
                                      Matrix.CreateFromYawPitchRoll(model.ModelRotation.Y, model.ModelRotation.X, model.ModelRotation.Z) *
                     Matrix.CreateTranslation(model.ModelPosition);

                //foreach (BasicEffect effect in mesh.Effects)
                for (int j = 0; j < mesh.Effects.Count; j++)
                {
                    BasicEffect effect = (BasicEffect)mesh.Effects[j];

                    effect.EmissiveColor = model.EmissiveColor;

                    effect.LightingEnabled = true;
                    effect.PreferPerPixelLighting = true;

                    //secondary lights to give scatter effect

                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight0.Direction = GameConstants.EarthDrawDirectionLight0;
                    effect.DirectionalLight0.DiffuseColor = GameConstants.EarthDrawDiffuseColor0;

                    effect.DirectionalLight1.Enabled = true;
                    effect.DirectionalLight1.Direction = GameConstants.EarthDrawDirectionLight1;
                    effect.DirectionalLight1.DiffuseColor = GameConstants.EarthDrawDiffuseColor1;

                    //primary light
                    effect.DirectionalLight2.Enabled = true;
                    effect.DirectionalLight2.Direction = GameConstants.EarthDrawDirectionLight2;
                    effect.DirectionalLight2.DiffuseColor = GameConstants.EarthDrawDiffuseColor2;

                    effect.DirectionalLight2.SpecularColor = GameConstants.EarthDrawSpecularColor2;

                    effect.SpecularPower = 200f;

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


    }
}
