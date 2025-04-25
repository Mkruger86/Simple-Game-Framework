using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Creatures
{
    public class Enemy : Creature
    {
        public Enemy(string creatureName, CreatureConfig config, IDamageCalculation damageCalculationStrategy, DifficultyLevel difficultyLevel) : base(creatureName, config, damageCalculationStrategy, difficultyLevel)
        {
        }

        protected override Creature? ChooseTarget()
        {
            throw new NotImplementedException();
        }

        protected override ActionType DecideAction()
        {
            throw new NotImplementedException();
        }

        protected override void Loot()
        {
            throw new NotImplementedException();
        }

        protected override ActionType Move()
        {
            throw new NotImplementedException();
        }

        protected override void Attack(Creature target)
        {
            throw new NotImplementedException();
        }
    }
}
