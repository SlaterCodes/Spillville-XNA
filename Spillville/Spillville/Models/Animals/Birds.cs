using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spillville.Models.Animals
{
    class Birds : Animal
    {
        public Birds()
        {
            this.CleaningTime = TimeSpan.FromSeconds(15);
        }
    }
}
