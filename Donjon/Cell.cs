using System;
using OldDonjon.Entities;

namespace OldDonjon {
    internal class Cell : IDrawable {
        public Monster Monster { get; set; }
        public Item Item { get; set; }

        public ConsoleColor Color {
            get { return Monster?.Color ?? Item?.Color ?? ConsoleColor.DarkGray; }
            set { }
        }


        public string Symbol => Monster?.Symbol ?? Item?.Symbol ?? ".";

        public bool IsWall => false;
    }
}