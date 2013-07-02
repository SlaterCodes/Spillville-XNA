using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Spillville.MainGame.OilSpillContainer;
using Spillville.Models.Animals;
using Spillville.Models.Boats;
using Spillville.Models.Objects;

namespace Spillville.MainGame.Levels
{
    public class Level2 : Level
    {
        public override string Name
        {
            get { return "South Pacific Spill";  }
        }

        public override string Description
        {
            get { return "Many small oil spills";  }
        }

        public override int UnitLimit
        {
            get { return 8; }
        }


        public override int StartingFunds
        {
            get { return 10000000; }
        }

        public Level2()
        {
            EarthLocation = new Vector2(80, 90);
            TimeLimit = TimeSpan.FromMinutes(8);
        }

        public override void Initialize()
        {
            OilPlumes = new List<OilSpill>();
            StartingBoats = new List<Boat>();
            Animals = new List<Animal>();
            Achievements = new List<Achievement>();
            InitialPopularity = 50;



            var cship = new CommandShip();
            cship.Initialize(new Vector2(0,1000));
            StartingBoats.Add(cship);


            // TODO require delegate for Earn conditions to be passed in constructor
            var saveDolphin = new Achievement(
                "Save the Dolphin!",
                "Save the dolphin from the Oil",
                10,
                10);

            
            var oil1 = new OilSpill();
            oil1.Initialize(Vector2.Zero,20);
            OilPlumes.Add(oil1);
            var oil2 = new OilSpill();
            oil2.Initialize(new Vector2(1500, 1800), 15);
            OilPlumes.Add(oil2);
            var oil3 = new OilSpill();
            oil3.Initialize(new Vector2(-1500, 1800), 18);
            OilPlumes.Add(oil3);
            var oil4 = new OilSpill();
            oil4.Initialize(new Vector2(-1500, -1800), 19);
            OilPlumes.Add(oil4);
            var oil5 = new OilSpill();
            oil5.Initialize(new Vector2(1500, -1800), 11);
            OilPlumes.Add(oil5);
        }

        public override bool WinGame
        {
            get { return OilPlumes.Count(g => g.Tiles.Count == 0) == OilPlumes.Count; }
        }

        public override bool LoseGame
        {
            get { return GameStatus.Populatity == 0 || GameStatus.TimeElapsed > GameStatus.TimeLimit; }
        }

        public void Reset()
        {
            StartingBoats.Clear();
            Animals.Clear();
            Achievements.Clear();
        }
    }
}
