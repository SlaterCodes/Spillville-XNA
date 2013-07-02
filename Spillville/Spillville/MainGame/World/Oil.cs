using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spillville.Utilities;

namespace Spillville.MainGame.World
{
    class Oil
    {
        
        private int[] indices;
        private VertexPositionColor[] vertices;
        //private VertexPositionTexture[] verticesT;

        private int numCells;
        private float scale;
        private Texture2D waterTexture;

        private BasicEffect effect;

        private RasterizerState rast;

        GraphicsDevice graphicsDevice;
        private Vector3 startLoc;

        public Oil(int numCells,float scale,Vector3 startPos,Texture2D waterTexture)
        {
            this.numCells = numCells;
            this.scale = scale;
            this.waterTexture = waterTexture;
            startLoc = startPos;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            effect = new BasicEffect(graphicsDevice);
            rast = new RasterizerState();
            rast.CullMode = CullMode.None;
            rast.FillMode = FillMode.Solid;
            setupVertices();
            setupIndices();
        }

        private void setupVertices()
        {
            vertices = new VertexPositionColor[(numCells + 1) * (numCells + 1)];
            //verticesT = new VertexPositionTexture[(numCells + 1) * (numCells + 1)];

            for (int x = 0; x < (numCells + 1); x++)
            {
                for (int y = 0; y < (numCells + 1); y++)
                {
                    vertices[x + y * (numCells + 1)] = new VertexPositionColor(new Vector3((startLoc.X + x) * scale, startLoc.Y, -(startLoc.Z + y) * scale), Color.Brown);
                    //verticesT[x + y * (numCells + 1)] = new VertexPositionTexture(new Vector3(x * scale, 0, y * scale), new Vector2(x * scale, y * scale));
                    
                }
            }
        }

        private void setupIndices()
        {
            indices = new int[numCells * numCells * 6];
            int c = 0;
            for (int y = 0; y < numCells; y++)
            {
                for (int x = 0; x < numCells; x++)
                {
                    int lowerLeft = x + y * (numCells + 1);
                    int lowerRight = (x + 1) + y * (numCells + 1);
                    int topLeft = x + (y + 1) * (numCells + 1);
                    int topRight = (x + 1) + (y + 1) * (numCells + 1);

                    indices[c++] = topLeft;
                    indices[c++] = lowerRight;
                    indices[c++] = lowerLeft;

                    indices[c++] = topLeft;
                    indices[c++] = topRight;
                    indices[c++] = lowerRight;
                }
            }
        }

        public void Draw(GameTime gameTime )
        {  
            //effect.EnableDefaultLighting();
            RasterizerState prev = graphicsDevice.RasterizerState;
            graphicsDevice.RasterizerState = rast;

            //graphicsDevice.BlendState = BlendState.Additive;

            effect.VertexColorEnabled = true;
            effect.View = Camera.View;
            effect.Projection = Camera.Projection;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();    
            }
            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
            
            graphicsDevice.RasterizerState = prev;

            //graphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}
