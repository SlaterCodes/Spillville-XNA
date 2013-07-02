using System;

namespace Spillville
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Spillville game = new Spillville())
            {
                game.Run();
            }
        }
    }
#endif
}

