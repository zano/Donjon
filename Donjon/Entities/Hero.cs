using System;
using System.Linq;
using Lib;
using Lib.Extensions;

namespace Donjon.Entities {
    class Hero : Creature {
        public Weapon Wielding { get; set; }
        public LimitedList<Item> Backpack { get; } = new LimitedList<Item>(8);

        // Override Attack to take weapon into account 
        public override int Attack => base.Attack + (Wielding?.Attack ?? 0);

        public Hero(int health) : base("Hero", "@", ConsoleColor.Cyan, health, attack: 20) {}

        public Result Fight(Cell cell) {
            var monster = cell.Monster;
            var result = Fight(monster);
            if (monster.IsDead) {
                cell.Monster = null;
                if (cell.Item == null) cell.Item = ItemFactory.Corpse(monster);
            }
            return result;
        }

        public Result PickUpItem(Cell cell) {
            var item = cell.Item;
            if (item == null) return Result.NoAction("There is nothing to pick up here");

            if (Wielding == null && item is Weapon) {
                cell.Item = null;
                return Wield((Weapon)item);
            }

            var stack = item as IStackable;
            if (stack != null) {
                var bpStack = (IStackable)Backpack.FirstOrDefault(i => i.GetType() == stack.GetType() && i.Name == item.Name);
                if (bpStack != null) {
                    bpStack.Stack(stack);
                    cell.Item = null;
                    return Result.Action($"You pick up the {item.Name}.");
                }
            }

            if (Backpack.IsFull) return Result.Action($"The backpack is full, so you couldn't pick up the {item.Name}.");

            Backpack.Add(item);
            cell.Item = null;

            return Result.Action($"You pick up the {item.Name}.");
        }

        public Result Drop(Item item, Cell cell) {
            if (cell.Item != null) return Result.NoAction($"You can't drop the {item.Name}, there's already stuff here");

            cell.Item = item;
            if (Backpack.Contains(item)) Backpack.Remove(item);
            if (Wielding == item) Wielding = null;

            return Result.Action($"You dropped the {item.Name}");
        }

        public Result Inventory() {
            string message = $"Your backpack contains {"item".Counted(Backpack.Count)}\n";
            foreach (var item in Backpack) {
                message += $"  {item}\n";
            }
            return Result.NoAction(message);
        }

        public Result Use(Item item) {
            if (item is Weapon) return WieldFromBackpack(item as Weapon);

            if (item is IConsumable) {
                return Consume((IConsumable)item);
            }
            return Result.NoAction($"You can't use {item}");
        }

        private Result Consume(IConsumable consumable) {
            Backpack.Remove(consumable as Item);
            return consumable.Affect(this);
        }

        private Result WieldFromBackpack(Weapon weapon) {
            var unwield = Wielding;
            Backpack.Remove(weapon);
            if (unwield != null) Backpack.Add(unwield);
            return Wield(weapon);
        }

        private Result Wield(Weapon weapon) {
            Wielding = weapon;
            return Result.Action($"You wield the {weapon.Name}");
        }
    }
}