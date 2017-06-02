using System;
using System.Text;

namespace OldDonjon {
    class Program {
        static void Main(string[] args) {
            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.Unicode;
    
            Game game = new Game(width: 15, height: 10);
            game.Run();
        }
    }
}