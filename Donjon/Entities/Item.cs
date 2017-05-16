using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donjon.Entities {
    abstract class Item : Entity {
        public Item(string name, string symbol, ConsoleColor color) : base(name, symbol, color) {}
    }

    class Coin : Item {
        public Coin() : base("Coin", "c", ConsoleColor.Yellow) {}
    }

    class Sock : Item {
        public Sock() : base("Old sock", "s", ConsoleColor.Gray) {}
    }

    class Sword : Item {
        public Sword() : base("Sword", "!", ConsoleColor.Blue) {}
    }
}