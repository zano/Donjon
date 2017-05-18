using System;
using System.Linq;
using Lib;
using Lib.Extensions;

namespace Donjon {
    internal class Game {
        private bool quit = false;

        private readonly Map map;
        private Hero hero;

        private readonly Ui ui = new Ui();
        private readonly Log log;

        private readonly int helpTextWidth = 40;
        private readonly int helpTextLeft;

        public Game(int width, int height) {
            log = new Log(ui.WriteLine);
            helpTextLeft = width * 2 + 1;
            map = new Map(width, height);

            Console.CancelKeyPress += Quit;
            Console.WindowWidth = helpTextLeft + helpTextWidth;
            Console.WindowHeight = height + 20;
        }


        private void Quit(object sender, ConsoleCancelEventArgs consoleCancelEventArgs) {
            quit = true;
            consoleCancelEventArgs.Cancel = true;
        }

        internal void Run() {
            // Initialisera
            // skapa hjälte
            hero = new Hero(health: 100) { Damage = 10 };

            // placera monster
            map.Populate();

            // gör en första utskrift
            log.Add("Welcome to Donjon!");

            while (!quit) {
                // rita spelplan och övrig information
                Draw();

                // hantera indata
                var acted = UserActions();

                if (hero.Health <= 0) {
                    log.Add("The hero is dead... Game Over");
                    quit = true;
                    break;
                }

                // updatera spelobjekt
                if (acted) GameActions();

                if (hero.Health <= 0) {
                    log.Add("The hero is dead... Game Over");
                    quit = true;
                }
            }
            Draw();
            ui.AskForKey("Press a key to quit");
        }

        private void GameActions() {
            map.CleanUp();
            foreach (var monster in map.Monsters.Where(m => m.IsAggressive)) {
                if (map.AreAdjacent(hero, monster)) log.Add(monster.Fight(hero));
            }
            map.CleanUp();
        }

        private void Draw() {
            ui.SetCorner(0, 0);
            map.Draw(hero);
            ui.WriteLine($" Health: {hero.Health:###} hp, Weapon: {hero.Weapon?.Name ?? "Fists"} {hero.Damage:###}");
            ui.WriteLine("");
            log.Flush();


            var left = map.Width * 2 + 1;

            ui.SetCorner(left, 0);
            ui.WriteLine("Arrow keys for movement");
            ui.WriteLine("I: Inventory");
            ui.WriteLine("P: Pick up item");
        }

        private bool UserActions() {
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
                case ConsoleKey.U:
                    return UseItem();
                default:
                    return false;
            }
        }

        private bool UseItem() {
            var c = 0;
            ui.WriteLine(hero.Backpack.Select(i => $"{c++}: {i}"));
            var pos = ui.AskForInt("Select item to use");
            log.Add(hero.Use(hero.Backpack[pos]));
            return true;
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