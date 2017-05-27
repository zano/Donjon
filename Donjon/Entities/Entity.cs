using System;

namespace Donjon.Entities {
    internal abstract class Entity : IDrawable {
        public string Name { get; set; }
        public virtual string Symbol { get; protected set; }
        public virtual ConsoleColor Color { get; set; }
        public bool RemoveFromCell = false;

        public Entity(string name, string symbol, ConsoleColor color) {
            Name = name;
            Symbol = symbol; // Virtual member call in constructor. Beware!
            Color = color; // Virtual member call in constructor. Beware!
        }

        public override string ToString() {
            return Name;
        }
    }
}