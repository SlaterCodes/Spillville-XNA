using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Utilities;
using Spillville.Models;
using Spillville.MainGame.World;
using Spillville.Models.Boats;

namespace Spillville.MainGame.OilSpillContainer
{
    public class OilSpill
    {

        public TimeSpan EntryTime;
        public bool Deployed;
        private List<GridTile> _tiles;
        public List<GridTile> Tiles { get { return _tiles; } }
        private TimeSpan _oilSpreadRate;
        private TimeSpan _oilSpreadRateTrack;
        public bool IsOilSpreading { get; set; }

        public bool Initialized { get; private set; }
        private int _numTiles;
        public int InitialSize { get; private set; }

        private GridTile _initialCenterTile;

        public OilSpill()
        {
            _tiles = new List<GridTile>();
        }

        public void Initialize(Vector2 center, int numTiles)
        {
            _initialCenterTile = WorldGrid.GridTileAtLocation(center.X, center.Y);

            _oilSpreadRate = TimeSpan.FromSeconds(15);
            Deployed = false;
            if (_initialCenterTile.OccupiedOilSpill != null)
                throw new Exception("OilSpill location taken");

            InitialSize = numTiles;
            _numTiles = numTiles-1;
            this.Initialized = true;

            _tiles.Add(_initialCenterTile);
            
            _initialCenterTile.OccupiedOilSpill = this;

            PopulateOilSpill();

            Debug.WriteLine("Oil Spill Tiles: "+_tiles.Count);

            IsOilSpreading = true;
        }



        private void PopulateOilSpill()
        {
            var rand = new Random();
            while (_numTiles > 0)
            {
                
                var tempTilesFree = WorldGrid.GetAdjacentGridTiles(_tiles[rand.Next(_tiles.Count-1)]);

                var newlist = tempTilesFree.TakeWhile<GridTile>(gTile => !gTile.HasOil).ToList<GridTile>();

                var n = newlist.Count;
                while (n > 1)
                {
                    n--;
                    var k = rand.Next(n + 1);
                    var value = newlist[k];
                    newlist[k] = newlist[n];
                    newlist[n] = value;
                }

                foreach (var g in newlist)
                {
                    if (_numTiles > 0 && !g.HasOil)
                    {
                        g.OccupiedOilSpill = this;
                        _numTiles--;
                        _tiles.Add(g);
                    }
                }

            }
        }

        public bool CleanOilAtGridTile(GridTile tile)
        {
            tile.OccupiedOilSpill = null;            
            return _tiles.Remove(tile);
        }

        public GridTile GetClosestGridTileInSpill(Vector3 location)
        {
            return GetClosestGridTileInSpill(WorldGrid.GetTileAtLocationByMath(location.X, location.Z));
        }

        public GridTile GetClosestGridTileInSpill(GridTile locationGrid)
        {
            var closest = _tiles[0];
            var min = WorldGrid.GetLogicalTileDistance(locationGrid, closest);
            for (var i = 1; i < _tiles.Count; i++)
            {
                var tmp = _tiles[i];
                var dist = WorldGrid.GetLogicalTileDistance(locationGrid, tmp);
                if (dist < min)
                {
                    closest = tmp;
                    min = dist;
                }
            }
            return closest;
        }        

        public void Update(GameTime gameTime)
        {
            _oilSpreadRateTrack += gameTime.ElapsedGameTime;
            // Stops spreading if half the original oil spill has been cleaned
            if (IsOilSpreading && _oilSpreadRateTrack > _oilSpreadRate && _tiles.Count > InitialSize / 2)
            {
                Spillville.ThreadPool.AddTask(SpreadOil, delegate(Task t, Exception exc) { }, null);
                _oilSpreadRateTrack = TimeSpan.Zero;
            }
        }

        public void SpreadOil()
        {
            var optGridTile = GetOptimalSpreadTile();
            var infectTiles =
                WorldGrid.GetAdjacentGridTiles(optGridTile).Where(g => !g.HasOil && !g.HasBarricade);
            bool cont = true;
            for (var i = 0; i < infectTiles.Count() && cont; i++)
            {
                var infectTile = infectTiles.ElementAt(i);
                // Prevent different oil spills from grabbing same tile in spread
                lock (this)
                {
                    if (infectTile != null && !infectTile.HasBarricade && !infectTile.HasOil)
                    {
                        cont = false;
                        infectTile.OccupiedOilSpill = this;
                        _tiles.Add(infectTile);
                    }
                }
            }
        }

        public GridTile GetOptimalSpreadTile()
        {
            var optimal = _tiles[0];
            var optCount = WorldGrid.GetAdjacentGridTiles(optimal).Count(tmp => !tmp.HasOil);
            for (var i = 1; i < _tiles.Count; i++)
            {
                var tmpg = _tiles[i];
                var tmpCount = WorldGrid.GetAdjacentGridTiles(tmpg).Count(tmp => !tmp.HasOil);
                if ((tmpCount < optCount && tmpCount > 0) || optCount==0)
                {
                    optimal = tmpg;
                    optCount = tmpCount;
                }
            }
            return optimal;
        }

        public bool IsOilSpillCleaned
        {
            get { return _tiles.Count == 0; }
        }

        /*
                public Vector3 CoordinatesToGrid(int x, int z)
                {

                    int i = (x / 100) + 49;
                    int j = (z / 100) + 49;

                    if (i < 0 || i > 100 || j < 0 || j > 100)
                    {
                        return Vector3.Up;
                    }

                    return worldGrid[i, j].gridCoordinates;
                }

                public Boolean RequestJob(Vector3 location)
                {
                    return RequestJob((int)location.X, (int)location.Z);
                }

                public Boolean RequestJob(int x, int z)
                {
                    lock (thisLock)
                    {

                    if (HasOil(x, z))
                    {
                        int i = (x / 100) + 49;
                        int j = (z / 100) + 49;
                
                        if (worldGrid[i, j].jobTaken)
                            return false;


                        worldGrid[i, j].jobTaken = true;
                        return true;
                    }

                    return false;
                    }
                }

                public Boolean HasOil(Vector3 location)
                {
                    return HasOil((int)location.X, (int)location.Z);
                }

                public Boolean HasOil(int x, int z)
                {
                    lock (thisLock)
                    {
                        int i = (x / 100) + 49;
                        int j = (z / 100) + 49;

                        if (i < 0 || i > 100 || j < 0 || j > 100)
                        {
                            return false;
                        }

                         return worldGrid[i, j].hasSpill;
                    }
                }

                public void CleanOilAt(Vector3 location)
                {
                    CleanOilAt((int)location.X, (int)location.Z);
                }

                public void CleanOilAt(int x, int z)
                {
                    //lock (thisLock)
                    //{
                        if (HasOil(x, z))
                        {
                            int i = (x / 100) + 49;
                            int j = (z / 100) + 49;


                            worldGrid[i, j].coordinates.Clear();

                            if (!worldGrid[i, j].hasSpill)
                            {
                                throw new Exception("CleanOil Algorithm is messed up");
                            }
                            worldGrid[i, j].hasSpill = false;
                            worldGrid[i, j].jobTaken = false;

                        }
                    //}
                }
                public int CountOil
                {
                    get
                    {
                        var c = 0;
                        for (var i = 0; i < worldGrid.GetUpperBound(0); i++)
                        {
                            for (var j = 0; j < worldGrid.GetUpperBound(1); j++)
                            {
                                c += worldGrid[i, j].coordinates.Count;
                            }
                        }
                        return c;
                    }
                }
                private static int[,] OilMap1 = { {0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0}
        ,{0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0}
        ,{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0}
        ,{0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0}
        ,{0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0}
        ,{0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0}
        ,{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0}
        ,{0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        ,{0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0}
        ,{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0}
        ,{0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0}
        ,{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        ,{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0}
        ,{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0}
        ,{0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        ,{0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0}
        ,{0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0}
        ,{0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0}
        ,{0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0}
        ,{0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0}};*/

    }
}
