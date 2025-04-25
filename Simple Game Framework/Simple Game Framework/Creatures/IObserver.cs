using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Creatures
{
    public interface IObserver
    {
        int Update(Creature attacker, Creature target, int baseDamage);
    }
}
