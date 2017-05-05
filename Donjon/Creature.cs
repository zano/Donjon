using System;

namespace Donjon
{
    internal abstract class Creature : GameObject
    {
        public Creature(string name, string symbol, ConsoleColor color, int health, int damage) : base(name, symbol, color)
        {
            Health = health;
            Damage = damage;
        }

    

        public int Health { get; set; }
        public int Damage { get; set; }
    }

   
}