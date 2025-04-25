using Simple_Game_Framework.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Items
{
    public class DefenseItem : Item, IObserver
    {
        private readonly Dictionary<DamageType, int> _defenseValues;

        public DefenseItem(string itemName, string description)
            : base(itemName, description)
        {
            _defenseValues = new Dictionary<DamageType, int>();
        }

        public void AddDefense(DamageType type, int value)
        {
            if (_defenseValues.ContainsKey(type))
            {
                _defenseValues[type] += value;
            }
            else
            {
                _defenseValues[type] = value;
            }
        }

        public override Dictionary<DamageType, int> GetDamageValues()
        {
            return new Dictionary<DamageType, int>(); // Defense items don't provide damage
        }

        public override Dictionary<DamageType, int> GetDefenseValues()
        {
            return _defenseValues;
        }

        /// <summary>
        /// Modifies the damage dealt to a target creature based on the defense item's properties.
        /// </summary>
        /// <param name="attacker">
        /// The <see cref="Creature"/> that is attacking.
        /// </param>
        /// <param name="target">
        /// The <see cref="Creature"/> that is being attacked.
        /// </param>
        /// <param name="baseDamage">
        /// The base damage dealt by the attacker before applying any modifications.
        /// </param>
        /// <returns>
        /// The modified damage after applying the defense item's effects.
        /// </returns>
        /// <remarks>
        /// This method is part of the observer pattern implementation. It is called when a creature is attacked and allows the defense item to modify the incoming damage.
        /// The modification is based on the defense values provided by the item for specific damage types.
        /// </remarks>
        public int Update(Creature attacker, Creature target, int baseDamage)
        {
            // Reduce damage based on defense values
            int reducedDamage = baseDamage;

            foreach (var defense in _defenseValues)
            {
                Console.WriteLine($"{ItemName} reduces {defense.Value} {defense.Key} damage.");
                reducedDamage -= defense.Value;
            }

            return Math.Max(0, reducedDamage); // Ensure damage doesn't go below 0
        }
    }
}
