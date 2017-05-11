using System;
using System.Collections.Generic;

namespace Donjon
{

    internal class Hero : Creature
    {
        public int X { get; set; }
        public int Y { get; set; }
        public LimitedList<Item> Backpack { get; private set; } = new LimitedList<Item>(2);

        public Hero(int health) : base("Hero", "H", ConsoleColor.White, health, 100)
        {
        }

        public override void RemoveFrom(Cell cell)
        {
            
        }

        public override string Action(GameObject obj)
        {
            string message = "";
            if (obj is Creature)
            {
                Creature creature = obj as Creature;
                creature.Health -= Damage;
                message = $"You attack the {creature.Name} for {Damage} damage";
                if (creature.Health <= 0)
                {
                    creature.RemoveFromCell = true;
                    message += $" and killed it!";                   
                }
            }
            else if (obj is Item)
            {
                Item item = obj as Item;
                if (Backpack.Add(item))
                {
                    item.RemoveFromCell = true;
                    message = $"You pick up the {item.Name}.";
                }
                else {
                    message = $"The backpack is full, so you couldn't pick up the {item.Name}.";
                }
            }           

            return message;
        }

        internal string Inventory()
        {
            string message = $"Your backpack contains {Backpack.Count} items\n";
            foreach (var item in Backpack)
            {
                message += "  " + item.Name + "\n";
            }
            return message;
        }
    }
}