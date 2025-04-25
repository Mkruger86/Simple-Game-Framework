using Simple_Game_Framework.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Creatures
{
    public interface IDamageCalculation
    {
        int CalculateDamage(Dictionary<DamageType, int> baseDamageValues, DifficultyLevel difficultyLevel);
    }
}
