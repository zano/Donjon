namespace Maze.DungeonBuilder {
    public enum TileType {
        None = 0,
        Floor,
        Wall,
        OpenDoor,
        ClosedDoor
    }

    public class Cell {
        public TileType Tile { get; set; }
        // Todo: could track its position, region, connections
    }

    public class Level {
        public int Width { get; }
        public int Height { get; }
        public Rect Bounds => new Rect(0, 0, Width, Height);

        private readonly Cell[,] cells;
        public Cell CellAt(Vector pos) => cells[pos.X, pos.Y];

        public Level(int width, int height) {
            Width = width;
            Height = height;
            cells = new Cell[width, height];
            foreach (var pos in Bounds) cells[pos.X, pos.Y] = new Cell();
        }
    }

    internal class Direction : Vector {
        public Direction(int x, int y) : base(x, y) {}

        public static Direction None = new Direction(0, 0);

        public static Direction N  = new Direction(0, -1);
        public static Direction Ne = new Direction(1, -1);
        public static Direction E  = new Direction(1, 0);
        public static Direction Se = new Direction(1, 1);
        public static Direction S  = new Direction(0, 1);
        public static Direction Sw = new Direction(-1, 1);
        public static Direction W  = new Direction(-1, 0);
        public static Direction Nw = new Direction(-1, -1);

        /// The eight cardinal and intercardinal directions.
        public static Direction[] All = { N, Ne, E, Se, S, Sw, W, Nw };

        public static Direction[] Cardinal = { N, E, S, W };
    }
}