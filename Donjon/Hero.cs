using System;

namespace Donjon
{

    internal class Hero : Creature
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Hero(int health) : base("Hero", "H", ConsoleColor.White, health, 100)
        {          
        }
    }
}