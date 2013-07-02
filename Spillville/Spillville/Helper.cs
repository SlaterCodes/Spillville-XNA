using System;
using Microsoft.Xna.Framework;

namespace Spillville
{
    class Helper
    {
        public static float Distance(Vector3 possition1, Vector3 possition2)
        {
            float a = (possition2.X - possition1.X);
            float b = (possition2.Z - possition1.Z);

            return (float)Math.Sqrt(a * a + b * b);
        }

    }
}
