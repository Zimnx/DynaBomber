using System;

namespace DynaBomber
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (DynaBomber game = new DynaBomber())
            {
                game.Run();
            }
        }
    }
#endif
}

