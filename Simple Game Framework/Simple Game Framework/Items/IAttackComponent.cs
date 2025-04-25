using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Items
{
    public interface IAttackComponent
    {
        int CalculateDamage(); // Calculate the damage of the item or group
        void Add(IAttackComponent component); // Add a child component (only for groups)
        void Remove(IAttackComponent component); // Remove a child component (only for groups)
        IEnumerable<IAttackComponent> GetChildren(); //Get child components(only for groups)
    }
}
