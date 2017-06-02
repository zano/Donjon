using System;

namespace Donjon.Entities {
    abstract class Entity : IDrawable {
        public virtual string Name { get; }
        public virtual string Symbol { get; }
        public virtual ConsoleColor Color { get; }

        protected Entity(string name, string symbol, ConsoleColor color) {
            Name = name;
            Symbol = symbol;
            Color = color;
        }

        public override string ToString() {
            return Name;
        }
    }
}