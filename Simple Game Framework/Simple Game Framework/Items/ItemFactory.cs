using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Items
{
    public class ItemFactory
    {
        private readonly Dictionary<string, Func<Item>> _itemCreators;

        public ItemFactory()
        {
            // Initialize item creators
            _itemCreators = new Dictionary<string, Func<Item>>
            {
                { "Sword", () => new AttackItem(15, "Sword", "A sharp blade.") },
                { "Axe", () => new AttackItem(20, "Axe", "A heavy axe with immense power.") },
                { "Bow", () => new AttackItem(10, "Bow", "A long-range bow.") },
                { "Shield", () => new DefenseItem("Shield", "A sturdy shield.") },
                { "Armor", () => new DefenseItem("Armor", "Heavy armor for maximum protection.") }
            };
        }

        public Item CreateItem(string itemName)
        {
            if (_itemCreators.TryGetValue(itemName, out var creator))
            {
                return creator(); // Create the item on demand
            }

            throw new ArgumentException($"Item with name '{itemName}' not found in the factory.");
        }
    }
}
