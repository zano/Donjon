using System;

namespace Donjon.Entities {
    class Monster : Creature {
        // Color is overriden to return red if aggressive, otherwise use the base class' Color property 
        public override ConsoleColor Color => IsAggressive ? ConsoleColor.Red : base.Color;
        public bool IsAggressive { get; set; }

        public Monster(string name, string symbol, ConsoleColor color, int health, int attack)
            : base(name, symbol, color, health, attack) {}
    }

    static class MonsterFactory {
        public static Monster Troll()
            => new Monster("Troll", "T", ConsoleColor.DarkMagenta, 80, 40);

        public static Monster Goblin()
            => new Monster("Goblin", "G", ConsoleColor.DarkGreen, 40, 10);
    }
}