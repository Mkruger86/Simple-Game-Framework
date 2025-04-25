using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Simple_Game_Framework.Creatures;
using Simple_Game_Framework.Items;
using Simple_Game_Framework.Utility.Logging;
using Simple_Game_Framework.WorldObjects;
using Simple_Game_Framework.Worlds;

namespace Simple_Game_Framework.Worlds
{
    public class World
    {
        public int Width { get; }
        public int Height { get; }
        public Graph Graph { get; }
        private Dictionary<Creature, Vector2> CreaturePositions { get; }
        private Dictionary<WorldObject, Vector2> WorldObjectPositions { get; }
        private readonly Dictionary<WorldObject, Item?> _worldObjectItems = new Dictionary<WorldObject, Item?>();

        public World(int width, int height)
        {
            Width = width;
            Height = height;
            Graph = new Graph(width, height);
            CreaturePositions = new Dictionary<Creature, Vector2>();
            WorldObjectPositions = new Dictionary<WorldObject, Vector2>();
        }

        public List<Creature> GetCreaturesAtPosition(Vector2 position)
        {
            return CreaturePositions
                .Where(kvp => kvp.Value == position)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public Creature? GetCreatureInDirection(Vector2 currentPosition, Vector2 direction)
        {
            var targetPosition = currentPosition + direction;
            if (!IsWithinBounds(targetPosition)) return null;

            return GetCreaturesAtPosition(targetPosition).FirstOrDefault();
        }

        public List<WorldObject> GetWorldObjectsAtPosition(Vector2 position)
        {
            return WorldObjectPositions
                .Where(kvp => kvp.Value == position)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        public bool IsWithinBounds(Vector2 position)
        {
            return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
        }

        /// <summary>
        /// Adds a creature to the world at the specified position.
        /// </summary>
        /// <param name="creature">The creature to add.</param>
        /// <param name="position">The position to place the creature.</param>
        /// <remarks>
        /// This method updates the internal dictionary of creature positions and logs the action using <see cref="Logger"/>.
        /// </remarks>
        public void AddCreature(Creature creature, Vector2 position)
        {
            if (IsWithinBounds(position))
            {
                CreaturePositions[creature] = position;
                Logger.LogWorldState($"Creature '{creature.CreatureName}' added at position {position}.");
            }
        }

        public void AddWorldObject(WorldObject worldObject, Vector2 position)
        {
            if (IsWithinBounds(position))
            {
                WorldObjectPositions[worldObject] = position;
                Logger.LogWorldState($"WorldObject '{worldObject.Name}' added at position {position}.");
            }
        }

        public Vector2 GetPosition(Creature creature)
        {
            return CreaturePositions.TryGetValue(creature, out var position) ? position : throw new Exception("Creature not found in world.");
        }

        public Vector2 GetPosition(WorldObject worldObject)
        {
            return WorldObjectPositions.TryGetValue(worldObject, out var position) ? position : throw new Exception("WorldObject not found in world.");
        }

        public bool SetPosition(Creature creature, Vector2 newPosition)
        {
            if (!IsWithinBounds(newPosition))
            {
                Console.WriteLine("The position is out of bounds.");
                return false;
            }

            if (GetCreaturesAtPosition(newPosition).Any())
            {
                Console.WriteLine("The position is already occupied by another creature.");
                return false;
            }

            CreaturePositions[creature] = newPosition;
            return true;
        }

        public void SetPosition(WorldObject worldObject, Vector2 newPosition)
        {
            if (IsWithinBounds(newPosition))
            {
                WorldObjectPositions[worldObject] = newPosition;
            }
            else
            {
                throw new Exception("New position is out of bounds.");
            }
        }

        /// <summary>
        /// Adds a world object to the specified position in the world and optionally associates it with an item.
        /// </summary>
        /// <param name="worldObject">
        /// The <see cref="WorldObject"/> to be added to the world.
        /// </param>
        /// <param name="item">
        /// The optional <see cref="Item"/> to associate with the world object. If <c>null</c>, no item is associated.
        /// </param>
        /// <param name="position">
        /// The <see cref="Vector2"/> position where the world object will be placed.
        /// </param>
        /// <remarks>
        /// This method places the specified world object at the given position in the world. If an item is provided, it is associated with the world object.
        /// The method ensures that the position is within the bounds of the world. If the position is out of bounds, an exception is thrown.
        /// The action is logged using the <see cref="Logger"/> class.
        /// </remarks>
        public void AddWorldObjectWithItem(WorldObject worldObject, Item? item, Vector2 position)
        {
            if (IsWithinBounds(position))
            {
                WorldObjectPositions[worldObject] = position;

                if (item != null)
                {
                    _worldObjectItems[worldObject] = item; 
                    item.SetWorldObject(worldObject); 
                    Logger.LogWorldState($"WorldObject '{worldObject.Name}' placed at {position}.");
                }
                else
                {
                    Logger.LogWorldState($"WorldObject '{worldObject.Name}' placed at {position}.");
                }
            }
            else
            {
                throw new Exception("Position is out of bounds.");
            }
        }

        public Item? GetItemFromWorldObject(WorldObject worldObject)
        {           
            return _worldObjectItems.TryGetValue(worldObject, out var item) ? item : null;
        }

        public void RemoveItemFromWorldObject(WorldObject worldObject)
        {
            if (_worldObjectItems.ContainsKey(worldObject))
            {
                _worldObjectItems[worldObject] = null;
            }
        }

        public void RemoveWorldObject(WorldObject worldObject)
        {
            if (WorldObjectPositions.ContainsKey(worldObject))
            {
                WorldObjectPositions.Remove(worldObject);
                _worldObjectItems.Remove(worldObject);
            }
        }

        public bool IsPositionInRange(Vector2 currentPosition, Vector2 targetPosition, int range)
        {
            return Vector2.Distance(currentPosition, targetPosition) <= range;
        }

        /// <summary>
        /// Renders the world grid to the console, displaying creatures, objects, and walkable areas.
        /// </summary>
        /// <param name="player">
        /// The <see cref="Player"/> to highlight on the grid. The player's position is marked with an "x".
        /// </param>
        /// <remarks>
        /// This method iterates through the grid and renders each cell based on its contents:
        /// <list type="bullet">
        /// <item><description>"x": Represents the player's position.</description></item>
        /// <item><description>"C": Represents a creature's position.</description></item>
        /// <item><description>"i": Represents a world object's position.</description></item>
        /// <item><description>".": Represents a walkable area.</description></item>
        /// <item><description>"#": Represents a non-walkable area.</description></item>
        /// </list>
        /// The method interacts with the <see cref="Graph"/>, <see cref="Creature"/>, and <see cref="WorldObject"/> classes to determine the state of each cell.
        /// </remarks>
        public void RenderGrid(Player player)
        {
            // Top border
            Console.Write("/");
            Console.Write(new string('-', Width * 2 + 1));
            Console.WriteLine("\\");

            // Grid rows
            for (int y = 0; y < Height; y++)
            {
                Console.Write("| ");
                for (int x = 0; x < Width; x++)
                {
                    var position = new Vector2(x, y);

                    if (player != null && GetPosition(player) == position)
                    {
                        Console.Write("x "); 
                    }
                    else if (CreaturePositions.Values.Any(p => p == position))
                    {
                        Console.Write("C "); 
                    }
                    else if (WorldObjectPositions.Values.Any(p => p == position))
                    {
                        Console.Write("i "); 
                    }
                    else
                    {
                        var node = Graph.GetNode(x, y);
                        Console.Write(node.Walkable ? ". " : "# "); 
                    }
                }
                Console.WriteLine("|");
            }

            // Bottom border
            Console.Write("\\");
            Console.Write(new string('-', Width * 2 + 1));
            Console.WriteLine("/");
        }

    }
}






