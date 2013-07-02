using System;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Models;
using Spillville.Models.Boats;
using Spillville.Models.Animals;
using Spillville.MainGame.OilSpillContainer;
using Spillville.Models.Objects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Spillville.MainGame.World
{
    public class GridTile : IEquatable<GridTile>
    {
        public Boat OccupiedBoat;
        public Animal OccupiedAnimal;
        public OilSpill OccupiedOilSpill;
        public Boat ReservedBy { get; private set; }
        public Barricade OccupiedBaricade { get; private set; }
        public OilDrawData OilDrawInfo;

        // Adjacent Tiles
        public GridTile N { get; private set; }
        public GridTile NE { get; private set;}
        public GridTile E { get; private set;}
        public GridTile SE { get; private set;}
        public GridTile S { get; private set;}
        public GridTile SW { get; private set;}
        public GridTile W { get; private set;}
        public GridTile NW { get; private set;}

        // Helper Properties
        public bool HasOil { get { return OccupiedOilSpill != null; } }
        public bool HasBoat { get { return OccupiedBoat != null; } }
        public bool HasAnimal { get { return OccupiedAnimal != null; } }
        public bool Reserved { get { return ReservedBy != null; } }
        public bool HasBarricade { get { return OccupiedBaricade != null; } }
        
        public Vector2 Corner { get; private set; }
        public Vector2 CenterPoint { get; private set; }

        public GridTile(Vector2 loc)
        {
            Corner = loc;
            CenterPoint = new Vector2(Corner.X + WorldGrid.VerticeLength / 2, Corner.Y + WorldGrid.VerticeLength / 2);
            OilDrawInfo = new OilDrawData(Corner);
        }

        public bool ReserveTile(Boat boat)
        {
            if (!Reserved)
            {
                ReservedBy = boat;
                return true;
            }

            return false;
        }

        public bool UnReserved(Boat boat)
        {
            if (boat == ReservedBy)
            {
                ReservedBy = null;
                return true;
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            if(HasOil)
                OilDrawInfo.Update(gameTime);
        }

        public bool CanCreateBarricade
        {
            get { return !HasBarricade && !HasOil && !HasBoat && !HasAnimal; }
        }

        public void CreateBarricade()
        {
            if(CanCreateBarricade)
            {
                var bar = new Barricade(this);
                bar.Initialize();
                OccupiedBaricade = bar;
                GameStatus.RegisterDrawableUnit(bar);
            }
        }

        public void DestroyBarricade()
        {
            if(HasBarricade)
            {
                GameStatus.UnRegisterDrawableUnit(OccupiedBaricade);
                OccupiedBaricade = null;
            }
        }

        public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
            
        }

        public bool ContainsPoint(Vector2 vec)
        {
            return ContainsPoint(vec.X, vec.Y);
        }
        public bool ContainsPoint(float x, float y)
        {
            return (x >= Corner.X && x < Corner.X + WorldGrid.VerticeLength) &&
                (y >= Corner.Y && y < Corner.Y + WorldGrid.VerticeLength);
        }

        public bool Equals(GridTile other)
        {
            return Corner.Equals(other.Corner);
        }

        public void Reset()
        {
            OccupiedBoat = null;
            OccupiedAnimal = null;
            OccupiedOilSpill = null;
            OccupiedBaricade = null;
            ReservedBy = null;
            OilDrawInfo.Reset();
            OilDrawInfo = null;
        }

        public override string ToString()
        {
            return String.Format("Position:{0},Oil:{1},Boat:{2}", 
                Corner.ToString(), 
                HasOil,
                HasBoat?OccupiedBoat.BoatType:"false");
        }

        public List<GridTile> GetAdjacentTiles()
        {
            var Tiles = new List<GridTile>();
            if (N != null)
                Tiles.Add(N);
            if (NE != null)
                Tiles.Add(NE);
            if (E != null)
                Tiles.Add(E);
            if (SE != null)
                Tiles.Add(SE);
            if (S != null)
                Tiles.Add(S);
            if (SW != null)
                Tiles.Add(SW);
            if (W != null)
                Tiles.Add(W);
            if (NW != null)
                Tiles.Add(NW);

            return Tiles;

        }

        public void SetAdjacentTiles(GridTile n, GridTile ne, GridTile e, GridTile se, GridTile s, GridTile sw, GridTile w, GridTile nw)
        {
            N = n;
            NE = ne;
            E = e;
            SE = se;
            S = s;
            SW = sw;
            W = w;
            NW = nw;
        }
    }
}


