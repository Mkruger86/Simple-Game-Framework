using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Items
{
    public class DefenseDecorator : ItemDecorator
    {
        private readonly DamageType _type;
        private readonly int _value;

        public DefenseDecorator(Item item, DamageType type, int value) : base(item)
        {
            _type = type;
            _value = value;
        }

        public override Dictionary<DamageType, int> GetDefenseValues()
        {
            var defenseValues = base.GetDefenseValues();
            if (defenseValues.ContainsKey(_type))
            {
                defenseValues[_type] += _value;
            }
            else
            {
                defenseValues[_type] = _value;
            }
            return defenseValues;
        }
    }
}
