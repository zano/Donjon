using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donjon
{
    class Item : GameObject
    {
        public Item(string name, string symbol, ConsoleColor color) : base(name, symbol, color)
        {
        }
    }
}
