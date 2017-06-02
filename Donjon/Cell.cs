using System;
using Donjon.Entities;

namespace Donjon {
    class Cell : IDrawable {
        private static readonly string symbol = ".";

        private Item item;
        private Monster monster;

        public int X { get; }
        public int Y { get; }

        public bool IsWall { get; }

        public Item Item {
            get { return item; }
            set {
                if (IsWall) return;
                item = value;
            }
        }

        public Monster Monster {
            get { return monster; }
            set {
                if (IsWall) return;
                monster = value;
                if (value == null) return;
                monster.X = X;
                monster.Y = Y;
            }
        }

        public Cell(int x, int y, bool isWall = false) {
            Y = y;
            X = x;
            IsWall = isWall;
        }

        public ConsoleColor Color
            => Monster?.Color
               ?? Item?.Color
               ?? ConsoleColor.DarkGray;

        public string Symbol
            => Monster?.Symbol
               ?? Item?.Symbol
               ?? symbol;
    }
}