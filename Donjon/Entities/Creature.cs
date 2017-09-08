using System;
using Lib;

namespace Donjon.Entities {
    class Creature : Entity {
        public int X { get; set; }
        public int Y { get; set; }

        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public virtual int Attack { get; }
        public bool IsDead => Health <= 0;

        public Creature(string name, string symbol, ConsoleColor color, int health, int attack) : base(name, symbol, color) {
            MaxHealth = Health = health;
            Attack = attack;
        }

        public Result Fight(Monster monster) {
            monster.IsAggressive = true;
            return Fight(monster as Creature);
        }

        public Result Fight(Creature creature) {
            var damage = Randomizer.Dice(Attack);
            creature.Health -= damage;
            var fataly = creature.IsDead ? " fataly" : "";
            return Result.LastAction($"The {Name} attacks the {creature.Name}{fataly} for {damage} damage");
        }

        public override string ToString() {
            return base.ToString() + $" ({Attack / Health})";
        }
    }
}