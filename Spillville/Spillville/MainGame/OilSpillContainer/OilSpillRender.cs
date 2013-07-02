using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spillville.MainGame.World;
using Spillville.Utilities;

namespace Spillville.MainGame.OilSpillContainer
{
    class OilSpillRender
    {
        private GraphicsDevice _graphicsDevice;
        private BasicEffect _effect;
        private RasterizerState _rasterizer;
        private float _scale;
        private int[] _indices;
        private int _sides;
        private VertexPositionColor[] _vertices;


        public void Initialize(GraphicsDevice gd)
        {
            _graphicsDevice = gd;
            _effect = new BasicEffect(_graphicsDevice);
            _rasterizer = new RasterizerState();
            _rasterizer.CullMode = CullMode.None;
            _rasterizer.FillMode = FillMode.Solid;

            _sides = 14;
            _scale = (80.0f * 1.42f);
            _vertices = new VertexPositionColor[_sides + 1];
            SetUpVertices();
            SetUpOctagonIndices();

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(OilSpill spill,GameTime gameTime)
        {
            RasterizerState prev = _graphicsDevice.RasterizerState;
            _graphicsDevice.RasterizerState = _rasterizer;
            for (var i = 0; i < spill.Tiles.Count; i++)
            {
                for (var j = 0; j < spill.Tiles[i].OilDrawInfo.OilSpotMatrix.Length;j++ )
                {
                    _effect.VertexColorEnabled = true;
                    _effect.View = Camera.View;
                    _effect.Projection = Camera.Projection;
                    _effect.World = spill.Tiles[i].OilDrawInfo.OilSpotMatrix[j];
                    _effect.CurrentTechnique.Passes[0].Apply();
                    _graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length / 3, VertexPositionColor.VertexDeclaration);
                }
            }
            _graphicsDevice.RasterizerState = prev;
        }

        private void SetUpVertices()
        {
            var origin = Vector3.Zero;
            _vertices[0] = new VertexPositionColor(origin, Color.Black);

            double angle = 0;
            const float alterScale = 1.38f;

            for (int i = 1; i < _vertices.Length; i++)
            {
                var x = (float)Math.Cos(angle) * _scale * alterScale;
                var y = (float)Math.Sin(angle) * _scale * alterScale;

                _vertices[i] = new VertexPositionColor(
                    new Vector3(origin.X + x,
                                origin.Y,
                                origin.Z + y),
                    Color.Black);

                angle += (2 * Math.PI) / _sides;
            }
        }

        private void SetUpOctagonIndices()
        {
            var c = 0;

            _indices = new int[_sides * 3];

            for (var i = 0; i < _sides - 1; i++)
            {
                var point2 = i + 1;
                var point3 = i + 2;

                _indices[c++] = 0;
                _indices[c++] = point2;
                _indices[c++] = point3;
            }

            _indices[c++] = 0;
            _indices[c++] = _sides;
            _indices[c++] = 1;
        }
    }
}
