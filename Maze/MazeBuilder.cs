using System;
using System.Collections.Generic;
using System.Linq;
using Lib;

namespace Maze {
    public class MazeConfig {
        public bool ConnectLeft { get; set; }
        public bool StraightPassageways { get; set; }
        public bool OverlappingRooms { get; set; }
        public bool ConnectCenter { get; set; }
        public bool Enclosed { get; set; } = true;
        public bool StairsUp { get; set; }
        public bool StairDown { get; set; }

        public int MinRoomWidth { get; set; } = 3;
        public int MinRoomHeight { get; set; } = 3;
        public int MaxRoomWidth { get; set; } = 9;
        public int MaxRoomHeight { get; set; } = 9;
    }

    public class MazeBuilder {
        public readonly CellType[,] Map;
        public readonly MazeConfig Config;

        public int Width { get; }
        public int Height { get; }

        private MazeBuilder(int width, int height, MazeConfig config) {
            Config = config;
            Width = width;
            Height = height;
            Config.MaxRoomWidth = Math.Min(width, Config.MaxRoomWidth);
            Config.MaxRoomHeight = Math.Min(height, Config.MaxRoomHeight);
            Map = new CellType[Width, Height];
        }

        private Rect MapArea() {
            var area = new Rect(0, 0, Width, Height);
            if (Config.Enclosed) area.Shrink();
            return area;
        }

        public static MazeBuilder Generate(int width, int height, MazeConfig config = null) {
            var mazeBuilder = new MazeBuilder(width, height, config ?? new MazeConfig());
            mazeBuilder.Build();
            return mazeBuilder;
        }

        private void Build() {
            if (Config.OverlappingRooms) {
                BuildOverlapping();
            } else {
                BuildDense();
            }
        }

        private void BuildDense() {
            //var area = new Rect(1, 1, Width - 1, Height - 1);
            var area = MapArea();
            SetCellType(area, CellType.Any);
            int rooms = 0;
            while (rooms++ < 1) {
                // pick random undecided cell
                var room = PickRandomAvailableCell(area);
                if (room == null) return;

                // grow until stop or maxsize

                bool grew;
                do {
                    grew = false;
                    if (room.Width < Config.MaxRoomWidth && room.Right < Width - 1) {
                        var wider = new Rect(room.X, room.Y, room.Width + 1, room.Height);
                        if (Cells(wider).All(c => c == CellType.Any)) {
                            room = wider;
                            grew = true;
                        }
                    }

                    if (room.Height < Config.MaxRoomHeight && room.Bottom < Height - 1) {
                        var higher = new Rect(room.X, room.Y, room.Width, room.Height + 1);
                        if (Cells(higher).All(c => c == CellType.Any)) {
                            room = higher;
                            grew = true;
                        }
                    }
                } while (grew);


                var walls = new Rect(room.X - 1, room.Y - 1, room.Width + 2, room.Height + 2);

                //SetCellType(walls, CellType.Wall);
                SetCellType(room, CellType.Open);
            }

            for (int x = area.X; x < area.Right; x++)
            for (int y = area.Y; y < area.Bottom; y++) {
                if (Map[x, y] == CellType.Any) Map[x, y] = CellType.Wall;
            }
        }

        private Rect PickRandomAvailableCell(Rect area) {
            CellType cell;
            var iterations = 100;
            int x;
            int y;
            do {
                iterations--;
                x = Randomizer.Dice(area.X, area.Width - 1);
                y = Randomizer.Dice(area.Y, area.Height - 1);
                cell = Map[x, y];
                if (iterations == 0) return null;
            } while (cell != CellType.Any);
            var room = new Rect(x, y, 1, 1);
            return room;
        }

        private IEnumerable<CellType> Cells(Rect area) {
            for (int x = area.X; x < area.X + area.Width; x++)
            for (int y = area.Y; x < area.Y + area.Height; y++) {
                yield return Map[x, y];
            }
        }


        private void BuildOverlapping() {
            var area = MapArea();

            var rooms = new LinkedList<Rect>(Enumerable.Repeat(0, 20).Select(i => CreateRoom(area)));
            if (Config.ConnectCenter) rooms.AddFirst(new Rect(Width / 2, Height / 2, 1, 1));
            if (Config.ConnectLeft) rooms.AddFirst(new Rect(0, Randomizer.Next(1, Height - 1), 1, 1));

            Rect lastRoom = null;
            foreach (var room in rooms) {
                if (lastRoom != null) Connect(lastRoom, room);
                lastRoom = room;
            }
        }


        private Rect CreateIsolatedRoom() {
            var room = new Rect(Randomizer.Next(Width), Randomizer.Next(Height), 1, 1);
            bool wallsOnly = true;
            for (int x = 0; x < room.Width; x++) {
                wallsOnly = wallsOnly && Map[x, room.Y - 1] == CellType.Wall;
            }
            if (wallsOnly) {
                //room.Y--;
                //room.Height++;
            }
            return room;
        }

        private Rect CreateRoom(Rect rect = null) {
            rect = rect ?? new Rect(0, 0, Width, Height);

            var roomWidth = Randomizer.Dice(Config.MinRoomWidth, Config.MaxRoomWidth);
            var roomHeight = Randomizer.Dice(Config.MinRoomHeight, Config.MaxRoomHeight);

            var x = Randomizer.Dice(rect.X, rect.Width - roomWidth);
            var y = Randomizer.Dice(rect.Y, rect.Height - roomHeight);

            SetCellType(new Rect(x, y, roomWidth, roomHeight), CellType.Open);
            return new Rect(x, y, roomWidth, roomHeight);
        }

        private void SetCellType(Rect rectangle, CellType cellType) {
            for (int x = 0; x < rectangle.Width; x++)
            for (int y = 0; y < rectangle.Height; y++) {
                Map[x + rectangle.X, y + rectangle.Y] = cellType;
            }
        }

        private void Connect(Rect room0, Rect room1) {
            // avoid corner doors
            room0.Shrink();
            room1.Shrink();

            var x0 = room0.X + Randomizer.Next(room0.Width);
            var y0 = room0.Y + Randomizer.Next(room0.Height);
            var x1 = room1.X + Randomizer.Next(room1.Width);
            var y1 = room1.Y + Randomizer.Next(room1.Height);

            Connect(x0, y0, x1, y1);
        }

        private void Connect(int x0, int y0, int x1, int y1) {
            Map[x0, y0] = CellType.Door;
            Map[x1, y1] = CellType.Door;

            if (x0 == x1 || y0 == y1) {
                SetCellType(Rect.FromCorners(x0, y0, x1, y1), CellType.Open);
                return;
            }

            int midpointX;
            int midpointY;
            if (Config.StraightPassageways) {
                midpointX = Randomizer.Next(1) * (x1 - x0) + x0;
                midpointY = Randomizer.Next(1) * (y1 - y0) + y1;
            } else {
                midpointX = Randomizer.Dice(x0, x1);
                midpointY = Randomizer.Dice(y0, y1);
            }
            Map[midpointX, midpointY] = CellType.Door;
            Connect(x0, y0, midpointX, midpointY);
            Connect(midpointX, midpointY, x1, y1);
        }

        public enum CellType {
            Wall = 0,
            Open,
            StairsUp,
            StairsDown,
            Door,
            Any
        }
    }
}