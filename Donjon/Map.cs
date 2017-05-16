using System;
using System.Collections.Generic;
using System.Linq;
using Donjon.Entities;
using Lib;

namespace Donjon {
    internal class Map {
        public int Height { get; set; }
        public int Width { get; set; }

        public IEnumerable<Monster> Monsters => cells
            .Cast<Cell>()
            .Where(cell => cell.Monster != null)
            .Select(cell => cell.Monster);

        public IEnumerable<Entity> Items => cells
            .Cast<Cell>()
            .Where(cell => cell.Item != null)
            .Select(cell => cell.Item);

        private readonly Cell[,] cells;
        public IEnumerable<Cell> Cells => cells.Cast<Cell>();

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
                    switch (Rand.Next(3)) {
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
                            cell.Item = new Sword();
                            break;
                    }
                }

                var x = Lib.Rand.Next(Width);
                var y = Lib.Rand.Next(Height);
                cells[4, 7].Monster = new Orc();
                cells[7, 4].Monster = new Goblin();

                cells[5, 9].Item = new Coin();
                cells[9, 5].Item = new Sock();
                cells[9, 9].Item = new Sword();
            }
        }

        internal void Print(Hero hero) {
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

            internal Cell Cell(int x, int y) => cells[x, y];
        }
    }