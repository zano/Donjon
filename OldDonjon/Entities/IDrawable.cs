using System;

namespace OldDonjon.Entities {
    internal interface IDrawable {
        ConsoleColor Color { get; set; }
        string Symbol { get; }
    }
}