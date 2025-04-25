using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Simple_Game_Framework.Creatures;
using Simple_Game_Framework.Items;
using Simple_Game_Framework.WorldObjects;
using Simple_Game_Framework.Worlds;

namespace Simple_Game_Framework
{
    /// <summary>
    /// loads and parses world configuration from an XML file.
    /// </summary>
    /// <remarks>
    /// This class is responsible for initializing the game world, including grid size, difficulty level, creatures, and world objects.
    /// It interacts with various game components such as <see cref="World"/>, <see cref="Player"/>, <see cref="Enemy"/>, and <see cref="WorldObject"/>.
    /// I've put additional internal remarks throughout this class aswell, as copilot provided considerable semantic and structural assistance for setup here.
    /// </remarks>
    public static class ConfigurationLoader
    {
        /// <summary>
        /// Loads the world configuration from an XML file and initializes the game world.
        /// </summary>
        /// <param name="filePath">The path to the configuration XML file.</param>
        /// <param name="damageCalculationStrategy">The strategy used for calculating damage based on difficulty.</param>
        /// <returns>A fully configured <see cref="World"/> instance.</returns>
        /// <remarks>
        /// This method parses the XML file to configure the grid size, difficulty level, creatures, and world objects.
        /// It interacts with the <see cref="World"/>, <see cref="Player"/>, <see cref="Enemy"/>, and <see cref="WorldObject"/> classes.
        /// </remarks>
        public static World LoadConfiguration(string filePath, DifficultyDamageCalculationStrategy damageCalculationStrategy)
        {
            // Load the XML file
            var document = XDocument.Load(filePath);

            // Parse the grid size
            var gridSizeElement = document.Element("WorldConfiguration")?.Element("GridSize");
            if (gridSizeElement == null) throw new Exception("Grid size not specified in configuration.");
            var width = int.Parse(gridSizeElement.Attribute("width")?.Value ?? throw new Exception("Grid width not specified."));
            var height = int.Parse(gridSizeElement.Attribute("height")?.Value ?? throw new Exception("Grid height not specified."));

            // Initialize the world with the specified grid size
            var world = new World(width, height);

            // Parse the difficulty level
            var difficultyElement = document.Element("WorldConfiguration")?.Element("Difficulty");
            if (difficultyElement == null) throw new Exception("Difficulty level not specified in configuration.");
            var difficultyLevel = Enum.Parse<DifficultyLevel>(difficultyElement.Value, true);

            // Parse creature placement
            var creaturePlacementElement = document.Element("WorldConfiguration")?.Element("CreaturePlacement");
            if (creaturePlacementElement == null) throw new Exception("Creature placement not specified in configuration.");
            var randomCreaturePlacement = bool.Parse(creaturePlacementElement.Attribute("random")?.Value ?? "false");

            Player? player = null;

            // Parse creatures
            var creaturesElement = creaturePlacementElement.Element("Creatures");
            if (creaturesElement != null)
            {
                foreach (var creatureElement in creaturesElement.Elements("Creature"))
                {
                    var typeAttribute = creatureElement.Attribute("type");
                    if (typeAttribute == null) throw new Exception("Creature type not specified.");

                    var creatureType = Enum.Parse<CreatureType>(typeAttribute.Value, true);

                    Vector2 position;
                    var isPlayer = bool.Parse(creatureElement.Attribute("isPlayer")?.Value ?? "false");

                    if (isPlayer)
                    {
                        // Player's position is always set to (1, 1)
                        position = new Vector2(1, 1);

                        if (player != null)
                        {
                            throw new Exception("Multiple players found in the configuration. Only one player is allowed.");
                        }

                        var playerConfig = new CreatureConfig
                        {
                            CreatureType = creatureType,
                            BaseHealth = 100, // Default base health
                            BaseDamageValues = new Dictionary<DamageType, int> { { DamageType.Physical, 10 } }
                        };

                        player = new Player(
                            "Hero", // You can also add a name attribute in the XML if needed
                            playerConfig,
                            damageCalculationStrategy,
                            difficultyLevel,
                            world,
                            position
                        );

                    }
                    else
                    {
                        // Enemy creatures
                        if (randomCreaturePlacement)
                        {
                            // Place the enemy randomly
                            position = GetRandomPosition(world);
                        }
                        else
                        {
                            // Use the specified position
                            var positionElement = creatureElement.Element("Position");
                            if (positionElement == null)
                            {
                                throw new Exception($"Position is missing for creature of type '{creatureType}' when random placement is disabled.");
                            }
                            position = ParsePositionFromElement(positionElement);
                        }

                        var enemyConfig = new CreatureConfig
                        {
                            CreatureType = creatureType,
                            BaseHealth = 100, // Default base health
                            BaseDamageValues = new Dictionary<DamageType, int> { { DamageType.Physical, 10 } }
                        };

                        var enemy = new Enemy(
                            creatureType.ToString(),
                            enemyConfig,
                            damageCalculationStrategy,
                            difficultyLevel
                        );

                        world.AddCreature(enemy, position);
                    }
                }
            }

            // Parse world objects
            var worldObjectsElement = document.Element("WorldConfiguration")?.Element("WorldObjects");
            if (worldObjectsElement == null) throw new Exception("WorldObjects section not specified in configuration.");
            var randomObjectPlacement = bool.Parse(worldObjectsElement.Attribute("random")?.Value ?? "false");

            foreach (var worldObjectElement in worldObjectsElement.Elements("WorldObject"))
            {
                var nameAttribute = worldObjectElement.Attribute("name");
                if (nameAttribute == null) throw new Exception("WorldObject name not specified.");

                Vector2 position;
                if (randomObjectPlacement)
                {
                    // Place the world object randomly
                    position = GetRandomPosition(world);
                }
                else
                {
                    // Use the specified position
                    var positionElement = worldObjectElement.Element("Position");
                    if (positionElement == null)
                    {
                        throw new Exception($"Position is missing for WorldObject '{nameAttribute.Value}' when random placement is disabled.");
                    }
                    position = ParsePositionFromElement(positionElement);
                }

                var worldObject = new WorldObject(nameAttribute.Value, lootable: true, removeable: true);

                // Parse item
                Item? item = null;
                var itemElement = worldObjectElement.Element("Item");
                if (itemElement != null)
                {
                    var itemType = itemElement.Attribute("type")?.Value;
                    var itemName = itemElement.Attribute("name")?.Value;
                    var itemDescription = itemElement.Attribute("description")?.Value;

                    if (itemType == "AttackItem")
                    {
                        var attackItem = new AttackItem(5, itemName, itemDescription);
                        foreach (var damageElement in itemElement.Elements("Damage"))
                        {
                            var damageType = Enum.Parse<DamageType>(damageElement.Attribute("type")?.Value, true);
                            var damageValue = int.Parse(damageElement.Attribute("value")?.Value ?? "0");
                            attackItem.AddDamage(damageType, damageValue);
                        }
                        item = attackItem;
                    }
                    else if (itemType == "DefenseItem")
                    {
                        var defenseItem = new DefenseItem(itemName, itemDescription);
                        foreach (var defenseElement in itemElement.Elements("Defense"))
                        {
                            var defenseType = Enum.Parse<DamageType>(defenseElement.Attribute("type")?.Value, true);
                            var defenseValue = int.Parse(defenseElement.Attribute("value")?.Value ?? "0");
                            defenseItem.AddDefense(defenseType, defenseValue);
                        }
                        item = defenseItem;
                    }
                }

                world.AddWorldObjectWithItem(worldObject, item, position);
            }

            if (player == null)
            {
                throw new Exception("Player not found in the configuration.");
            }

            return world;
        }

        private static Vector2 GetRandomPosition(World world)
        {
            var random = new Random();
            Vector2 position;
            do
            {
                position = new Vector2(random.Next(0, world.Width), random.Next(0, world.Height));
            } while (!world.IsWithinBounds(position) || world.GetCreaturesAtPosition(position).Any() || world.GetWorldObjectsAtPosition(position).Any());
            return position;
        }

        private static Vector2 ParsePositionFromElement(XElement positionElement)
        {
            if (positionElement == null) throw new Exception("Position element is missing.");
            var x = int.Parse(positionElement.Attribute("x")?.Value ?? throw new Exception("Position 'x' attribute is missing."));
            var y = int.Parse(positionElement.Attribute("y")?.Value ?? throw new Exception("Position 'y' attribute is missing."));
            return new Vector2(x, y);
        }
    }
}






