using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Utilities;

namespace Spillville.MainGame.World
{
    public static class WaterShader
    {
        // Variables for Matrix calculations, viewport and object movment

        static float moveObject = 0;
        static GraphicsDevice graphicsDevice;
        static Model m_Ocean;

        static Effect effectOcean;

        static Texture2D colorOceanMap;
        static Texture2D normalOceanMap;
        static Texture2D reflectOceanMap;
        static Matrix renderMatrix, worldMatrix;
        static Matrix[] oceanBones;
        static private RasterizerState rast;
        static Vector4 vLightDirection;
        static Vector4 vecEye;
        //static public float WaveHeight;

        static public BoundingBox WaterBox { get; private set; }

        static private float _waveSize;
        static public float WaveSize
        {
            // must be waveSize>0 , smaller = bigger waves
            get { return _waveSize; }
            set { _waveSize = MathHelper.Clamp(value, 1f, 64f); }
        }
        static private float _waveSpeed;
        static public float WaveSpeed
        {
            // must be waveSize>0 , smaller = bigger waves
            get { return _waveSpeed; }
            set { _waveSpeed = MathHelper.Clamp(value,1f,64f); }
        }
        static private float _waterAlpha;
        static public float WaterAlpha
        {
            get { return _waterAlpha; }
            set { _waterAlpha = MathHelper.Clamp(value, .1f, 1f); }
        }

        static public float scale { get; private set; }

        public static void Initialize(GraphicsDevice gd)
        {
            graphicsDevice = gd;

            WaveSize = 8f;
            WaveSpeed = 8.0f;
            WaterAlpha = .6f;

            // Set worldMatrix to Identity
            worldMatrix = Matrix.Identity;

            scale = 100.0f;

            //vLightDirection = new Vector4(1.0f, 0.0f, -1.0f, 1.0f);
            vLightDirection = new Vector4(1.0f, 0.0f, -5.0f, 1.0f);
            vecEye = new Vector4(Camera.Position.X, Camera.Position.Y, Camera.Position.Z, 0);

            rast = new RasterizerState();
            rast.CullMode = CullMode.None;
            renderMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationX(MathHelper.ToRadians(270)); 
        }

        public static void LoadContent(ContentManager content)
        {
            effectOcean = content.Load<Effect>(@"watershader\Ocean");

            colorOceanMap = content.Load<Texture2D>(@"watershader\water-sephia");
            normalOceanMap = content.Load<Texture2D>(@"watershader\wavesbump");
            reflectOceanMap = content.Load<Texture2D>(@"watershader\sky_dome2");

            m_Ocean = content.Load<Model>(@"watershader\oceanx");
            
            oceanBones = new Matrix[m_Ocean.Bones.Count];
            m_Ocean.CopyAbsoluteBoneTransformsTo(oceanBones);
            WaterBox = FindBoundary(m_Ocean);
            Console.WriteLine("WaterBox" + WaterBox);
        }

        public static void Update(GameTime gameTime)
        {
            float m = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
            moveObject += m;


            vecEye.X = Camera.Position.X;
            vecEye.Y = Camera.Position.Y;
            vecEye.Z = Camera.Position.Z;


            // Move our object by doing some simple matrix calculations.
            //objectMatrix = Matrix.CreateTranslation(Vector3.Zero);
            //renderMatrix = Matrix.CreateScale(1.0f);
            //viewMatrix = Camera.View;

            //renderMatrix = objectMatrix * renderMatrix;

            //System.Diagnostics.Debug.WriteLine(GetWaveHeight( 800f));

        }

        public static float GetWaveHeight(float Z)
        {
            return 1.8f*((float)Math.Sin((moveObject/2 * WaveSpeed) + (Z * 4.0)) / WaveSize);
        }

        public static void Draw(GameTime gameTime)
        {
            RasterizerState prev = graphicsDevice.RasterizerState;

            graphicsDevice.RasterizerState = rast;
            effectOcean.CurrentTechnique = effectOcean.Techniques["OceanEffect"];
            
            graphicsDevice.BlendState = BlendState.AlphaBlend;


            for (var j = 0;j< effectOcean.CurrentTechnique.Passes.Count;j++)
            {
                EffectPass pass = effectOcean.CurrentTechnique.Passes[j];
                pass.Apply();

                for (var i = 0; i < m_Ocean.Meshes.Count; i++)
                {
                    ModelMesh mesh = m_Ocean.Meshes[i];
                    worldMatrix = oceanBones[mesh.ParentBone.Index]*renderMatrix;

                    for (var index = 0; index < mesh.MeshParts.Count; index++)
                    {
                        ModelMeshPart part = mesh.MeshParts[index];
                        effectOcean.Parameters["matWorldViewProj"].SetValue(worldMatrix*Camera.View*Camera.Projection);
                        effectOcean.Parameters["matWorld"].SetValue(worldMatrix);
                        effectOcean.Parameters["vecEye"].SetValue(vecEye);
                        effectOcean.Parameters["vecLightDir"].SetValue(vLightDirection);
                        effectOcean.Parameters["ColorMap"].SetValue(colorOceanMap);
                        effectOcean.Parameters["BumpMap"].SetValue(normalOceanMap);
                        effectOcean.Parameters["EnvMap"].SetValue(reflectOceanMap);
                        effectOcean.Parameters["time"].SetValue(moveObject/2);
                        effectOcean.Parameters["waveSize"].SetValue(WaveSize);
                        effectOcean.Parameters["waveSpeed"].SetValue(WaveSpeed);
                        effectOcean.Parameters["A"].SetValue(WaterAlpha);
                        effectOcean.Parameters["bSpecular"].SetValue(true);
                        graphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        graphicsDevice.Indices = part.IndexBuffer;
                        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                             part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
            graphicsDevice.RasterizerState = prev;
        }

        private static BoundingBox FindBoundary(Model model)
        {
            BoundingBox _boundingBox = new BoundingBox();
            //_boundingBox.Min = Vector3.Zero;
            //_boundingBox.Max = Vector3.Zero;

            Func<VertexElement, bool> elementPredicate =
                ve =>
                ve.VertexElementUsage == VertexElementUsage.Position &&
                ve.VertexElementFormat == VertexElementFormat.Vector3;

            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix transform = oceanBones[mesh.ParentBone.Index] * worldMatrix;

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    VertexDeclaration vd = meshPart.VertexBuffer.VertexDeclaration;
                    VertexElement[] elements = vd.GetVertexElements();

                    Vector3[] vertexData = new Vector3[meshPart.NumVertices];
                    meshPart.VertexBuffer.GetData(
                        (meshPart.VertexOffset * vd.VertexStride) + elements.First(elementPredicate).Offset,
                        vertexData, 0, vertexData.Length, vd.VertexStride);

                    Vector3[] transformedPositions = new Vector3[vertexData.Length];
                    Vector3.Transform(vertexData, ref transform, transformedPositions);

                    _boundingBox = BoundingBox.CreateMerged(_boundingBox,
                                                           BoundingBox.CreateFromPoints(transformedPositions));
                }
            }
            return _boundingBox;
        }

    }
}
