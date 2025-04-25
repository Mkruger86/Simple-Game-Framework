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
            // Create a damage calculation strategy
            var damageCalculationStrategy = new DifficultyDamageCalculationStrategy();

            // Configure file logging

            ListenerManager.ConfigureFileLogging(@"C:\Temp\GameLog.txt");
            Trace.AutoFlush = true; // Ensure logs are written immediately

            // Log initialization message
            Logger.Log("Logger initialized successfully.", TraceEventType.Information);

            // Load configuration and initialize the world
            string configFilePath = @"C:\Users\micha\OneDrive\Datamatiker\Opgaver\4. Semester\Advanced Sofware Construction\Simple Game Framework\Simple Game Framework\Simple Game Framework\Worlds\WorldConfig.xml";
            var world = ConfigurationLoader.LoadConfiguration(configFilePath, damageCalculationStrategy);

            // Find the player in the world (if needed)
            var player = world.GetCreaturesAtPosition(new Vector2(1, 1))
                .OfType<Player>()
                .FirstOrDefault();

            if (player == null)
            {
                Console.WriteLine("Player not found in the configuration. Exiting...");
                return;
            }

            // Game loop
            while (true)
            {
                // Render the grid
                world.RenderGrid(player);

                // Allow the player to perform an action
                Console.WriteLine("What would you like to do?");
                player.PerformAction();

                // Check if the player is still alive
                if (player.HitPoints <= 0)
                {
                    Console.WriteLine("Game Over! The player has died.");
                    break;
                }
            }
        }
    }
}

