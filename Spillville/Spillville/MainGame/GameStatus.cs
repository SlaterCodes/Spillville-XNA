using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Spillville.MainGame.HUD;
using Spillville.MainGame.Levels;
using Spillville.Models;
using Spillville.Models.Boats;
using Spillville.Models.Animals;
using Spillville.Utilities;
using Spillville.MainGame.OilSpillContainer;

namespace Spillville.MainGame
{
    public static class GameStatus
    {
        public static TimeSpan TimeElapsed { get; private set; }
        public static TimeSpan TimeStart { get; private set; }
        public static int AvailableFunds { get; private set; }
        public static int Populatity { get; private set; }
        public static int TotalUnits { get; private set; }
        public static bool LevelRunning;
        //public static ThreadPoolComponent ThreadPool { get; private set; }

        public static int MaxUnits
        {
            get
            {
                return CurrentLevel != null ? CurrentLevel.UnitLimit : 0;
            }
        }

        public static TimeSpan TimeLimit
        {
            get
            {
                return CurrentLevel != null ? CurrentLevel.TimeLimit : TimeSpan.Zero;
            }
        }

        public static List<IDrawableModel> DrawList;
        public static List<IDrawableModel> AnimatedDrawList;
        public static bool LevelStarted { get; private set; }
        public static Level CurrentLevel { get; private set; }

        //say 1-100, we start at 30

        public static void Initialize()
        {
            DrawList = new List<IDrawableModel>();
            AnimatedDrawList = new List<IDrawableModel>();
            TimeElapsed = TimeSpan.Zero;
            AvailableFunds = 10000;
            Populatity = 60;
            TotalUnits = 0;
        }

        // TODO finish
        public static void LoadLevel(Level level)
        {
            Reset();
            level.Initialize();
            CurrentLevel = level;
            TimeElapsed = TimeSpan.Zero;
            LevelStarted = false;
            LevelRunning = false;
            TotalUnits = level.StartingBoats.Count;
            AvailableFunds = level.StartingFunds;
            Populatity = level.InitialPopularity;

            //foreach (Boat b in level.StartingBoats)
            for(var i=0;i<level.StartingBoats.Count;i++)
            {
                RegisterDrawableUnit(level.StartingBoats[i]);
            }
            
            //foreach (Animal a in level.Animals)
            for(var i=0;i<level.Animals.Count;i++)
            {
                var a = level.Animals[i];
                if(a.EntryTime.Equals(TimeSpan.Zero))
                {
                    if (a.IsAnimated)
                        AnimatedDrawList.Add(a);
                    else
                        RegisterDrawableUnit(a);
                    a.Deployed = true;
                }
            }
            //foreach (var oil in level.OilPlumes)
            for(var i=0;i<level.OilPlumes.Count;i++)
            {
                var oil = level.OilPlumes[i];
                if(oil.EntryTime.Equals(TimeSpan.Zero))
                {
                    OilSpillManager.RegisterOilSpill(oil);
                    oil.Deployed = true;
                }
            }
        }

        public static void StartLevel(GameTime gt)
        {
            TimeStart = gt.TotalGameTime;
            LevelStarted = true;
            LevelRunning = true;
        }

        public static void EndLevel()
        {
            LevelRunning = false;
            LevelStarted = false;
        }

        private static TimeSpan _lastGoodBadBulletin;

        public static void Update(GameTime gameTime)
        {
            if (LevelRunning)
            {
                TimeElapsed = TimeElapsed.Add(gameTime.ElapsedGameTime);
                var secs = ((int)TimeElapsed.TotalSeconds);
                if((int)_lastGoodBadBulletin.TotalSeconds != secs && secs % 30 == 0)
                {
                    if(Populatity < 50)
                        BulletinContainer.CallBulletin("Bad");
                    else
                        BulletinContainer.CallBulletin("Good");
                    _lastGoodBadBulletin = TimeSpan.FromSeconds((int)TimeElapsed.TotalSeconds);
                }

                if (CurrentLevel != null)
                    CheckDeployments(gameTime);

                for (int i = 0; i < DrawList.Count; i++)
                {
                    DrawList[i].Update(gameTime);
                }
                for (int i = 0; i < AnimatedDrawList.Count; i++)
                {
                    AnimatedDrawList[i].Update(gameTime);
                }
            }
        }

        private static void CheckDeployments(GameTime gameTime)
        {
            for (var i = 0; i < CurrentLevel.OilPlumes.Count; i++)
            {
                var coil = CurrentLevel.OilPlumes[i];
                if (!coil.Deployed && coil.EntryTime.TotalSeconds >= TimeElapsed.TotalSeconds)
                {
                    OilSpillManager.RegisterOilSpill(coil);
                    coil.Deployed = true;
                }
            }
            for (var i = 0; i < CurrentLevel.Animals.Count; i++)
            {
                var canimal = CurrentLevel.Animals[i];
                if (!canimal.Deployed && canimal.EntryTime.TotalSeconds <= TimeElapsed.TotalSeconds)
                {
                    if (canimal.IsAnimated)
                        AnimatedDrawList.Add(canimal);
                    else
                        RegisterDrawableUnit(canimal);
                    canimal.Deployed = true;
                }
            }
        }

        public static bool RemoveAnimal(Animal a)
        {
            if (a.IsAnimated && AnimatedDrawList.Contains(a))
            {
                AnimatedDrawList.Remove(a);
                return true;
            }
            else
            {
                if (!a.IsAnimated && DrawList.Contains(a))
                {
                    DrawList.Remove(a);
                    return true;
                }
            }
            return false;
        }

        public static void RegisterDrawableUnit(IDrawableModel m)
        {
            DrawList.Add(m);
        	var notifyOnRegister = m as INotifyOnRegister;
        	if (notifyOnRegister != null)
        	{
        		notifyOnRegister.Registered();
        	}
        }

        public static bool UnRegisterDrawableUnit(IDrawableModel m)
        {
			var notifyOnRegister = m as INotifyOnRegister;
			if (notifyOnRegister != null)
        	{
				notifyOnRegister.UnRegistered();
        	}
        	return DrawList.Remove(m);  	
        }

        public static Boolean BuildUnits(int amount)
        {
            if (CanBuildUnits(amount))
            {
                TotalUnits += amount;
                return true;
            }
            return false;
        }

        public static bool CanBuildUnits(int amount)
        {
            return (TotalUnits + amount <= MaxUnits);
        }

        public static void RemoveUnits(int amount)
        {
            TotalUnits = Math.Max(0, TotalUnits - amount);
        }

        public static bool CanSpendMoney(int fund)
        {
            return (AvailableFunds - fund) >= 0;
        }

        public static Boolean SpendMoney(int fund)
        {
            if (CanSpendMoney(fund))
            {
                AvailableFunds -= fund;
#if DEBUG
                System.Diagnostics.Debug.WriteLine("just spent ${0}", fund);
#endif
                return true;
            }
            return false;
        }

        public static void ReceiveFund(int fund)
        {
            AvailableFunds += fund;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("just received ${0}", fund);
#endif
        }

        public static void DecreasePopulatity(int rate)
        {
            Populatity -= rate;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("popularity decreased by {0}", rate);
#endif
        }

        public static void IncreasePopularity(int rate)
        {
            Populatity += rate;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("popularity increased by {0}", rate);
#endif
        }

        public static bool WinGame
        {
            get { return CurrentLevel != null && LevelStarted && CurrentLevel.WinGame; }
        }

        public static bool LoseGame
        {
            get { return CurrentLevel != null && LevelStarted && CurrentLevel.LoseGame; }
        }

        public static TimeSpan TimeRemaining
        {
            //return TimeLimit - TimeRemaining;
            get { return TimeLimit - TimeElapsed; }
        }

        public static void Reset()
        {
            CurrentLevel = null;
            TotalUnits = 0;
            DrawList.Clear();
            AnimatedDrawList.Clear();
            TimeElapsed = TimeSpan.Zero;
        }
    }
}
