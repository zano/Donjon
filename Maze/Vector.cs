namespace Maze {
    public class Vector {
        public int X { get; set; }
        public int Y { get; set; }
        public int Distance => X + Y;

        public Vector(int x, int y) {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector v, Vector w) => new Vector(v.X + w.X, v.Y + w.Y);
        public static Vector operator -(Vector v, Vector w) => new Vector(v.X + -w.X, v.Y + -w.Y);
        public static Vector operator -(Vector v) => new Vector(-v.X, -v.Y);
        public static Vector operator *(Vector v, int i) => new Vector(v.X * i, v.Y * i);
    }
}