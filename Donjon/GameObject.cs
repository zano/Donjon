using System;

namespace Donjon
{
    internal abstract class GameObject : IDrawable
    {
        public string Name { get; set; }
        public string Symbol { get; protected set; }
        public ConsoleColor Color { get; set; }
        public bool RemoveFromCell = false;

        public GameObject(string name, string symbol, ConsoleColor color)
        {
            Name = name;
            Symbol = symbol;
            Color = color;
        }

        public abstract string Action(GameObject obj);

        public abstract void RemoveFrom(Cell cell);
    }
}