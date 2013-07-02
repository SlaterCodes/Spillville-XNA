using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Spillville.Utilities;

namespace Spillville.MainGame.World
{
    public class Quad
    {
        public VertexPositionNormalTexture[] Vertices;
        public Vector3 Origin;
        public Vector3 Up;
        public Vector3 Normal;
        public Vector3 Left;
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
        public int[] Indexes;

        private GraphicsDevice graphicsDevice;
        private RasterizerState rasterizer;
        private BasicEffect oilEffect;
        private Texture2D OilTexture;
        private VertexDeclaration vertexDeclaration;

        private float height;
        private float width;

        //private float oilSpillX = 900;
        //private float oilSpilly = 900;

        public Matrix billboardMatrix;

        //
        public Vector3 ScreenCoordinates;
        private SpriteBatch spriteBatch;

        public Quad(Vector3 origin, Vector3 normal, Vector3 up,
                 float width, float height, GraphicsDevice graphics, Texture2D oilTexture)
        {
            graphicsDevice = graphics;
            this.OilTexture = oilTexture;
            rasterizer = new RasterizerState();
            oilEffect = new BasicEffect(graphicsDevice);
            
            this.Vertices = new VertexPositionNormalTexture[4];
            this.Indexes = new int[6];
            this.Origin = origin;
            this.Normal = normal;
            this.Up = up;

            this.height = height;
            this.width = width;

            // Calculate the quad corners
            CalculateCorners();
            this.FillVertices();

            vertexDeclaration = new VertexDeclaration(new VertexElement[]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                    new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
                }
            );

            spriteBatch = new SpriteBatch(graphicsDevice);
            ScreenCoordinates = Vector3.Zero;
        }

        public void CalculateCorners()
        {
            this.Left = Vector3.Cross(this.Normal, this.Up);
            Vector3 uppercenter = (this.Up * this.height / 2) + this.Origin;
            this.UpperLeft = uppercenter + (this.Left * this.width / 2);
            this.UpperRight = uppercenter - (this.Left * this.width / 2);
            this.LowerLeft = this.UpperLeft - (this.Up * this.height);
            this.LowerRight = this.UpperRight - (this.Up * this.height);
        }

        private void FillVertices()
        {
            //Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            //Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            //Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            //Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            
            for (int i = 0; i < this.Vertices.Length; i++)
            {
                this.Vertices[i].Normal = this.Normal;
                //this.Vertices[i].Normal = Vector3.Up;
            }
            CalculateNewTextureCorners();

            this.Indexes[0] = 0;
            this.Indexes[1] = 1;
            this.Indexes[2] = 2;
            this.Indexes[3] = 2;
            this.Indexes[4] = 1;
            this.Indexes[5] = 3;
        }

        public void CalculateNewTextureCorners()
        {
            //Vector2 textureUpperLeft = new Vector2((Origin.X - width) / oilSpillX, (Origin.Y - height) / oilSpilly);
            //Vector2 textureUpperRight = new Vector2((Origin.X + width) / oilSpillX, (Origin.Y - height) / oilSpilly);
            //Vector2 textureLowerLeft = new Vector2((Origin.X - width) / oilSpillX, (Origin.Y + height) / oilSpilly);
            //Vector2 textureLowerRight = new Vector2((Origin.X + width) / oilSpillX, (Origin.Y + height) / oilSpilly);

            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            this.Vertices[0].Position = this.LowerLeft;
            this.Vertices[0].TextureCoordinate = textureLowerLeft;
            this.Vertices[1].Position = this.UpperLeft;
            this.Vertices[1].TextureCoordinate = textureUpperLeft;
            this.Vertices[2].Position = this.LowerRight;
            this.Vertices[2].TextureCoordinate = textureLowerRight;
            this.Vertices[3].Position = this.UpperRight;
            this.Vertices[3].TextureCoordinate = textureUpperRight;

        }

        public void CreationTranslation(Vector3 location)
        {
            this.Origin = location;
            CalculateCorners();
            CalculateNewTextureCorners();
            //FillVertices();
        }

        
        public void Update(Vector3 modelPosition)
        {
             //billboardMatrix = Matrix.CreateBillboard(Vector3.Up, Camera.Position, Vector3.Up, null);
            //output.Position = Matrix.Multiply(output.Position, billboardRotation);
            //Matrix effectWorldMatrix = InputController.Instance.GetEffectWorldMatrix();
            Matrix effectWorldMatrix = Matrix.Identity;
            var MyDefaultViewport = graphicsDevice.Viewport;
            ScreenCoordinates = MyDefaultViewport.Project(modelPosition, Camera.Projection, Camera.View, effectWorldMatrix);

        }

        public void Draw(GameTime gameTime)
        {
            
            //////spriteBatch.Begin();
            //////spriteBatch.Draw(OilTexture, new Vector2(ScreenCoordinates.X, ScreenCoordinates.Y), Color.White);
            //////spriteBatch.End();
            ////graphicsDevice.BlendState = BlendState.AlphaBlend;
            

            //oilEffect.World = billboardMatrix;
            //oilEffect.View = Camera.View;
            //oilEffect.Projection = Camera.Projection;
            //oilEffect.TextureEnabled = true;
            //oilEffect.Texture = OilTexture;

            //oilEffect.EnableDefaultLighting();
            //oilEffect.PreferPerPixelLighting = true;

            ////oilEffect.VertexColorEnabled = false;



            //foreach (EffectPass pass in oilEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();

            //    graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>
            //            (PrimitiveType.TriangleList,
            //            Vertices,
            //            0,
            //            4,
            //            Indexes,
            //            0,
            //            2);
            //}


        }
    
    }
}
