using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donjon {
    class Program {
        static void Main(string[] args) {
            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.Unicode;
    
            Game game = new Game(width: 15, height: 10);
            game.Run();
        }
    }
}