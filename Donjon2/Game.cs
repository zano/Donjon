using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Donjon.Entities;
using Lib;
using Lib.Extensions;
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
            map = new Map(ui, ui.Width / 4, ui.Height / 2);
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
                var result = PlayerAction();
                log.Add(result.Message);
                if (playing && result.IsAction) GameAction();
            }
            Draw();
        }

        private void Initialize() {
            Populate();
            map.Hero = hero;
        }

        private void Populate() {
            var distributionOfMonsters = new Distribution<Monster> {
                { 2, MonsterFactory.Goblin },
                { 1, MonsterFactory.Troll },
            };
            var distributionOfItems = new Distribution<Item>() {
                { 01, ItemFactory.Sword },
                { 10, ItemFactory.Coin },
                { 05, () => new HealthPotion(R.Dice(4) * 20) },
                { 03, () => new TeleportPotion(map) },
            };
            foreach (var cell in map.Cells) {
                if (R.ChanceD(.01)) cell.Monster = distributionOfMonsters.Pick();
                if (R.ChanceD(.1)) cell.Item = distributionOfItems.Pick();
            }
        }

        private void Draw() {
            ui.SetCorner(0, 0);
            map.Draw();
            ui.WriteLine($" Health: {hero.Health:###} hp");
            ui.WriteLine($" {hero.Wielding?.Name ?? "Fists"}: {hero.Attack:###}");

            var cell = map.Cell(hero.X, hero.Y);
            ui.WriteLine($" You see {cell.Item:10#}");
            ui.WriteLine("");

            log.Flush();
            ui.WriteLine("------");
            ui.WriteLine();

            ui.SetCorner(ui.Width / 2 + 1, 0);
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
        }
    }
}