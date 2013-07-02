using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spillville.Models.ParticlesSystem
{
    public static class ParticlesSystemsCollection
    {
        public static List<ParticlesSystem> ParticlesSystems { get; set; }

        public static void Initialize()
        {
            ParticlesSystems = new List<ParticlesSystem>();
        }
        
    }
}
