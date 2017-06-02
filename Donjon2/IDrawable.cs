using System;

namespace Donjon {
    interface IDrawable {
        ConsoleColor Color { get; }
        string Symbol { get; }
    }
}