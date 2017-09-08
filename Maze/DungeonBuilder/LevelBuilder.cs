using System.Linq;
using Lib;

namespace Maze.DungeonBuilder {
    internal abstract class LevelBuilder {
        public Level Level { get; protected set; }

        protected Rect Bounds => Level.Bounds;
        protected TileType GetTile(Vector pos) => Level.CellAt(pos).Tile;
        protected void SetTile(Vector pos, TileType type) => Level.CellAt(pos).Tile = type;

        public LevelBuilder(Level level) {
            Level = level;
        }

        protected void Fill(TileType type) {
            foreach (var pos in Bounds) SetTile(pos, type);
        }

        /// Randomly turns some [wall] tiles into [floor] and vice versa.
        private void Erode(int iterations, TileType floor = TileType.Floor, TileType wall = TileType.Wall) {
            var bounds = Level.Bounds.Deflate();
            for (var i = 0; i < iterations; i++) {
                // TODO: This way this works is super inefficient. Would be better to
                // keep track of the floor tiles near open ones and choose from them.
                var pos = bounds.PickRandom();

                if (GetTile(pos) != wall) continue;

                // Keep track of how many floors we're adjacent too. We will only erode
                // if we are directly next to a floor.
                var floors = Direction.All.Count(dir => GetTile(pos + dir) == floor);

                // Prefer to erode tiles near more floor tiles so the erosion isn't too
                // spiky.
                if (floors < 2) continue;

                if (Randomizer.ChanceIn(9 - floors)) SetTile(pos, floor);
            }
        }
    }
}