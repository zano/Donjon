using System;
using System.Linq;
using System.Threading;
using Donjon.Entities;
using Lib;
using R = Lib.Randomizer;

namespace Donjon {
    class Game {
        private readonly Ui ui;
        private bool playing = true;
        private readonly Map map;
        private readonly Log log;
        private readonly Hero hero = new Hero(100);

        public Game(Ui ui) {
            this.ui = ui;
            ui.CancelKeyPress += Quit;
            map = new Map(ui, ui.Width / 2 - 16, ui.Height - 16);
            log = new Log(ui.WriteLine);
        }

        private void Quit(object sender, ConsoleCancelEventArgs e) {
            ui.WriteLine("Ctrl+C");
            playing = false;
            e.Cancel = true;
        }

        public void Play() {
            Initialize();
            while (playing) {
                Draw();
                Thread.Sleep(200);
                var result = PlayerAction();
                log.Add(result.Message?.Split('\n') ?? new string[] { });
                if (playing && result.IsAction) GameAction();
            }
            log.Add("   --- *** Game over *** ---");
            Draw();
        }

        private void Initialize() {
            Populate();
            hero.X = map.Width / 2;
            hero.Y = map.Height / 2;
            map.Hero = hero;
        }

        private void Populate() {
            var mazeConfig = new MazeConfig {
                ConnectLeft = false,
                StraightPassageways = true,
                ConnectCenter = true
            };
            var maze = new Maze(map.Width, map.Height, mazeConfig);
            foreach (var cell in map.Cells) {
                switch (maze.Map[cell.X, cell.Y]) {
                    case CellType.Wall:
                        cell.IsWall = true;
                        break;
                    case CellType.Door:
                        cell.Item = new Item("Door", "D", ConsoleColor.Magenta);
                        break;
                }
            }

            var distributionOfMonsters = new Distribution<Monster> {
                { 2, MonsterFactory.Goblin },
                { 1, MonsterFactory.Troll },
            };
            var distributionOfItems = new Distribution<Item> {
                { 20, ItemFactory.Coin },
                { 20, ItemFactory.Coins },
                { 10, () => new HealthPotion(R.Dice(4) * 20) },
                { 05, () => new TeleportPotion(map) },
                { 02, ItemFactory.Dagger },
                { 01, ItemFactory.Sword },
            };

            foreach (var cell in map.Cells.Where(c => !c.IsWall)) {
                if (R.ChanceD(.01)) cell.Monster = distributionOfMonsters.Pick();
                if (R.ChanceD(.1) && cell.Item == null) cell.Item = distributionOfItems.Pick();
            }
        }

        private void Draw() {
            ui.SetCorner(0, 0);
            map.Draw();
            ui.WriteLine($" Health: {hero.Health:##0} hp");
            ui.WriteLine($" {hero.Wielding?.Name ?? "Fists"}: {hero.Attack:###}");

            var cell = map.Cell(hero.X, hero.Y);
            if (cell.Item != null) ui.WriteLine($" You see {cell.Item}");
            ui.WriteLine("");

            log.Flush();
            ui.WriteLine();

            ui.SetCorner(map.Width * 2 + 1, 0);
            ui.WriteLine(new[] {
                "Arrow keys for movement",
                "I: Inventory",
                "P: Pick up item",
                "D: Drop item",
                "Q: Quit"
            });
        }

        private Result PlayerAction() {
            var key = Console.ReadKey(intercept: true).Key;
            while (Console.KeyAvailable) Console.ReadKey(); // flush input stream
            switch (key) {
                case ConsoleKey.NumPad8:
                case ConsoleKey.UpArrow:
                    return TryMove(0, -1);

                case ConsoleKey.NumPad2:
                case ConsoleKey.DownArrow:
                    return TryMove(0, 1);

                case ConsoleKey.NumPad4:
                case ConsoleKey.LeftArrow:
                    return TryMove(-1, 0);

                case ConsoleKey.NumPad6:
                case ConsoleKey.RightArrow:
                    return TryMove(1, 0);

                case ConsoleKey.Q:
                    playing = false;
                    return Result.NoAction();

                case ConsoleKey.I:
                    return hero.Inventory();

                case ConsoleKey.P:
                    return hero.PickUpItem(map.Cell(hero.X, hero.Y));

                case ConsoleKey.U:
                    return UseItem();

                case ConsoleKey.D:
                    return DropItem();

                default:
                    return Result.NoAction();
            }
        }

        private Result UseItem() {
            return InteractWithItem("What item do you want to use?", "Can't use that", hero.Use);
        }

        private Result DropItem() {
            return InteractWithItem("What item do you want to drop?", "Can't drop that", item => hero.Drop(item, map.Cell(hero.X, hero.Y)));
        }

        private Result InteractWithItem(string question, string failText, Func<Item, Result> action) {
            //log.Add("Backpack contents:");
            log.Add("");
            log.Add(question);
            log.Add($"  {0:#0}: Nothing");
            for (var i = 0; i < hero.Backpack.Count; i++) log.Add($"  {i + 1:#0}: {hero.Backpack[i]}");
            log.Flush();


            var index = ui.AskForInt("> ");
            try {
                var itemToUse = hero.Backpack[index - 1];
                return action(itemToUse);
            } catch {
                return Result.NoAction(failText);
            }
        }


        private Result TryMove(int dx, int y) {
            var targetX = hero.X + dx;
            var targetY = hero.Y + y;

            if (map.IsWall(targetX, targetY)) return Result.NoAction();

            var cell = map.Cell(targetX, targetY);
            if (cell.Monster != null) return hero.Fight(cell);

            hero.X = targetX;
            hero.Y = targetY;

            //return cell.Item == null
            //    ? Result.Action()
            //    : Result.Action($"You see: {cell.Item}");

            return Result.Action();
        }

        private void GameAction() {
            var aggressive = map.Cells.Select(c => c.Monster).Where(m => m != null && m.IsAggressive);
            foreach (var monster in aggressive) {
                if (map.AreAdjacent(hero, monster)) log.Add(monster.Fight(hero).Message);
            }
            if (hero.IsDead) playing = false;
        }
    }
}