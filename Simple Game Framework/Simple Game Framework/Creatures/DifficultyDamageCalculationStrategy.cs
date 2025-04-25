using Simple_Game_Framework.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Creatures
{
    public class DifficultyDamageCalculationStrategy : IDamageCalculation
    {
        public int CalculateDamage(Dictionary<DamageType, int> baseDamageValues, DifficultyLevel difficultyLevel)
        {
            // Defining multipliers for each difficulty level that i use for general configloader (world) 
            int multiplier = difficultyLevel switch
            {
                DifficultyLevel.Easy => 1,
                DifficultyLevel.Normal => 2,
                DifficultyLevel.Hard => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(difficultyLevel), "Invalid difficulty level")
            };

            // Calculate total damage with the multiplier
            return baseDamageValues.Values.Sum() * multiplier;
        }
    }
}
