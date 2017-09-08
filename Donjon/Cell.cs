using System;
using System.Collections.Generic;
using Donjon.Entities;

namespace Donjon {
    class Symbol {
        public static readonly string LightShade = "░";
        public static readonly string MediumShade = "▒";
        public static readonly string HeavyShade = "▓";
        public static readonly string SolidShade = "█";

        public static readonly string HalfBlockUpper = "▀";
        public static readonly string HalfBlockLower = "▄";
        public static readonly string HalfBlockLeft = "▌";
        public static readonly string HalfBlockRight = "▐";

        public static readonly string UpTriangle = "▲";
        public static readonly string DownTriangle = "▼";
        public static readonly string LeftTriangle = "◄";
        public static readonly string RightTriangle = "►";

        public static readonly string Square = "■";
        public static readonly string EmptySquare = "□";
        public static readonly string SmallSquare = "▪";
        public static readonly string SmallEmptySquare = "▫";
    }

    class CellType {
        public string Symbol { get; private set; }
        public ConsoleColor Color { get; private set; } = ConsoleColor.DarkGray;
        public ConsoleColor Background { get; private set; }
        public bool Obstructing { get; private set; }
        public bool Occluding { get; private set; }

        private CellType(string symbol) {
            Symbol = symbol;
        }

        public static CellType Open = new CellType(".");
        public static CellType StairsAscending = new CellType(Donjon.Symbol.UpTriangle);
        public static CellType StairsDescending = new CellType(Donjon.Symbol.UpTriangle);

        public static CellType Door = new CellType(Donjon.Symbol.Square) {
            Occluding = true
        };

        public static CellType Wall = new CellType("#") {
            Color = ConsoleColor.DarkGray,
            Background = ConsoleColor.DarkGray,
            Obstructing = true,
            Occluding = true
        };

        public static CellType Lava = new CellType(Donjon.Symbol.MediumShade) {
            Color = ConsoleColor.Red,
            Background = ConsoleColor.DarkRed,
            Obstructing = true
        };
    }

    class Cell : IDrawable {
        public int X { get; }
        public int Y { get; }

        public CellType CellType { private get; set; }

        public bool Obstructing => CellType.Obstructing;
        public bool Occluding => CellType.Occluding;

        private Item item;

        public Item Item {
            get { return item; }
            set {
                if (CellType != CellType.Open) return;
                item = value;
            }
        }

        private Monster monster;

        public Monster Monster {
            get { return monster; }
            set {
                if (CellType.Obstructing) return;
                monster = value;
                if (value == null) return;
                monster.X = X;
                monster.Y = Y;
            }
        }

        public ConsoleColor Color => Monster?.Color ?? Item?.Color ?? CellType.Color;
        public ConsoleColor Background => CellType.Background;
        public string Symbol => Monster?.Symbol ?? Item?.Symbol ?? CellType.Symbol;

        public Cell(int x, int y, CellType cellType = null) {
            Y = y;
            X = x;
            CellType = cellType ?? CellType.Open;
        }
    }
}