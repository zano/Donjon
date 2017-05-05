using System;

namespace Donjon
{
    internal abstract class GameObject : IDrawable
    {
        public string Name { get; set; }
        public string Symbol { get; protected set; }
        public ConsoleColor Color { get; set; }

        public GameObject(string name, string symbol, ConsoleColor color)
        {
            Name = name;
            Symbol = symbol;
            Color = color;
        }
    }
}