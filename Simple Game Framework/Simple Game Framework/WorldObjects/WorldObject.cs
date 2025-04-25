using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.WorldObjects
{
    public class WorldObject
    {
        public string Name { get; set; }
        public bool Lootable { get; set; }
        public bool Removeable { get; set; }
        public bool IsWalkable { get; set; } 
        public bool BlocksVision { get; set; } // property for obstacles if i choose to add them

        public WorldObject(string name, bool lootable, bool removeable, bool isWalkable = true, bool blocksVision = false)
        {
            Name = name;
            Lootable = lootable;
            Removeable = removeable;
            IsWalkable = isWalkable;
            BlocksVision = blocksVision;
        }
    }
}
