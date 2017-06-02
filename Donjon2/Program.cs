using System;
using System.Text;
using Lib;


namespace Donjon {
    class Program {
        static void Main(string[] args) {
            var ui = new Ui(width: 80, height: 40);
            do {
                var game = new Game(ui);
                game.Play();
            } while (ui.AskForKey("Another game? (y/n)") == ConsoleKey.Y);
        }
    }
}