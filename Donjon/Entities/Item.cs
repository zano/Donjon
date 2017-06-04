using System;
using System.Threading;
using Lib.Extensions;
using R = Lib.Randomizer;

namespace Donjon.Entities {
    class Item : Entity {
        public Item(string name, string symbol, ConsoleColor color) : base(name, symbol, color) {}
    }

    interface IStackable {
        int Amount { get; set; }
        void Stack(IStackable otherStack);
        IStackable Remove(int amount);
    }

    static class StackableExtension {
        public static void Stack(this IStackable thisStack, IStackable otherStack) {
            thisStack.Amount += otherStack.Amount;
            otherStack.Amount = 0;
        }      
    }

    class StackableItem : Item, IStackable {
        public int Amount { get; set; }

        public StackableItem(string name, string symbol, ConsoleColor color, int amount = 1) : base(name, symbol, color) {
            Amount = amount;
        }

        public void Stack(IStackable otherStack) => StackableExtension.Stack(this, otherStack);

        public IStackable Remove(int amount) {
            amount = Math.Min(amount, Amount);
            Amount -= amount;
            return new StackableItem(Name, Symbol, Color, amount);
        }

        public override string ToString() => Name.Counted(Amount) ;
    }

    static class ItemFactory {
        public static Weapon Sword() => Sword(R.Dice(4, 2) * 10);
        public static Weapon Sword(int attack) => new Weapon("sword", "/", ConsoleColor.White, attack);

        public static Weapon Dagger() => Dagger(R.Dice(2) * 10);
        public static Weapon Dagger(int attack) => new Weapon("dagger", "†", ConsoleColor.White, attack);

        public static StackableItem Coin() => Coin(1);
        public static StackableItem Coins() => Coin(R.Dice(10,2));
        public static StackableItem Coin(int amount) => new StackableItem("coin", "c", ConsoleColor.Yellow, amount);

        public static Item Corpse(Monster monster)
            => new Item(monster.Name + " corpse", monster.Symbol, ConsoleColor.Gray);
    }

    class Weapon : Item {
        public int Attack { get; set; }

        public Weapon(string name, string symbol, ConsoleColor color, int attack) : base(name, symbol, color) {
            Attack = attack;
        }

        public override string ToString() => base.ToString() + $" ({Attack})";
    }

    class TeleportPotion : Item, IConsumable {
        private readonly Map map;

        public TeleportPotion(Map map) : base("teleport potion", "t", ConsoleColor.Magenta) {
            this.map = map;
        }

        public Result Affect(Hero hero) {
            int x, y;
            do {
                x = R.Next(map.Width);
                y = R.Next(map.Height);
            } while (map.IsWall(x, y) && map.Cell(x, y).Monster == null);
            hero.X = x;
            hero.Y = y;
            return Result.Action("The room spins around you and when it stops you feel relocated");
        }
    }

    class HealthPotion : Item, IConsumable {
        public int Health { get; }

        public HealthPotion(int health) : base("health potion", "h", ConsoleColor.Green) {
            Health = health;
        }

        public Result Affect(Hero hero) {
            var restore = Math.Min(hero.MaxHealth - hero.Health, Health);
            hero.Health += restore;

            return Result.Action($"You feel invigorated. The {Name} restored {restore} health points");
        }

        public override string ToString() => base.ToString() + $" ({Health})";

    }
}