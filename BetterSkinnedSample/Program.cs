using System;

namespace BetterSkinnedSample
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var game = new SkinnedGame())
            {
                game.Run();
            }
        }
    }
}