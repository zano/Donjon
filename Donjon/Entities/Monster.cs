using System;

namespace OldDonjon.Entities {
    internal abstract class Monster : Creature {
        public Monster(string name, string symbol, ConsoleColor color, int health, int damage)
            : base(name, symbol, color, health, damage) {}
    }

    class Orc : Monster {
        public Orc() : base("Orc", "O", ConsoleColor.Red, health: 40, damage: 20) {}
    }

    class Goblin : Monster {
        public Goblin() : base("Goblin", "G", ConsoleColor.DarkYellow, health: 20, damage: 10) {}
    }
}