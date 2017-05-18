using System;
using Donjon.Entities;
using Lib;
using Lib.Extensions;

namespace Donjon {
    internal class Hero : Creature {
        // override Creature's override

        public override int Damage {
            get { return Weapon?.Damage ?? base.Damage; }
            set { base.Damage = value; }
        }

        public override ConsoleColor Color { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public LimitedList<Item> Backpack { get; } = new LimitedList<Item>(6);

        public Hero(int health) : base("Hero", "H", ConsoleColor.White, health, 100) {}

        public string PickUp(Item item) {
            if (Backpack.Add(item)) {
                item.RemoveFromCell = true;
                return $"You pick up the {item.Name}.";
            }
            return $"The backpack is full, so you couldn't pick up the {item.Name}.";
        }

        internal string Inventory() {
            string message = $"Your backpack contains {"items".Count(Backpack.Count)}\n";
            foreach (var item in Backpack) {
                message += $"  {item.Name.A()}\n";
            }
            return message;
        }

        public string Use(Item item) {
            var message = "";
            var log = new Log(m => message += m);

            if (item is Weapon) log.Add(Wield(item as Weapon));

            if (item is IAffecting) {
                log.Add((item as IAffecting).Affect(this));
                var removed = Backpack.Remove(item);
            }

            log.Flush();
            return message != "" 
                ? message 
                : item.Name.A() + " can't be \"used\"";
        }

        public Weapon Weapon { get; private set; }

        private string Wield(Weapon weapon) {
            var limbo = Weapon;
            Backpack.Remove(weapon);
            Weapon = weapon;
            if (limbo != null) Backpack.Add(Weapon); // Todo: Or drop weapon
            return $"{Name} wields the {weapon.Name}";
        }
    }
}