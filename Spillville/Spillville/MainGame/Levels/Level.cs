using System;
using System.Collections.Generic;
using Spillville.Models.Animals;
using Spillville.Models.Boats;
using Spillville.MainGame.OilSpillContainer;
using Microsoft.Xna.Framework;
namespace Spillville.MainGame.Levels
{
    /*
     * Each level needs:
     * - Win Conditions
     * - Lose Conditions
     * - Amount of Oil
     * - Unit limits
     * - Time limitt
     * - Starting funds
     * - starting units
     * 
     * popularity matters for number of stars attained on that level (1-5) stars
     * so need to associate what scores correlate to number of stars
     */
    abstract public class Level
    {
        public Level() { }

        abstract public string Name { get; }

        abstract public string Description { get; }

        ///protected TimeSpan _timeLimit;
        public TimeSpan TimeLimit { get; protected set; }

        public List<OilSpill> OilPlumes { get; protected set; }

        public int InitialPopularity { get; protected set; }

        abstract public int UnitLimit { get;  }
        
        abstract public int StartingFunds { get; }

        public List<Boat> StartingBoats { get; protected set; }

        public List<Animal> Animals { get; protected set; }

        public List<Achievement> Achievements { get; protected set; }

        public Vector2 EarthLocation { get; protected set; }

        // Initialize all variables
        abstract public void Initialize();
        public abstract bool WinGame { get; }
        public abstract bool LoseGame { get; }
    }
}
