using System;
using Microsoft.Extensions.Configuration;

namespace BetterSkinnedSample
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            var configurationFileName = "app-settings.json";
            var configuration = new ConfigurationBuilder().AddJsonFile(configurationFileName, true, true).Build();
            using var game = new SkinnedGame(configuration);
            game.Run();
        }
    }
}