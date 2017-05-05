using System;

namespace Donjon
{
    internal class Cell : IDrawable
    {
        public Monster Monster { get; set; }

        public ConsoleColor Color {
            get {
                return Monster?.Color ?? ConsoleColor.DarkGray;
            }
            set { }
        }


        public string Symbol
        {
            get
            {
                if (Monster == null) return ".";
                return Monster.Symbol;
            }
        }
    }
}