///Draw lines for debugging purpose only
/// Remember to the call before we submit or demo the game


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Spillville.Utilities;

namespace Spillville.MainGame.World
{
    public class DrawLine
    {
        private int points = 800;
        private VertexPositionColor[] primitiveList;
        private short[] lineListIndices;
        private GraphicsDevice graphicsDevice;
        private BasicEffect effect;
        private RasterizerState rasterizer;

        //world's edges is roughly x:[-5000,5000] z:[-5000, 5000]
        //so if each grid is size 100,100
        // 100 lines for each axis, 200 lines in total
        // 2 points per line so 400 points
        

        public DrawLine()
        {
            primitiveList = new VertexPositionColor[points];
            lineListIndices = new short[points];
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            effect = new BasicEffect(graphicsDevice);
            rasterizer = new RasterizerState();
            rasterizer.CullMode = CullMode.None;
            rasterizer.FillMode = FillMode.Solid;


            Method1();
            Method2();

        }

        public void Method1()
        {
            //draw vertical lines, then horizontal lines
            int c=0;
            int a = 200;
            for (int i = -5000; i < 5000; i += 100)
            {
                primitiveList[c++] = new VertexPositionColor(new Vector3(i, 0, -5000), Color.Black);
                primitiveList[c++] = new VertexPositionColor(new Vector3(i, 0, 5000), Color.Black);

                primitiveList[a++] = new VertexPositionColor(new Vector3(-5000, 0, i), Color.Black);
                primitiveList[a++] = new VertexPositionColor(new Vector3(5000, 0, i), Color.Black);
            }
            
        }

        public void Method2()
        {            
            for (int i = 0; i < points - 1; i++)
            {
                lineListIndices[i] = (short)(i);
            }
        }

        public void Draw()
        {
            RasterizerState prev = graphicsDevice.RasterizerState;
            graphicsDevice.RasterizerState = rasterizer;


            effect.VertexColorEnabled = true;
            effect.View = Camera.View;
            effect.Projection = Camera.Projection;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>
            ( PrimitiveType.LineList,primitiveList, 0 , lineListIndices.Length, lineListIndices, 0, 400);

            graphicsDevice.RasterizerState = prev;
        }

    }
}
