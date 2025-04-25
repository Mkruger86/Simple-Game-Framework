using Simple_Game_Framework.Items;

namespace Simple_Game_Framework
{
    class Program
    {
        static void Main(string[] args)
        {

            var fireandicesord = new AttackItem(5, "Fire and Ice Sword", "A sword that deals fire and ice damage.");
            fireandicesord.AddDamage(DamageType.Fire, 10);
            // Add 10 fire damage
            fireandicesord.AddDamage(DamageType.Fire, 10);

            // Add 5 ice damage
            fireandicesord.AddDamage(DamageType.Ice, 10);

            // Display the item's details
            Console.WriteLine($"Item: {fireandicesord.ItemName}");
            Console.WriteLine($"Description: {fireandicesord.Description}");
            Console.WriteLine("Damage Values:");
            foreach (var damage in fireandicesord.GetDamageValues())
            {
                Console.WriteLine($"- {damage.Key}: {damage.Value}");
            }

            // Calculate total damage


        }
    }
}
