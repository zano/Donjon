using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Donjon.Entities;
using Lib;
using Maze;
using R = Lib.Randomizer;

namespace Donjon {
    class Game {
        private readonly Ui ui;
        private bool playing = true;
        private int actionPoints;
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
            Draw();

            while (playing) {
                actionPoints = 2;
                Draw();
                do
                {
                    var result = PlayerAction();
                    log.Add(result.Message?.Split('\n') ?? new string[] { });
                    if (result.IsAction) actionPoints--;
                    if (result.EndsTurn) actionPoints = 0;
                    Draw();
                } while (playing && actionPoints > 0);

                if (playing) GameAction();
                Draw();
                //Thread.Sleep(1500);
            }
            log.Add("   --- === Game over === ---");
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
                StraightPassageways = false,
                ConnectCenter = true
            };
            var maze = MazeBuilder.Generate(map.Width, map.Height, mazeConfig);
            foreach (var cell in map.Cells) {
                switch (maze.Map[cell.X, cell.Y]) {
                    case MazeBuilder.CellType.Wall:
                        cell.CellType = CellType.Wall;
                        break;
                    case MazeBuilder.CellType.Door:
                        cell.CellType = CellType.Door;
//                      cell.Item = new Item("Door", "D", ConsoleColor.Magenta);
                        break;
                    case MazeBuilder.CellType.StairsUp:
                        cell.CellType = CellType.StairsAscending;
                        break;
                    case MazeBuilder.CellType.StairsDown:
                        cell.CellType = CellType.StairsDescending;
                        break;
                    case MazeBuilder.CellType.Open:
                        cell.CellType = CellType.Open;
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

            var cells = map.Cells.Where(c => !c.Obstructing);
            
            Queue<Item> items = null;
       
            foreach (var cell in cells) {         
                if (items == null || items.Count  <= 0) items = distributionOfItems.RandomQueue();

                if (R.Chance(.02) && cell.Monster == null) cell.Monster = distributionOfMonsters.PickRandom();
                if (R.Chance(.02) && cell.Item == null) cell.Item = /*distributionOfItems.PickRandom();*/ items.Dequeue();

            }
        }

        private void Draw() {
            ui.SetCorner(0, 0);
            map.Draw();
            ui.WriteLine($" {actionPoints:0} ap");
            ui.WriteLine($" {hero.Health:##0} hp");
            ui.WriteLine($" {hero.Wielding?.Name ?? "Fists"}: {hero.Attack:###}");

            ui.WriteLine();

            log.Write();
            ui.WriteLine();

            ui.SetCorner(map.Width * 2 + 1, 0);
            ui.WriteLine(new[] {
                "Arrow keys for movement",
                "I: Inventory",
                "P: Pick up item",
                "D: Drop item",
                "Q: Quit"
            });

            var cell = map.Cell(hero.X, hero.Y);
            if (cell.Item != null) ui.WriteLine($" You see {cell.Item}");
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


        private Result TryMove(int dx, int dy) {
            var targetX = hero.X + dx;
            var targetY = hero.Y + dy;

            if (map.IsWall(targetX, targetY)) return Result.NoAction();

            var cell = map.Cell(targetX, targetY);
            if (cell.Monster != null) return hero.Fight(cell);

            hero.X = targetX;
            hero.Y = targetY;

            return Result.Action();
        }

        private void GameAction() {
            var aggressive = map.Cells.Select(c => c.Monster).Where(m => m != null && m.IsAggressive).ToList();
            foreach (var monster in aggressive) {
                if (map.AreAdjacent(hero, monster)) {
                    log.Add(monster.Fight(hero).Message);
                } else {
                    if (map.Distance(hero, monster) < 6) TryMoveTo(monster, hero.X, hero.Y);
                }
            }
            if (hero.IsDead) playing = false;
        }

        public bool TryMoveTo(Monster monster, int x, int y) {
            var dx = x - monster.X;
            var dy = y - monster.Y;
            bool moved;
            if (dx > dy) {
                moved = TryMove(monster, dx, 0) || TryMove(monster, 0, dy) || TryMove(monster, 0, -dy) || TryMove(monster, -dx, 0);
            } else {
                moved = TryMove(monster, 0, dy) || TryMove(monster, dx, 0) || TryMove(monster, -dx, 0) || TryMove(monster, 0, -dy);
            }
            return moved;
        }

        private bool TryMove(Monster monster, int dx, int dy) {
            dx = Math.Sign(dx);
            dy = Math.Sign(dy);

            var targetX = monster.X + dx;
            var targetY = monster.Y + dy;

            if (map.IsWall(targetX, targetY)) return false;

            var cell = map.Cell(targetX, targetY);
            if (cell.Monster != null) return false;

            map.Cell(monster.X, monster.Y).Monster = null;
            map.Cell(targetX, targetY).Monster = monster;
            return true;
        }
    }
}