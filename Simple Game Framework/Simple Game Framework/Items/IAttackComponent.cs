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
        int CalculateDamage(); 
        void Add(IAttackComponent component); 
        void Remove(IAttackComponent component); 
        IEnumerable<IAttackComponent> GetChildren(); 
    }
}
