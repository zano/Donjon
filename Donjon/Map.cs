﻿using System;
using System.Collections.Generic;
using System.Linq;
using Donjon.Entities;
using Lib;

namespace Donjon {
    class Map {
        private readonly Ui ui;
        private readonly Cell[,] cells;

        public int Width { get; }
        public int Height { get; }
        public Hero Hero { get; set; }

        public IEnumerable<Cell> Cells => cells.Cast<Cell>();

        public Map(Ui ui, int width, int height) {
            this.ui = ui;
            Width = width;
            Height = height;

            // Instanciate 2D Cell Array
            cells = new Cell[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    cells[x, y] = new Cell(x, y);
                }
            }
        }

        public Cell Cell(int x, int y) {
            try {
                return cells[x, y];
            } catch {
                return null;
            }
        }

        public void Draw() {
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    IDrawable d = cells[x, y];
                    if (x == Hero.X && y == Hero.Y) d = Hero;

                    var bgColor = ConsoleColor.Black;
                    if (cells[x, y].IsWall) bgColor = ConsoleColor.DarkGray;
                    ui.Write(" " + d.Symbol, d.Color, bgColor);
                }
                ui.BackgroundColor = ConsoleColor.Black;
                ui.WriteLine();
            }
        }

        public bool IsWall(int x, int y)
            => x < 0 || y < 0 || x >= Width || y >= Height || cells[x, y].IsWall;

        internal bool AreAdjacent(Creature c1, Creature c2)
            => Math.Abs(c1.X - c2.X) + Math.Abs(c1.Y - c2.Y) == 1;
    }
}