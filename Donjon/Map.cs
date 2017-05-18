using System;
using System.Collections.Generic;
using System.Linq;
using Donjon.Entities;
using Lib;
using Lib.Extensions;

namespace Donjon {
    internal class Map {
        public int Height { get; private set; }
        public int Width { get; private set; }

        public IEnumerable<Monster> Monsters => cells
            .Cast<Cell>()
            .Where(cell => cell.Monster != null)
            .Select(cell => cell.Monster);

        public IEnumerable<Entity> Items => cells
            .Cast<Cell>()
            .Where(cell => cell.Item != null)
            .Select(cell => cell.Item);

        public Cell Cell(int x, int y) {
            try {
                return cells[x, y];
            } catch {
                return null;
            }
            ;
        }

        public IEnumerable<Cell> Cells => cells.Cast<Cell>();

        private readonly Cell[,] cells;

        public Map(int width, int height) {
            Width = width;
            Height = height;

            // instansiera 2d-Cellvektorn
            cells = new Cell[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    cells[x, y] = new Cell();
                }
            }
        }

        internal void Populate() {
            foreach (var cell in cells) {
                if (Rand.Chance(10)) {
                    switch (Rand.Next(3)) {
                        case 0:
                            cell.Monster = new Orc();
                            break;
                        default:
                            cell.Monster = new Goblin();
                            break;
                    }
                }
                if (Rand.Chance(20)) {
                    switch (Rand.Next(7)) {
                        case 0:
                        case 1:
                            cell.Item = new Sock();
                            break;
                        case 2:
                        case 3:
                        case 4:
                            cell.Item = new Coin();
                            break;
                        case 5:
                            cell.Item = new Weapon();
                            break;
                        case 6:
                            cell.Item = new HealingPotion();
                            break;
                    }
                }                
            }
        }

        List<Tuple<int, int>> edges = new List<Tuple<int, int>> {
            { -1, 0 },
            { 1, 0 },
            { 0, -1 },
            { 0, 1 }
        };
        internal bool AreAdjacent(Hero hero, Creature creature) 
            => edges.Where(e => creature == Cell(hero.X + e.Item1, hero.Y + e.Item2)?.Monster).Any();

        internal void Draw(Hero hero) {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    IDrawable d = cells[x, y];
                    if (x == hero.X && y == hero.Y) d = hero;

                    Console.ForegroundColor = d.Color;
                    Console.Write(" " + d.Symbol);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        internal void CleanUp() {
            foreach (var cell in cells) {
                if (cell.Monster?.RemoveFromCell == true) cell.Monster = null;
                if (cell.Item?.RemoveFromCell == true) cell.Item = null;
            }
        }
    }
}