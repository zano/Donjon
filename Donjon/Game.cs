using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using Lib;

namespace Donjon {
    internal class Game {
        private bool quit = false;

        private readonly Map map;
        private Hero hero;
        private readonly Log log = new Log();
        private readonly int helpTextWidth = 20;

        public Game(int width, int height) {
            map = new Map(width, height);

            Console.CancelKeyPress += Quit;
            Console.WindowWidth = width * 2 + helpTextWidth + 3;
            Console.WindowHeight = height + 20;
        }

        private void Quit(object sender, ConsoleCancelEventArgs consoleCancelEventArgs) {
            quit = true;
            consoleCancelEventArgs.Cancel = true;
        }

        internal void Run() {
            // Initialisera
            // skapa hjälte
            hero = new Hero(health: 100) {
                Damage = 10
            };

            // placera monster
            map.Populate();

            // gör en första utskrift

            while (!quit) {
                // rita spelplan och övrig information
                Draw();

                // hantera indata
                var acted = UserInput();

                // updatera spelobjekt
                if (acted) Update();
            }
            Draw();
        }

        private void Update() {           
            foreach (var monster in map.Monsters) {
                if (monster.Target == hero) {
                    // Todo: and is adjacent to hero
                    log.Add(monster.Fight(hero));
                }
            }

            map.CleanUp();
        }

        private void Draw() {
            Console.SetCursorPosition(0, 0);
            Ui.WriteLine($"Health: {hero.Health:###} hp, Attack: {hero.Damage:###}");
            map.Print(hero);
            log.Flush();

            var left = map.Width * 2 + 1;
            Ui.SetCursorPosition(left, 0);
            Ui.WriteLine("Arrow keys for movement");
            Ui.WriteLine("I: Inventory");
            Ui.WriteLine("P: Pick up item");
        }

        private bool UserInput() {
            var key = Console.ReadKey(intercept: true).Key;
            switch (key) {
                case ConsoleKey.UpArrow:
                    return TryMove(0, -1);
                case ConsoleKey.DownArrow:
                    return TryMove(0, 1);
                case ConsoleKey.LeftArrow:
                    return TryMove(-1, 0);
                case ConsoleKey.RightArrow:
                    return TryMove(1, 0);
                case ConsoleKey.Q:
                    quit = true;
                    return false;
                case ConsoleKey.I:
                    log.Add(hero.Inventory());
                    return false;
                case ConsoleKey.P:
                    var item = map.Cell(hero.X, hero.Y).Item;
                    if (item == null) return false;
                    log.Add(hero.PickUp(item));
                    return true;
                default:
                    return false;
            }
        }

        private bool TryMove(int x, int y) {
            var newX = hero.X + x;
            var newY = hero.Y + y;
            
            if (OutOfBonds(newX, newY) || map.Cell(newX, newY).IsWall) {
                // invalid action
                return false;
            }

            var cell = map.Cell(newX, newY);
            if (cell.Monster == null) {
                hero.X = newX;
                hero.Y = newY;
                if (cell.Item != null) log.Add($"You find {cell.Item.Name.A()}");
            } else {
                log.Add(hero.Fight(cell.Monster));
            }
            return true;
        }

        private bool OutOfBonds(int newX, int newY) => newX < 0 || newX >= map.Width ||
                                                       newY < 0 || newY >= map.Height;
    }
}