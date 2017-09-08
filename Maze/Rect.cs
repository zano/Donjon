using System;
using System.Collections;
using System.Collections.Generic;
using Maze.DungeonBuilder;

namespace Maze {
    public class Rect : IEnumerable<Vector> {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Right => X + Width - 1;
        public int Bottom => Y + Height - 1;

        public int OuterRight => X + Width;
        public int OuterBottom => Y + Height;

        public Rect(int x, int y, int width, int height) {
            X = width > 0 ? x : x + Width;
            Y = height > 0 ? y : Y + Height;
            Width = Math.Abs(width);
            Height = Math.Abs(height);
        }

        public static Rect FromCorners(int x0, int y0, int x1, int y1) => new Rect(
            x: Math.Min(x0, x1),
            y: Math.Min(y0, y1),
            width: Math.Abs(x1 - x0) + 1,
            height: Math.Abs(y1 - y0) + 1
        );

        public Rect Shrink(int amount = 1) {
            if (Width > 2 * amount) {
                Width -= 2 * amount;
                X += amount;
            }
            if (Height > 2 * amount) {
                Height -= 2 * amount;
                Y += amount;
            }
            return this;
        }

        // Dart Project

        public Rect Deflate(int amount = 1)
            => amount >= 0
                ? new Rect(X, Y, Width, Height).Shrink(amount)
                : Inflate(-amount);

        public Rect Inflate(int amount = 1)
            => amount >= 0
                ? new Rect(X - amount, Y - amount, Width + amount * 2, Height + amount * 2)
                : Deflate(-amount);

        /// <summary>
        /// Returns the distance between this Rect and [other]. This is minimum
        /// length that a corridor would have to be to go from one Rect to the other.
        /// If the two Rects are adjacent, returns zero. If they overlap, returns -1.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int DistanceTo(Rect other) {
            if (Overlaps(other)) return -1;
            
            var d = ComponentDistancesTo(other);
            
            if (d.Y == -1 && d.X == -1) return -1;
            if (d.Y == -1) return d.X;
            if (d.X == -1) return d.Y;

            return d.X + d.Y;
        }

        private Vector ComponentDistancesTo(Rect other) {
            var dY = -1;
            if (Y >= other.OuterBottom) {
                dY = Y - other.OuterBottom;
            } else if (OuterBottom <= other.Y) {
                dY = other.Y - OuterBottom;
            } 

            var dX = -1;
            if (X >= other.OuterRight) {
                dX = X - other.OuterRight;
            } else if (OuterRight <= other.X) {
                dX = other.X - OuterRight;
            } 

            return new Vector(dX, dY);
        }

        public bool Overlaps(Rect other)
            => Y < other.OuterBottom && OuterBottom > other.Y &&
               X < other.OuterRight  && OuterRight  > other.X;

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<Vector> GetEnumerator() {
            for (int x = X; x < X + Width; x++)
            for (int y = Y; y < Y + Height; y++)
                yield return new Vector(x, y);
        }
    }
}