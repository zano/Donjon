using System.Collections.Generic;
using System.Linq;
using Lib;

namespace Maze.DungeonBuilder {
    internal static class Extensions {
        public static Vector PickRandom(this Rect rect)
            => new Vector(Randomizer.Dice(rect.Right, rect.X),
                          Randomizer.Dice(rect.Bottom, rect.Y));

        public static T PickRandom<T>(this List<T> list)
            => list.Any() ? list[Randomizer.Next(list.Count)] : default(T);
    }
}