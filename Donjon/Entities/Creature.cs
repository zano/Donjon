using System;

namespace Donjon.Entities {
    internal abstract class Creature : Entity {
        public int Health { get; set; }
        public virtual int Damage { get; set; }
        public bool IsAggressive { get; set; }

        public override ConsoleColor Color {
            get { return IsAggressive ? ConsoleColor.Red : base.Color; }
            set { base.Color = value; }
        }

        public Creature(string name, string symbol, ConsoleColor color, int health, int damage) : base(name, symbol, color) {
            Health = health;
            Damage = damage;
        }

        public string Fight(Creature creature) {
            creature.IsAggressive = true;
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