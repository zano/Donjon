using System;
using System.Collections.Generic;
using Donjon.Entities;

namespace Donjon {
    class Symbol {
        public readonly string LightShade = "░";
        public readonly string MediumShade = "▒";
        public readonly string HeadyShade = "▓";
        public readonly string SolidShade = "█";

        public readonly string HalfBlockUpper = "▀";
        public readonly string HalfBlockLower = "▄";
        public readonly string HalfBlockLeft = "▌";
        public readonly string HalfBlockRight = "▐";

        public readonly string TriangleUp = "▲";
        public readonly string TriangleDown = "▼";
        public readonly string TriangleLeft = "◄";
        public readonly string TriangleRight = "►";
    }

    class Cell : IDrawable {
        private readonly string empty = ".";
        private readonly string wall = "#";

        private Item item;
        private Monster monster;

        public int X { get; }
        public int Y { get; }

        public bool IsWall { get; set; }

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

        public string Symbol => Monster?.Symbol ?? Item?.Symbol ?? (IsWall ? wall : empty);
    }
}