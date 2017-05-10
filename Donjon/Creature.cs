using System;

namespace Donjon
{
    internal abstract class Creature : GameObject
    {
        public int Health { get; set; }
        public int Damage { get; set; }

        public Creature(string name, string symbol, ConsoleColor color, int health, int damage) : base(name, symbol, color)
        {
            Health = health;
            Damage = damage;
        }

        public override string Action(GameObject obj)
        {
            string message = "";
            Creature creature = obj as Creature;
            if (obj is Creature)
            {
                creature.Health -= Damage;
                message = $"The {Name} attacks the {creature.Name} for {Damage} damage.";
            }
            return message;
        }

    }

   
}