using System;
using CGCCPlatformer.Helpers;
using CGCCPlatformer.Helpers.ExternalUtils;

namespace CGCCPlatformer
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ExceptionHelper.Initialize();
            Logging.Initialize();
            using (var game = new TheGame())
                game.Run();
        }
    }
}
