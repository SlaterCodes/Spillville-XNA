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
    public class Level4 : Level
    {
        public override string Name
        {
            get { return "The Gulf Spill";  }
        }

        public override string Description
        {
            get { return "Massive, fast spreading oil spill";  }
        }

        public override int UnitLimit
        {
            get { return 14; }
        }


        public override int StartingFunds
        {
            get { return 10000000; }
        }

        public Level4()
        {
            EarthLocation = new Vector2(-25, 184);
            TimeLimit = TimeSpan.FromMinutes(8);
        }

        public override void Initialize()
        {
            OilPlumes = new List<OilSpill>();
            StartingBoats = new List<Boat>();
            Animals = new List<Animal>();
            Achievements = new List<Achievement>();
            InitialPopularity = 40;



            var cship = new CommandShip();
            cship.Initialize(new Vector2(0,1000));
            StartingBoats.Add(cship);

        	var oilRig = new OilRig();
			oilRig.Initialize();
			oilRig.SetPossition(new Vector3(0,-10,600));
			GameStatus.RegisterDrawableUnit(oilRig);


            var dolphin = new Dolphin();
            dolphin.Initialize(new Vector2(-200,0));
            Animals.Add(dolphin);

            // TODO require delegate for Earn conditions to be passed in constructor
            var saveDolphin = new Achievement(
                "Save the Dolphin!",
                "Save the dolphin from the Oil",
                10,
                10);

            
            var oil1 = new OilSpill();
            oil1.Initialize(new Vector2(0,-10),140);
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
