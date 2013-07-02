using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spillville.MainGame.Levels
{
    public class Achievement
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public int ReceivePopularity { get; protected set; }
        public int ReceiveFunds { get; protected set; }

        public bool Awarded { get; set; }

        public delegate Achievement EarnedAchievement();

        public Achievement(string name,string description,int receivePopularity,int receiveFunds)
        {
            Name = name;
            Description = description;
            ReceivePopularity = receivePopularity;
            ReceiveFunds = receiveFunds;
            
        }
    }
}
