using Simple_Game_Framework.Creatures;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Items
{
    // Attack item should have:
    // - Range
    // - Added Damage
    // - Name
    // - Calculate Damage
    public class AttackItem : Item, IAttackComponent, IObserver
    {

        private readonly Dictionary<DamageType, int> _damageValues;

        public int Range { get; private set; }

        public AttackItem(int range, string itemName, string description)
            : base(itemName, description)
        {
            Range = range;
            _damageValues = new Dictionary<DamageType, int>();
        }

        public void AddDamage(DamageType type, int value)
        {
            if (_damageValues.ContainsKey(type))
            {
                _damageValues[type] += value;
            }
            else
            {
                _damageValues[type] = value;
            }
        }

        public override Dictionary<DamageType, int> GetDamageValues()
        {
            return _damageValues;
        }

        public override Dictionary<DamageType, int> GetDefenseValues()
        {
            return new Dictionary<DamageType, int>(); // Attack items don't provide defense
        }

        public int CalculateDamage()
        {
            return _damageValues.Values.Sum(); // Sum all damage values
        }

        public void Add(IAttackComponent component)
        {
            throw new NotSupportedException("Cannot add to a leaf component.");
        }

        public void Remove(IAttackComponent component)
        {
            throw new NotSupportedException("Cannot remove from a leaf component.");
        }

        public IEnumerable<IAttackComponent> GetChildren()
        {
            return Enumerable.Empty<IAttackComponent>(); // No children for a leaf
        }

        public int Update(Creature attacker, Creature target, int baseDamage)
        {
            Console.WriteLine($"{ItemName} adds {CalculateDamage()} damage.");
            return baseDamage + CalculateDamage();
        }
    }
}
