using System;
using System.Numerics;
using Simple_Game_Framework.Worlds;
using Simple_Game_Framework.WorldObjects;
using Simple_Game_Framework.Items;
using Simple_Game_Framework.Utility.Logging;

namespace Simple_Game_Framework.Creatures
{
    public class Player : Creature
    {
        private readonly World _world; // Reference to the world
        
        public Player(string creatureName, CreatureConfig config, IDamageCalculation damageCalculationStrategy, DifficultyLevel difficultyLevel, World world, Vector2 startPosition)
            : base(creatureName, config, damageCalculationStrategy, difficultyLevel)
        {
            _world = world;
            Position = startPosition;

            // Place the player in the world
            if (_world.IsWithinBounds(Position))
            {
                _world.AddCreature(this, Position);
            }
        }

        /// <summary>
        /// Determines the player's next action based on the current state of the world.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionType"/> representing the player's chosen action.
        /// Possible values are <see cref="ActionType.Move"/>, <see cref="ActionType.Attack"/>, or <see cref="ActionType.Skip"/>.
        /// </returns>
        /// <remarks>
        /// This method evaluates the player's surroundings to determine available actions. 
        /// It checks for lootable objects at the player's current position and enemies in adjacent positions.
        /// Based on the evaluation, the player is prompted to choose an action via console input.
        /// </remarks>
        /// <seealso cref="World.GetWorldObjectsAtPosition(Vector2)"/>
        /// <seealso cref="World.GetCreatureInDirection(Vector2, Vector2)"/>
        /// <param name="canLoot">
        /// A boolean indicating whether there are lootable objects at the player's current position.
        /// </param>
        /// <param name="canAttack">
        /// A boolean indicating whether there are enemies in adjacent positions.
        /// </param>
        /// <param name="directions">
        /// An array of <see cref="Vector2"/> representing the four cardinal directions (up, down, left, right) 
        /// used to check for enemies in adjacent positions.
        /// </param>
        /// <param name="input">
        /// The player's input from the console, indicating their chosen action.
        /// Valid inputs are "m" (move), "a" (attack), and "s" (skip).
        /// </param>
        protected override ActionType DecideAction()
        {
            // Check if there is something to loot
            bool canLoot = _world.GetWorldObjectsAtPosition(Position).Any(obj => obj.Lootable);

            // Check if there is an enemy in range (adjacent nodes)
            bool canAttack = false;
            Vector2[] directions = { new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0) };
            foreach (var direction in directions)
            {
                if (_world.GetCreatureInDirection(Position, direction) != null)
                {
                    canAttack = true;
                    break;
                }
            }

            if (canLoot)
            {
                Console.WriteLine("There is something to loot here.");
            }

            if (canAttack)
            {
                Console.WriteLine("An enemy is in range for attack.");
                Console.WriteLine("Choose an action: (m) Move, (a) Attack, (s) Skip");
            }
            else
            {
                Console.WriteLine("Choose an action: (m) Move, (s) Skip");
            }

            var input = Console.ReadLine()?.Trim().ToLower();

            return input switch
            {
                "m" => ActionType.Move,
                "a" => canAttack ? ActionType.Attack : ActionType.Skip, // Only allow attack if an enemy is in range
                "s" => ActionType.Skip,
                _ => ActionType.Skip
            };
        }

        /// <summary>
        /// Handles the player's movement within the world.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionType"/> indicating the result of the move action.
        /// Returns <see cref="ActionType.Move"/> if the move is successful, or <see cref="ActionType.Skip"/> if the move fails.
        /// </returns>
        /// <remarks>
        /// This method allows the player to move in one of four cardinal directions (up, down, left, right) based on user input.
        /// It validates the new position to ensure it is within bounds and not occupied by another creature.
        /// If the move is successful, the player's position is updated, and any lootable objects at the new position are automatically looted.
        /// </remarks>
        /// <seealso cref="World.SetPosition(Creature, Vector2)"/>
        /// <seealso cref="Loot"/>
        protected override ActionType Move()
        {
            Console.WriteLine("Choose a direction to move: (w) Up, (a) Left, (s) Down, (d) Right");
            var input = Console.ReadLine()?.Trim().ToLower();

            Vector2 newPosition = Position;

            switch (input)
            {
                case "w":
                    newPosition += new Vector2(0, -1); // Move up
                    break;
                case "a":
                    newPosition += new Vector2(-1, 0); // Move left
                    break;
                case "s":
                    newPosition += new Vector2(0, 1); // Move down
                    break;
                case "d":
                    newPosition += new Vector2(1, 0); // Move right
                    break;
                default:
                    Console.WriteLine("Invalid input. Skipping move.");
                    return ActionType.Skip;
            }

            // Attempt to move to the new position
            if (_world.SetPosition(this, newPosition))
            {
                Position = newPosition; // Update the player's local position
                Logger.LogPlayerAction(CreatureName, "Move", $"Moved to position {Position}.");
                Console.WriteLine($"{CreatureName} moved to {Position}.");

                // Automatically loot if there is a lootable object at the new position
                var lootableObject = _world.GetWorldObjectsAtPosition(Position).FirstOrDefault(obj => obj.Lootable);
                if (lootableObject != null)
                {
                    Console.WriteLine("You moved over a lootable object. Looting...");
                    Loot(); // Call the Loot method to automatically loot the object
                }

                return ActionType.Move;
            }
            else
            {
                Console.WriteLine("Cannot move to the specified position.");
                return ActionType.Skip;
            }
        }
        /// <summary>
        /// Handles the looting of items from lootable objects at the player's current position.
        /// </summary>
        /// <remarks>
        /// This method checks for lootable objects at the player's current position in the world.
        /// If a lootable object is found, it retrieves the associated item, adds it to the player's inventory, 
        /// and removes the item from the world. If the lootable object is removable, it is also removed from the world.
        /// </remarks>
        /// <seealso cref="World.GetWorldObjectsAtPosition(Vector2)"/>
        /// <seealso cref="World.GetItemFromWorldObject(WorldObject)"/>
        /// <seealso cref="World.RemoveItemFromWorldObject(WorldObject)"/>
        /// <seealso cref="World.RemoveWorldObject(WorldObject)"/>
        /// <seealso cref="IAttackComponent"/>
        /// <seealso cref="DefenseItem"/>
        /// <seealso cref="Logger.LogPlayerAction(string, string, string)"/>
        /// <param name="lootableObject">
        /// A reference to the first lootable object found at the player's current position. 
        /// This is retrieved from the world using the player's position.
        /// </param>
        /// <param name="item">
        /// The item associated with the lootable object, if any. 
        /// This is retrieved from the world and added to the player's inventory if it exists.
        /// </param>
        /// <param name="attackItem">
        /// A cast of the looted item to <see cref="IAttackComponent"/> if it is an attack item.
        /// This is added to the player's attack inventory.
        /// </param>
        /// <param name="defenseItem">
        /// A cast of the looted item to <see cref="DefenseItem"/> if it is a defense item.
        /// This is added to the player's defense inventory.
        /// </param>
        protected override void Loot()
        {
            // Get the lootable object at the player's position
            var lootableObject = _world.GetWorldObjectsAtPosition(Position).FirstOrDefault(obj => obj.Lootable);

            if (lootableObject != null)
            {
                // Retrieve the item associated with the lootable object
                var item = _world.GetItemFromWorldObject(lootableObject);

                if (item != null)
                {
                    Logger.LogPlayerAction(CreatureName, "Loot", $"Looted item '{item.ItemName}' from '{lootableObject.Name}'.");
                    // Add the item to the player's inventory
                    if (item is IAttackComponent attackItem)
                    {
                        AttackItems.Add(attackItem);
                        Console.WriteLine($"{CreatureName} looted an attack item: {item.ItemName}.");
                    }
                    else if (item is DefenseItem defenseItem)
                    {
                        DefenseItems.Add(defenseItem);
                        Console.WriteLine($"{CreatureName} looted a defense item: {item.ItemName}.");
                    }

                    // Remove the item from the world
                    _world.RemoveItemFromWorldObject(lootableObject);
                }
                else
                {
                    // Only print this message if no item was ever associated with the lootable object
                    Console.WriteLine("The lootable object does not contain any items.");
                }

                // Remove the world object if it is removable
                if (lootableObject.Removeable)
                {
                    _world.RemoveWorldObject(lootableObject);
                    Console.WriteLine($"{lootableObject.Name} has been removed from the world.");
                }
            }
            else
            {
                Console.WriteLine("There is nothing to loot here.");
            }
        }

        /// <summary>
        /// Allows the player to choose a target for an attack by selecting a direction.
        /// </summary>
        /// <returns>
        /// A <see cref="Creature"/> object representing the target in the chosen direction, or <c>null</c> if no valid target is found.
        /// </returns>
        /// <remarks>
        /// This method prompts the player to input a direction (up, down, left, or right) to attack. 
        /// It calculates the target position based on the player's current position and the chosen direction.
        /// If a creature exists at the target position, it is returned as the target. Otherwise, the method returns <c>null</c>.
        /// </remarks>
        /// <seealso cref="World.GetCreatureInDirection(Vector2, Vector2)"/>
        /// <param name="input">
        /// The player's input indicating the direction of the attack. 
        /// Valid inputs are "w" (up), "a" (left), "s" (down), and "d" (right).
        /// </param>
        /// <param name="attackDirection">
        /// A <see cref="Vector2"/> representing the direction of the attack based on the player's input.
        /// This is used to calculate the target position.
        /// </param>
        /// <returns>
        /// A <see cref="Creature"/> at the target position if one exists, or <c>null</c> if no creature is found.
        /// </returns>
        protected override Creature? ChooseTarget()
        {
            Console.WriteLine("Choose a direction to attack: (w) Up, (a) Left, (s) Down, (d) Right");
            var input = Console.ReadLine()?.Trim().ToLower();

            Vector2 attackDirection = Vector2.Zero;

            switch (input)
            {
                case "w":
                    attackDirection = new Vector2(0, -1); // Attack up
                    break;
                case "a":
                    attackDirection = new Vector2(-1, 0); // Attack left
                    break;
                case "s":
                    attackDirection = new Vector2(0, 1); // Attack down
                    break;
                case "d":
                    attackDirection = new Vector2(1, 0); // Attack right
                    break;
                default:
                    Console.WriteLine("Invalid input. Skipping attack.");
                    return null;
            }

            // Check if there is a creature at the attack position
            return _world.GetCreatureInDirection(Position, attackDirection);
        }

        /// <summary>
        /// Executes an attack on a specified target creature.
        /// </summary>
        /// <param name="target">
        /// The <see cref="Creature"/> to be attacked. If the target is <c>null</c>, the method exits without performing an attack.
        /// </param>
        /// <remarks>
        /// This method calculates the base damage dealt by the player using the <see cref="CalculateDamage"/> method.
        /// It then notifies all attached observers (e.g., attack items) to modify the damage using the <see cref="NotifyObservers"/> method.
        /// The total damage is applied to the target using the <see cref="Creature.ReceiveHit(int)"/> method.
        /// The attack action is logged using the <see cref="Logger.LogPlayerAction(string, string, string)"/> method.
        /// </remarks>
        protected override void Attack(Creature target)
        {
            if (target == null)
            {
                Console.WriteLine("No valid target to attack.");
                return;
            }

            // Calculate base damage
            int baseDamage = CalculateDamage();

            // Notify observers (e.g., attack items) to modify the damage
            int totalDamage = NotifyObservers(target, baseDamage);

            // Apply damage to the target
            target.ReceiveHit(totalDamage);

            Logger.LogPlayerAction(CreatureName, "Attack", $"Attacked {target.CreatureName} for {totalDamage} damage.");

            Console.WriteLine($"{CreatureName} attacked {target.CreatureName} for {totalDamage} damage!");
        }
    }
}



