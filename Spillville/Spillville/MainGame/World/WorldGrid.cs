using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Utilities;

namespace Spillville.MainGame.World
{
    /*
     * [0,0] is the bottom left of grid
     */

    class WorldGrid
    {

        public static bool DrawGrid;
        public static GridTile[,] Grid;
        public static float VerticeLength { get; private set; }
        public static Color GridColor;
        public static Rectangle WorldGridBox { get; private set; }
        public static List<GridTile> AjacentTiles {get; private set;}

        public static void Initialize(GraphicsDevice gd, int ncells)
        {
            // Drawing doesnt work on xbox
            AjacentTiles = new List<GridTile>();
            DrawGrid = false;
#if WINDOWS && DEBUG
            DrawGrid = true;
#endif
            GridColor = Color.Green;
            _graphicsDevice = gd;
            NumCells = ncells;
            _effect = new BasicEffect(_graphicsDevice);
            Grid = new GridTile[NumCells, NumCells];
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            
        }

        public static void LoadContent(BoundingBox box,float tscale)
        {
            _scale = tscale;
            // For some reason Box.Min.Y is used instead of Z
            _startLoc = new Vector3(box.Min.X, 15 ,box.Min.Y);

            Debug.WriteLine("StartWorldGrid: "+(_startLoc.X*_scale)+" "+(_startLoc.Z*_scale));

            var maxLoc = new Vector3(box.Max.X, 0, box.Max.Y);
            _normVerticeLength = Math.Min(maxLoc.Z - _startLoc.Z,  maxLoc.X - _startLoc.X) / NumCells;
            VerticeLength = _normVerticeLength * _scale;

            WorldGridBox = new Rectangle((int)(_startLoc.X * _scale), (int)(_startLoc.Z * _scale), (int)(VerticeLength * NumCells), (int)(VerticeLength * NumCells));

            SetupGridTiles();
            SetupAdjacencies();
            SetupVertices();
            SetupIndices();
        }

        public static void Update(GameTime gameTime)
        {
            for (var x = 0; x < NumCells; x++)
            {
                for (var y = 0; y < NumCells; y++)
                {
                    Grid[x, y].Update(gameTime);
                }
            }
        }

        public static GridTile GetEastGridTile(GridTile tile)
        {
            for (var x = 0; x < NumCells; x++)
            {
                for (var y = 0; y < NumCells; y++)
                {
                    if (Grid[x, y] == tile)
                    {
                        return Grid[x+1, y ];
                    }
                }
            }

            return tile;
        }

        public static GridTile GetWestGridTile(GridTile tile)
        {
            for (var x = 0; x < NumCells; x++)
            {
                for (var y = 0; y < NumCells; y++)
                {
                    if (Grid[x, y] == tile)
                    {
                        return Grid[x-1, y];
                    }
                }
            }

            return tile;
        }

        public static GridTile GetNorthGridTile(GridTile tile)
        {
            for (var x = 0; x < NumCells; x++)
            {
                for (var y = 0; y < NumCells; y++)
                {
                    if (Grid[x, y] == tile)
                    {
                        return Grid[x, y + 1];
                    }
                }
            }

            return tile;
        }

        public static GridTile GetSouthGridTile(GridTile tile)
        {
            for (var x = 0; x < NumCells; x++)
            {
                for (var y = 0; y < NumCells; y++)
                {
                    if (Grid[x, y] == tile)
                    {
                        return Grid[x, y - 1];
                    }
                }
            }

            return tile;
        }

        public static List<GridTile> GetAdjacentGridTiles(GridTile tile)
        {
            var adj = new List<GridTile>();
            for (var x = 0; x < NumCells; x++)
            {
                for (var y = 0; y < NumCells; y++)
                {
                    if (Grid[x, y] == tile)
                    {
                        var xMinCheck = x > 0;
                        var xMaxCheck = x < NumCells-1;
                        var yMinCheck = y > 0;
                        var yMaxCheck = y < NumCells-1;

                        if (xMinCheck)
                        {

                            if (yMinCheck)
                                adj.Add(Grid[x - 1, y - 1]);

                            adj.Add(Grid[x - 1, y]);
                            
                            if(yMaxCheck)
                                adj.Add(Grid[x - 1, y + 1]);   
                        }

                        if (yMinCheck)
                            adj.Add(Grid[x, y + 1]);

                        //add self
                        adj.Add(Grid[x, y]);

                        if (yMaxCheck)
                            adj.Add(Grid[x, y - 1]);

                        if (xMaxCheck)
                        {
                            if (yMinCheck)
                                adj.Add(Grid[x + 1, y - 1]);
                            
                            adj.Add(Grid[x + 1, y]);
                            
                            if(yMaxCheck)
                                adj.Add(Grid[x + 1, y + 1]);
                        }



                        return adj;
                    }
                }
            }
            return adj;
        }

        public static List<GridTile> GetAdjacentGridTilesByMath(GridTile tile)
        {            
            //List<GridTile> tiles = new List<GridTile>();
            AjacentTiles.Clear();

            int x = (int)(((tile.CenterPoint.X / _scale) - _startLoc.X) / _normVerticeLength);
            int y = (int)(((-tile.CenterPoint.Y / _scale) - _startLoc.Z) / _normVerticeLength) + 1;


            //var xMinCheck = x > 0;
            //var xMaxCheck = x < NumCells - 1;
            //var yMinCheck = y > 0;
            //var yMaxCheck = y < NumCells - 1;

            if (x > 1 && y > 1 && x < NumCells - 1 && y < NumCells - 1)
            {
                AjacentTiles.Add(Grid[x - 1, y - 1]);
                AjacentTiles.Add(Grid[x - 1, y]);
                AjacentTiles.Add(Grid[x - 1, y + 1]);
                AjacentTiles.Add(Grid[x, y + 1]);
                AjacentTiles.Add(Grid[x, y]);
                AjacentTiles.Add(Grid[x, y - 1]);
                AjacentTiles.Add(Grid[x + 1, y - 1]);
                AjacentTiles.Add(Grid[x + 1, y]);
                AjacentTiles.Add(Grid[x + 1, y + 1]);
            }
            //else
            //{
            //    if (xMinCheck)
            //    {

            //        if (yMinCheck)
            //            AjacentTiles.Add(Grid[x - 1, y - 1]);

            //        AjacentTiles.Add(Grid[x - 1, y]);

            //        if (yMaxCheck)
            //            AjacentTiles.Add(Grid[x - 1, y + 1]);
            //    }

            //    if (yMinCheck)
            //        AjacentTiles.Add(Grid[x, y + 1]);

            //    //add self
            //    AjacentTiles.Add(Grid[x, y]);

            //    if (yMaxCheck)
            //        AjacentTiles.Add(Grid[x, y - 1]);

            //    if (xMaxCheck)
            //    {
            //        if (yMinCheck)
            //            AjacentTiles.Add(Grid[x + 1, y - 1]);

            //        AjacentTiles.Add(Grid[x + 1, y]);

            //        if (yMaxCheck)
            //            AjacentTiles.Add(Grid[x + 1, y + 1]);
            //    }
            //}

            return AjacentTiles;
        }

        public static GridTile GetTileAtLocationByMath(float i, float j)
        {

            int x = (int) (((i / _scale) - _startLoc.X)/_normVerticeLength);
            int y = (int) (((-j / _scale) - _startLoc.Z)/_normVerticeLength)+1;


            if (x > 0 && x < NumCells && y > 0 && y < NumCells)
            {
                return Grid[x, y];
            }                            

            return null;
        }

        public static GridTile GridTileAtLocation(float i, float j)
        {
            for (var x = 0; x < NumCells; x++)
            {
                for (var y = 0; y < NumCells; y++)
                {
                    if (Grid[x, y].ContainsPoint(i, j))
                        return Grid[x, y];
                }
            }
            return null;
        }

        public static GridTile GridTileAtLocation(Vector2 location)
        {
            return GridTileAtLocation(location.X, location.Y);
        }

        public static GridTile GridTileAtLocation(Vector3 location)
        {
            return GridTileAtLocation(location.X, location.Z);
        }

        public static int GetLogicalTileDistance(GridTile g1, GridTile g2)
        {
            var x1=0;
            var y1=0;
            var x2=0;
            var y2=0;
            var c = 0;
            for (var x = 0; x < NumCells && c < 2; x++)
            {
                for (var y = 0; y < NumCells && c<2; y++)
                {
                    if (Grid[x, y].Equals(g1))
                    {
                        x1 = x;
                        y1 = y;
                        c++;
                    }
                    if (Grid[x, y].Equals(g2))
                    {
                        x2 = x;
                        y2 = y;
                        c++;
                    }
                }
            }
            return (int)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }


        public bool IsPointOnWorldGrid(float x, float y)
        {
            return WorldGridBox.Contains((int)x, (int)y);
        }

        public static void Draw(GameTime gameTime)
        {
            /*
            for (var x = 0; x < NumCells; x++)
            {
                for (var y = 0; y < NumCells; y++)
                {
                    Grid[x, y].Draw(gameTime);
                }
            }*/

            _effect.VertexColorEnabled = true;
            _effect.View = Camera.View;
            _effect.Projection = Camera.Projection;
            _effect.CurrentTechnique.Passes[0].Apply();
            // Only 2 verts per primative, so indices.Length/2 lines will be drawn
            _graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length / 2, VertexPositionColor.VertexDeclaration);
        }

        /*
        public static void Draw(GameTime gameTime,int a)
        {
            for (int x = 0; x < numCells; x++)
            {
                for (int y = 0; y < numCells; y++)
                {
                    GridTile g = Grid[x, y];
                    effect.VertexColorEnabled = true;
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                    effect.World = Matrix.Identity;


                    effect.CurrentTechnique.Passes[0].Apply();
                    // Only 2 verts per primative, so indices.Length/2 lines will be drawn
                    
                    graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, g.vertices, 0, 4, g.indices, 0, 4, VertexPositionColor.VertexDeclaration);
  
                    
                }
            }
        }*/

        public static void Reset()
        {
            for (var x = 0; x < NumCells; x++)
            {
                for (var y = 0; y < NumCells; y++)
                {
                    Grid[x, y].Reset();
                }
            }
        }
        
        private static int[] _indices;
        private static VertexPositionColor[] _vertices;
        public static int NumCells { get; private set; }
        private static float _scale;
        private static BasicEffect _effect;
        //private static RasterizerState rast;
        private static GraphicsDevice _graphicsDevice;
        private static Vector3 _startLoc;
        private static float _normVerticeLength;
        private static SpriteBatch _spriteBatch;

        private static void SetupVertices()
        {
            _vertices = new VertexPositionColor[(NumCells + 1) * (NumCells + 1)];
            for (int x = 0; x < (NumCells + 1); x++)
            {
                for (int y = 0; y < (NumCells + 1); y++)
                {
                    _vertices[x + y * (NumCells + 1)] = new VertexPositionColor(new Vector3((_startLoc.X + (x * _normVerticeLength)) * _scale, _startLoc.Y, -(_startLoc.Z + (y * _normVerticeLength)) * _scale), GridColor);
                }
            }
        }

        private static void SetupGridTiles()
        {
            for (int x = 0; x < NumCells; x++)
            {
                for (int y = 0; y < NumCells; y++)
                {
                    Grid[x, y] = new GridTile(new Vector2((_startLoc.X + (x * _normVerticeLength)) * _scale, -(_startLoc.Z + (y * _normVerticeLength)) * _scale));
                }
            }
        }

        private static void SetupAdjacencies()
        {
            for (int x = 0; x < NumCells; x++)
            {
                for (int y = 0; y < NumCells; y++)
                {
                    var xMinCheck = x > 0;
                    var xMaxCheck = x < NumCells - 1;
                    var yMinCheck = y > 0;
                    var yMaxCheck = y < NumCells - 1;
                    GridTile N = null, NE = null, E = null, SE = null, S = null, SW = null, W = null, NW = null;

                    if (xMinCheck)
                    {

                        if (yMinCheck)
                            SW = Grid[x - 1, y - 1];

                        W = Grid[x - 1, y];

                        if (yMaxCheck)
                            NW = Grid[x - 1, y + 1];
                    }

                    if (yMinCheck)
                        S = Grid[x, y - 1];


                    if (yMaxCheck)
                        N = Grid[x, y + 1];

                    if (xMaxCheck)
                    {
                        if (yMinCheck)
                            SE = Grid[x + 1, y - 1];

                        E = Grid[x + 1, y];

                        if (yMaxCheck)
                            NE = Grid[x + 1, y + 1];
                    }

                    Grid[x, y].SetAdjacentTiles(N,NE,E,SE,S,SW,W,NW);
                }
            }
        }
        
        private static void SetupIndices()
        {
            _indices = new int[(NumCells + 1) * (NumCells + 1) * 8];

            int c = 0;
            for (int y = 0; y < NumCells; y++)
            {
                for (int x = 0; x < NumCells; x++)
                {
                    int lowerLeft = x + y * (NumCells + 1);
                    int lowerRight = (x + 1) + y * (NumCells + 1);
                    int topLeft = x + (y + 1) * (NumCells + 1);
                    int topRight = (x + 1) + (y + 1) * (NumCells + 1);

                    _indices[c++] = topLeft;
                    _indices[c++] = lowerLeft;
                    _indices[c++] = topLeft;
                    _indices[c++] = topRight;

                    if (x == NumCells - 1)
                    {
                        _indices[c++] = topRight;
                        _indices[c++] = lowerRight;
                    }

                    if (y == NumCells - 1)
                    {
                        _indices[c++] = lowerLeft;
                        _indices[c++] = lowerRight;
                    }
                }
            }
        }
    }
}
