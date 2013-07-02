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
    public class Level1 : Level
    {
        public override string Name
        {
            get { return "Indian Ocean Spill";  }
        }

        public override string Description
        {
            get { return "Save the dolphins!";  }
        }

        public override int UnitLimit
        {
            get { return 8; }
        }


        public override int StartingFunds
        {
            get { return 10000000; }
        }

        public Level1()
        {
            EarthLocation = new Vector2(-20, -10);
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

            var dolphin = new Dolphin();
            dolphin.Initialize(new Vector2(0,0));
            Animals.Add(dolphin);
            var dolphin1 = new Dolphin();
            dolphin1.Initialize(new Vector2(-500, 0));
            Animals.Add(dolphin1);
            var dolphin2 = new Dolphin();
            dolphin2.Initialize(new Vector2(-500, 500));
            Animals.Add(dolphin2);
            var dolphin3 = new Dolphin();
            dolphin3.Initialize(new Vector2(-500, -500));
            Animals.Add(dolphin3);

            // TODO require delegate for Earn conditions to be passed in constructor
            var saveDolphin = new Achievement(
                "Save the Dolphin!",
                "Save the dolphin from the Oil",
                10,
                10);

            
            var oil1 = new OilSpill();
            oil1.Initialize(Vector2.Zero,80);
            OilPlumes.Add(oil1);

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
