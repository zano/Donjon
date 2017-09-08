using System;
using System.Collections.Generic;
using System.Linq;
using Lib;

namespace Maze.DungeonBuilder {
    /// <summary>
    /// https://github.com/munificent/hauberk/blob/master/lib/src/content/dungeon.dart
    /// 
    /// The random dungeon generator.
    ///
    /// Starting with a stage of solid walls, it works like so:
    ///
    /// 1. Place a number of randomly sized and positioned rooms. If a room
    ///    overlaps an existing room, it is discarded. Any remaining rooms are
    ///    carved out.
    /// 2. Any remaining solid areas are filled in with mazes. The maze generator
    ///    will grow and fill in even odd-shaped areas, but will not touch any
    ///    rooms.
    /// 3. The result of the previous two steps is a series of unconnected rooms
    ///    and mazes. We walk the stage and find every tile that can be a
    ///    "connector". This is a solid tile that is adjacent to two unconnected
    ///    regions.
    /// 4. We randomly choose connectors and open them or place a door there until
    ///    all of the unconnected regions have been joined. There is also a slight
    ///    chance to carve a connector between two already-joined regions, so that
    ///    the dungeon isn't single connected.
    /// 5. The mazes will have a lot of dead ends. Finally, we remove those by
    ///    repeatedly filling in any open tile that's closed on three sides. When
    ///    this is done, every corridor in a maze actually leads somewhere.
    ///
    /// The end result of this is a multiply-connected dungeon with rooms and lots
    /// of winding corridors.
    /// </summary>
    internal class DungeonBuilder : LevelBuilder {
        private int numRoomTries = 100;

        /// The inverse chance of adding a connector between two regions that have
        /// already been joined. Increasing this leads to more loosely connected
        /// dungeons.
        private int extraConnectorChance = 20;

        /// Increasing this allows rooms to be larger.
        private int roomExtraSize = 0;

        private int windingPercent = 0;

        private readonly List<Rect> rooms = new List<Rect>();

        /// For each open position in the dungeon, the index of the connected region
        /// that that position is a part of.
        private int[,] regionMap;

        /// The index of the current region being carved.
        private int currentRegion = -1;

        public DungeonBuilder(int width, int height) : base(new Level(width, height)) {}

        public void Generate() {
            if (Level.Width % 2 == 0 || Level.Height % 2 == 0) throw new ArgumentException("The level must be odd-sized.");
            Fill(TileType.Wall);
            regionMap = new int[Level.Width, Level.Height];
            AddRooms();

            // Fill in all of the empty space with mazes.
            foreach (var pos in Bounds.Where(pos => GetTile(pos) == TileType.Wall)) GrowMaze(pos);


            ConnectRegions();
            RemoveDeadEnds();

            foreach (var room in rooms) {
                OnDecorateRoom(room);
            }
        }

        private void OnDecorateRoom(Rect room) {}

        /// Implementation of the "growing tree" algorithm from here:
        /// http://www.astrolog.org/labyrnth/algrithm.htm.
        private void GrowMaze(Vector startPos) {
            var positions = new Stack<Vector>();
            Direction lastDir = null;

            StartRegion();
            Carve(startPos);

            positions.Push(startPos);
            while (positions.Any()) {
                var tile = positions.Last();

                // See which adjacent cells are open.
                var neighbours = Direction.Cardinal
                    .Where(dir => CanCarve(tile, dir))
                    .ToList();

                if (neighbours.Any()) {
                    // Based on how "windy" passages are, try to prefer carving in the
                    // same direction.
                    Direction dir;
                    if (neighbours.Contains(lastDir) && !Randomizer.Chance(windingPercent)) {
                        dir = lastDir;
                    } else {
                        dir = neighbours.PickRandom();
                    }

                    Carve(tile + dir);
                    Carve(tile + dir * 2);

                    positions.Push(tile + dir * 2);
                    lastDir = dir;
                } else {
                    // No adjacent uncarved cells.
                    positions.Pop();

                    // This path has ended.
                    lastDir = null;
                }
            }
        }

        /// Places rooms ignoring the existing maze corridors.
        private void AddRooms() {
            for (var i = 0; i < numRoomTries; i++) {
                // Pick a random room size. The funny math here does two things:
                // - It makes sure rooms are odd-sized to line up with maze.
                // - It avoids creating rooms that are too rectangular: too tall and
                //   narrow or too wide and flat.
                // TODO: This isn't very flexible or tunable. Do something better here.
                var size = Randomizer.Next(1, 3 + roomExtraSize) * 2 + 1;
                var rectangularity = Randomizer.Next(0, 1 + size / 2) * 2;
                var width = size;
                var height = size;
                if (Randomizer.Chance(.5)) {
                    width += rectangularity;
                } else {
                    height += rectangularity;
                }

                var x = Randomizer.Next((Bounds.Width - width) / 2) * 2 + 1;
                var y = Randomizer.Next((Bounds.Height - height) / 2) * 2 + 1;

                var room = new Rect(x, y, width, height);

                if (rooms.Any(r => room.Overlaps(r))) continue;

                rooms.Add(room);
                StartRegion();
                foreach (var pos in room) Carve(pos);
            }
        }

        private void ConnectRegions() {
            // Find all of the tiles that can connect two (or more) regions.
            var connectorRegions = new Dictionary<Vector, HashSet<int>>();
            foreach (var pos in Bounds.Deflate()) {
                // Can't already be part of a region.
                if (GetTile(pos) != TileType.Wall) continue;

                var regions = new HashSet<int>();
                var neighbours = Direction.Cardinal
                    .Select(dir => pos + dir)
                    .Where(p => Bounds.Contains(p));

                foreach (var neighbour in neighbours) regions.Add(regionMap[neighbour.X, neighbour.Y]);
                if (regions.Count < 2) continue;
                connectorRegions[pos] = regions;
            }

            var connectors = connectorRegions.Keys.ToList();

            // Keep track of which regions have been merged. This maps an original
            // region index to the one it has been merged to.
            var merged = new Dictionary<int, int>();
            var openRegions = new HashSet<int>();
            for (var i = 0; i <= currentRegion; i++) {
                merged[i] = i;
                openRegions.Add(i);
            }

            // Keep connecting regions until we're down to one.
            while (openRegions.Count > 1) {
                var connector = connectors.PickRandom();
                if (connector == null) break;

                // Carve the connection.
                AddJunction(connector);

                // Merge the connected regions. We'll pick one region (arbitrarily) and
                // map all of the other regions to its index.
                var mRegions = connectorRegions[connector]
                    .Select(region => merged[region]).ToList();
                var dest = mRegions.First();
                var sources = mRegions.Skip(1).ToList();

                // Merge all of the affected regions. We have to look at *all* of the
                // regions because other regions may have previously been merged with
                // some of the ones we're merging now.
                for (var i = 0; i <= currentRegion; i++) {
                    if (sources.Contains(merged[i])) {
                        merged[i] = dest;
                    }
                }

                // The sources are no longer in use.            
                openRegions.RemoveWhere(i => sources.Contains(i));

                // Remove any connectors that aren't needed anymore.
                connectors.RemoveAll(pos => {
                    // Don't allow connectors right next to each other.
                    if ((connector - pos).Distance < 2) return true;

                    // If the connector no long spans different regions, we don't need it.
                    var regions = new HashSet<int>(connectorRegions[pos].Select(region => merged[region]));
                    if (regions.Count > 1) return false;

                    // This connecter isn't needed, but connect it occasionally so that the
                    // dungeon isn't singly-connected.
                    if (Randomizer.ChanceIn(extraConnectorChance)) AddJunction(pos);

                    return true;
                });
            }
        }

        private void AddJunction(Vector pos) {
            var tile = Randomizer.ChanceIn(4) ? (Randomizer.ChanceIn(3) ? TileType.OpenDoor : TileType.Floor) : TileType.ClosedDoor;
            SetTile(pos, tile);
        }

        private void RemoveDeadEnds() {
            var done = false;

            while (!done) {
                done = true;

                foreach (var pos in Bounds.Deflate()) {
                    if (GetTile(pos) == TileType.Wall) continue;

                    // If it only has one exit, it's a dead end.
                    var exits = Direction.Cardinal.Count(dir => GetTile(pos + dir) != TileType.Wall);
                    if (exits != 1) continue;

                    done = false;
                    SetTile(pos, TileType.Wall);
                }
            }
        }

        /// <summary>
        /// Gets whether or not an opening can be carved from the given starting
        /// [Cell] at [pos] to the adjacent Cell facing [direction]. 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="direction"></param>
        /// <returns>Returns `true` if the starting Cell is in bounds and the 
        /// destination Cell is filled (or out of bounds).</returns>
        private bool CanCarve(Vector pos, Direction direction) {
            // Must end in bounds.
            if (!Bounds.Contains(pos + direction * 3)) return false;

            // Destination must not be open.
            return GetTile(pos + direction * 2) == TileType.Wall;
        }

        private void StartRegion() => currentRegion++;

        private void Carve(Vector pos, TileType type = TileType.Floor) {
            SetTile(pos, type);
            regionMap[pos.X, pos.Y] = currentRegion;
        }
    }

    public class Generator {
        public static Level Dungeon(int width, int height) {
            var builder = new DungeonBuilder(width, height);
            builder.Generate();
            return builder.Level;
        }
    }
}