using Simple_Game_Framework.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Game_Framework.Items
{
    public class AttackItemGroup : IAttackComponent, IObserver
    {
        private readonly List<IAttackComponent> _components = new List<IAttackComponent>();

        public void Add(IAttackComponent component)
        {
            _components.Add(component);
        }

        public void Remove(IAttackComponent component)
        {
            _components.Remove(component);
        }

        public IEnumerable<IAttackComponent> GetChildren()
        {
            return _components;
        }

        public int CalculateDamage()
        {
            // Sum the damage of all child components
            return _components.Sum(component => component.CalculateDamage());
        }

        public int Update(Creature attacker, Creature target, int baseDamage)
        {
            // Notify all child components to modify the damage
            int modifiedDamage = baseDamage;
            foreach (var component in _components)
            {
                if (component is IObserver observer)
                {
                    modifiedDamage = observer.Update(attacker, target, modifiedDamage);
                }
            }
            return modifiedDamage;
        }
    }
}
