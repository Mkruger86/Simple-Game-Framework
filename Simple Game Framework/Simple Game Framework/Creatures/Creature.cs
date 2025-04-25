using Simple_Game_Framework.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


//A Creature should have (state) and be able to (behaviour):

//- Have a name = CName
//- Have hit points = HitPoints
//- Have a list of current attack items and defense items in inventory = attackItems & defenseItems
//- Loot a defense or attack item = Loot()
//- Attack another creature = Hit()
//- Have base damage as a creature without weapon = basedamage
// -I want the creature to be able to move in the world = Move()
//- Receive damage = ReceiveHit()
//- Notify observers when it receives damage
//- Attach and detach observers
//- Use the Strategy pattern to calculate damage

namespace Simple_Game_Framework.Creatures
{
    public abstract class Creature : ISubject
    {
        private readonly List<IObserver> _observers = new List<IObserver>();
        private readonly Dictionary<DamageType, int> _baseDamageValues = new Dictionary<DamageType, int>();
        private readonly IDamageCalculation _damageCalculationStrategy;
        private readonly DifficultyLevel _difficultyLevel;

        public string CreatureName { get; }
        public int BaseHealth { get; }
        public int HitPoints { get; set; }
        public int AttackPower { get; private set; }
        public int DefensePower { get; private set; }
        public Vector2 Position { get; set; }
        public CreatureType creatureType { get; set; } // Enum for creature type (e.g., Player, Monster, etc.)
        public List<IAttackComponent> AttackItems { get; private set; }
        public List<DefenseItem> DefenseItems { get; private set; }

        protected Creature(string creatureName, CreatureConfig config, IDamageCalculation damageCalculationStrategy, DifficultyLevel difficultyLevel)
        {
            CreatureName = creatureName;
            HitPoints = config.BaseHealth;
            _baseDamageValues = config.BaseDamageValues;
            _damageCalculationStrategy = damageCalculationStrategy;
            _difficultyLevel = difficultyLevel; // Set difficulty level dynamically
            AttackItems = new List<IAttackComponent>();
            DefenseItems = new List<DefenseItem>();
        }

        public void AttachObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void DetachObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public int NotifyObservers(Creature target, int baseDamage)
        {
            int modifiedDamage = baseDamage;

            foreach (var observer in _observers)
            {
                modifiedDamage = observer.Update(this, target, modifiedDamage);
            }

            return modifiedDamage;
        }

        public void Hit(Creature target)
        {
            int baseDamage = CalculateDamage();
            int finalDamage = NotifyObservers(target, baseDamage); // Notify observers to modify damage
            target.ReceiveHit(finalDamage); // Apply the final damage
        }

        /// <summary>
        /// Processes incoming damage to the creature, applying defense mitigation and reducing hit points.
        /// </summary>
        /// <param name="damage">
        /// The total damage dealt to the creature before applying defense mitigation.
        /// </param>
        /// <remarks>
        /// This method calculates the total defense provided by all equipped defense items and mitigates the incoming damage accordingly.
        /// The mitigated damage is subtracted from the creature's current hit points. If the mitigated damage is less than or equal to zero, no damage is applied.
        /// The method outputs the damage received, the defense applied, and the remaining hit points to the console.
        /// </remarks>
        /// <seealso cref="DefenseItem.GetDefenseValues"/>
        /// <seealso cref="HitPoints"/>
        public void ReceiveHit(int damage)
        {
            int totalDefense = 0;
            foreach (var defenseItem in DefenseItems)
            {
                var defenseValues = defenseItem.GetDefenseValues();
                foreach (var defenseValue in defenseValues.Values)
                {
                    totalDefense += defenseValue;
                }
            }

            int mitigatedDamage = Math.Max(0, damage - totalDefense);

            HitPoints -= mitigatedDamage;
            Console.WriteLine($"{CreatureName} received {mitigatedDamage} damage after applying {totalDefense} defense. Remaining HP: {HitPoints}");
        }

        /// <summary>
        /// Executes the creature's next action based on its decision-making logic.
        /// </summary>
        /// <remarks>
        /// This method determines the creature's next action by calling the abstract <see cref="DecideAction"/> method.
        /// Based on the returned <see cref="ActionType"/>, it performs one of the following actions:
        /// <list type="bullet">
        /// <item><description><see cref="ActionType.Move"/>: Moves the creature to a new position by calling the abstract <see cref="Move"/> method.</description></item>
        /// <item><description><see cref="ActionType.Loot"/>: Loots an item by calling the abstract <see cref="Loot"/> method.</description></item>
        /// <item><description><see cref="ActionType.Attack"/>: Attacks a target creature by calling the abstract <see cref="ChooseTarget"/> and <see cref="Attack(Creature)"/> methods.</description></item>
        /// <item><description><see cref="ActionType.Skip"/>: Skips the creature's turn.</description></item>
        /// </list>
        /// </remarks>
        public void PerformAction()
        {
            var action = DecideAction();
            switch (action)
            {
                case ActionType.Move:
                    Move();            
                    break;
                case ActionType.Loot:
                    Loot();
                    break;
                case ActionType.Attack:
                    var target = ChooseTarget(); // Choose a target for attack
                    if (target != null)
                    {
                        Attack(target); // Perform attack action
                    }
                    else
                    {
                        Console.WriteLine("No valid target to attack.");
                    }
                    break;
                case ActionType.Skip:
                    Console.WriteLine($"{CreatureName} skipped their turn.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract void Attack(Creature target);
        protected abstract ActionType DecideAction();
        protected abstract ActionType Move();
        protected abstract Creature? ChooseTarget();
        protected abstract void Loot();
        protected virtual int CalculateDamage()
        {
            // Start with the base damage calculated by the strategy
            int baseDamage = _damageCalculationStrategy.CalculateDamage(_baseDamageValues, _difficultyLevel);

            // Add the damage from all equipped attack items
            foreach (var attackItem in AttackItems)
            {
                baseDamage += attackItem.CalculateDamage();
            }
            return baseDamage;
        }

        protected virtual void NotifyAction(ActionType action)
        {
            Console.WriteLine($"{CreatureName} performed action: {action}.");
        }
   
    }
}
