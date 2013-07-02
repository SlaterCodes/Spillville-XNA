using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Spillville.MainGame.World;

namespace Spillville.MainGame.OilSpillContainer
{
    public class OilDrawData
    {
        public Matrix[] OilSpotMatrix { get; private set; }
        public Vector3[] OilSpotCoordinates { get; private set; }
        private Vector2 _gridCorner;
        private const short OilBlobDensity = 1;

        public OilDrawData(Vector2 gridTileCorner)
        {
            _gridCorner = gridTileCorner;
            OilSpotMatrix = new Matrix[OilBlobDensity];
            OilSpotCoordinates = new Vector3[OilBlobDensity];

            SetupOilSpotCoords();
        }

        public void Update(GameTime gameTime)
        {
            for (var x = 0; x < OilSpotCoordinates.Length; x++)
            {
                OilSpotCoordinates[x].Y += WaterShader.GetWaveHeight(OilSpotCoordinates[x].Z);
                OilSpotMatrix[x] = Matrix.CreateTranslation(OilSpotCoordinates[x]);
            }
        }

        private void SetupOilSpotCoords()
        {
            var rand = new Random();

            //OilSpotCoordinates[0] = new Vector3(_gridCorner.X + 60, 0,_gridCorner.Y+60);
            //OilSpotCoordinates[1] = new Vector3(_gridCorner.X + 60, 0, _gridCorner.Y+80);
            //OilSpotCoordinates[2] = new Vector3(_gridCorner.X + 80, 0, _gridCorner.Y+70);
            
            for (var x = 0; x < OilBlobDensity; x++)
            {
                var tempCoord = new Vector3(
                    _gridCorner.X + 50+rand.Next(40),
                    -50,
                    _gridCorner.Y + 50+rand.Next(40));
                OilSpotCoordinates[x] = tempCoord;
            }
        }

        public void Reset()
        {
            // TODO clear arrays
        }
    }
}
