using Simple_Game_Framework.WorldObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Items
{
    public abstract class Item
    {
        public string ItemName { get; private set; }
        public string Description { get; private set; }
        public WorldObject? WorldObject { get; private set; } // Reference to the world object
        protected Item(string itemName, string description)
        {
            ItemName = itemName;
            Description = description;
            WorldObject = null; //default is no association, hence simple association
        }

        public void SetWorldObject(WorldObject worldObject)
        {
            WorldObject = worldObject;
        }

        public abstract Dictionary<DamageType, int> GetDamageValues();
        public abstract Dictionary<DamageType, int> GetDefenseValues();
    }
}
