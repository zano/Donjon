using System;

namespace Donjon.Entities {
    internal abstract class Creature : Entity {
        public int Health { get; set; }
        public int Damage { get; set; }
        public Creature Target { get; set; }

        public Creature(string name, string symbol, ConsoleColor color, int health, int damage) : base(name, symbol, color) {
            Health = health;
            Damage = damage;
        }

        public string Fight(Creature creature) {
            creature.Target = this;
            creature.Health -= Damage;
            var fataly = "";
            if (creature.Health <= 0) {
                fataly = " fataly";
                creature.RemoveFromCell = true;
            }
            return $"The {Name} attacks the {creature.Name}{fataly} for {Damage} damage";
        }
    }
}