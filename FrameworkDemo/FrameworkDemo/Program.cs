using System;
using Simple_Game_Framework.Worlds;
using Simple_Game_Framework.Creatures;
using Simple_Game_Framework;
using System.Numerics;
using Simple_Game_Framework.Utility.Logging;
using System.Diagnostics;

namespace FrameworkDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var damageCalculationStrategy = new DifficultyDamageCalculationStrategy();

            // config file logging
            ListenerManager.ConfigureFileLogging(@"C:\Temp\GameLog.txt");
            Trace.AutoFlush = true; // Ensure logs are written immediately

            // Init logger
            Logger.Log("Logger initialized successfully.", TraceEventType.Information);
          
            string configFilePath = @"C:\Users\micha\OneDrive\Datamatiker\Opgaver\4. Semester\Advanced Sofware Construction\Simple Game Framework\Simple Game Framework\Simple Game Framework\Worlds\WorldConfig.xml";
            var world = ConfigurationLoader.LoadConfiguration(configFilePath, damageCalculationStrategy);

            var player = world.GetCreaturesAtPosition(new Vector2(1, 1))
                .OfType<Player>()
                .FirstOrDefault();

            if (player == null)
            {
                Console.WriteLine("Player not found in the configuration. Exiting...");
                return;
            }

            // Game loop, with rendering and player performaction.
            // Grid is not dynamic for now, rather it renders the same grid over and over again for each action, which is not very efficient, but requires rather large update.
            while (true)
            {
                world.RenderGrid(player);

                Console.WriteLine("What would you like to do?");
                player.PerformAction();

                if (player.HitPoints <= 0)
                {
                    Console.WriteLine("Game Over! The player has died.");
                    break;
                }
            }
        }
    }
}

