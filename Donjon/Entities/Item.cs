using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Donjon.Entities {
    interface IAffecting {
        string Affect(Hero hero); // or creature
    }

    abstract class Item : Entity {
        public Item(string name, string symbol, ConsoleColor color) : base(name, symbol, color) {}
    }

    class Coin : Item {
        public Coin() : base("Coin", "c", ConsoleColor.Yellow) {}
    }

    class Sock : Item {
        public Sock() : base("Old sock", "s", ConsoleColor.Gray) {}
    }

    class Weapon : Item {
        public int Damage { get; }
        public Weapon(int damage = 20) : base("Sword", "!", ConsoleColor.Blue) {
            Damage = damage;
        }
        public override string ToString() => base.ToString() + $" ({Damage} p)";
    }

    class HealingPotion : Item, IAffecting {
        public int Strenght { get; }

        public HealingPotion() : this(50) {}

        public HealingPotion(int strength) : base("Healing potion", "p", ConsoleColor.Magenta) {
            Strenght = strength;
        }

        public string Affect(Hero hero) {
            hero.Health += 50;
            return $"The {hero.Name} feels vigorated. The potion restored {Strenght} hp";
        }
        public override string ToString() => base.ToString() + $" ({Strenght} p)";
    }
}