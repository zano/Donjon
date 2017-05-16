using System;
using Donjon.Entities;

namespace Donjon
{
    internal abstract class Entity : IDrawable
    {
        public string Name { get; set; }
        public string Symbol { get; protected set; }
        public ConsoleColor Color { get; set; }
        public bool RemoveFromCell = false;

        public Entity(string name, string symbol, ConsoleColor color)
        {
            Name = name;
            Symbol = symbol;
            Color = color;
        }
    }
}