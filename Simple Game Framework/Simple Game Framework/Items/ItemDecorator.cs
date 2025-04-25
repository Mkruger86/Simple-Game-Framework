using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Items
{
    public class ItemDecorator : Item
    {
        protected Item _item;

        protected ItemDecorator(Item item) : base(item.ItemName, item.Description)
        {
            _item = item;
        }

        public override Dictionary<DamageType, int> GetDamageValues()
        {
            return _item.GetDamageValues();
        }

        public override Dictionary<DamageType, int> GetDefenseValues()
        {
            return _item.GetDefenseValues();
        }
    }
}
