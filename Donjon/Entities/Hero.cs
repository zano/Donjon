using System;
using System.Collections.Generic;
using Donjon.Entities;
using Lib;

namespace Donjon {
    internal class Hero : Creature {
        public int X { get; set; }
        public int Y { get; set; }
        public LimitedList<Item> Backpack { get; private set; } = new LimitedList<Item>(2);

        public Hero(int health) : base("Hero", "H", ConsoleColor.White, health, 100) {}


        public string PickUp(Item item) {
            if (Backpack.Add(item)) {
                item.RemoveFromCell = true;
                return $"You pick up the {item.Name}.";
            }
            return  $"The backpack is full, so you couldn't pick up the {item.Name}.";
        }

        internal string Inventory() {
            string message = $"Your backpack contains {"items".Count(Backpack.Count)}\n";
            foreach (var item in Backpack) {
                message += "  " + item.Name + "\n";
            }
            return message;
        }
    }
}