using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spillville.MainGame.Levels
{
    public static class Tutorials
    {
        public static bool TutorialCreateBoatHelped;
        public static bool TutorialCleanOilHelped;
        public static bool TutorialMoveBoatHelped;
        public static bool TutorialEmptyTankHelped;
        public static bool TutorialCreateTankerHelped;
        public static bool TutorialUpgradeBoatsHelped;
        public static bool TutorialSaveDolphinHelped;

        public static void Initialize()
        {
            TutorialCreateBoatHelped = false;
            TutorialCleanOilHelped = false;
            TutorialMoveBoatHelped = false;
            TutorialEmptyTankHelped = false;
            TutorialCreateTankerHelped = false;
            TutorialUpgradeBoatsHelped = false;
            TutorialSaveDolphinHelped = false;
        }


            

    }
}
