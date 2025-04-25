using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Items
{
    public class DamageDecorator : ItemDecorator
    {
        private readonly DamageType _type;
        private readonly int _value;

        public DamageDecorator(Item item, DamageType type, int value) : base(item)
        {
            _type = type;
            _value = value;
        }

        public override Dictionary<DamageType, int> GetDamageValues()
        {
            var damageValues = base.GetDamageValues();
            if (damageValues.ContainsKey(_type))
            {
                damageValues[_type] += _value;
            }
            else
            {
                damageValues[_type] = _value;
            }
            return damageValues;
        }
    }
}
