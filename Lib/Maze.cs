using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using R = Lib.Randomizer;

namespace Lib {
    public class MazeConfig {
        public bool ConnectLeft { get; set; }
        public bool StraightPassageways { get; set; }
        public bool OverlappingRooms { get; set; }
        public bool ConnectCenter { get; set; }
    }

    public class Maze {
        public readonly CellType[,] Map;
        private readonly MazeConfig Config;

        public int Width { get; }
        public int Height { get; }

        public int MinRoomWidth { get; } = 3;
        public int MinRoomHeight { get; } = 3;
        public int MaxRoomWidth { get; } = 9;
        public int MaxRoomHeight { get; } = 9;


        public Maze(int width, int height, MazeConfig config = null) {
            Config = config ?? new MazeConfig();

            Width = width;
            Height = height;
            MaxRoomWidth = Math.Min(width, MaxRoomWidth);
            MaxRoomHeight = Math.Min(width, MaxRoomHeight);

            Map = new CellType[Width, Height];
            InitializeMap(new Rect(0, 0, width, height), CellType.Wall);


            Rect lastRoom = null;
            var rooms = Enumerable.Range(0, 3).Select(i => CreateRoom()).ToList();
            if (Config.ConnectCenter) rooms.Insert(0, new Rect(Width /2, Height/2, 1, 1));
            if (Config.ConnectLeft) rooms.Insert(0, new Rect(0, R.Next(1, Height-1), 1, 1));
            foreach (var room in rooms) {
                if (lastRoom != null) Connect(lastRoom, room);
                lastRoom = room;
            }
            for (int i = 0; i < 4; i++) {
                var room1 = CreateRoom();
                Connect(lastRoom, room1);
                lastRoom = room1;
            }
        }

        public static Maze Generate(int width, int height, MazeConfig config) {
            return new Maze(width, height);
        }

        private Rect CreateIsolatedRoom() {
            var room = new Rect(R.Next(Width),R.Next(Height),1,1);
            bool wallsOnly = true;
            for (int x = 0; x < room.Width; x++) {
                wallsOnly = wallsOnly && Map[x, room.Y - 1] == CellType.Wall;
            }
            if (wallsOnly) {
                room.Y--;
                room.Height++;
            }
            return room;
        }

        private Rect CreateRoom() {
            var roomWidth = R.Dice(MinRoomWidth, MaxRoomWidth);
            var roomHeight = R.Dice(MinRoomHeight, MaxRoomHeight);

            var room = new Rect(
                R.Next(Width - roomWidth),
                R.Next(Height - roomHeight),
                roomWidth,
                roomHeight
            );

            InitializeMap(room, CellType.Empty);
            return room;
        }

        private void InitializeMap(Rect rectangle, CellType cellType) {
            for (int x = 0; x < rectangle.Width; x++)
            for (int y = 0; y < rectangle.Height; y++) {
                Map[x + rectangle.X, y + rectangle.Y] = cellType;
            }
        }

        private void Connect(Rect room0, Rect room1) {
            var x0 = R.Next(room0.X, room0.X + room0.Width);
            var y0 = R.Next(room0.Y, room0.Y + room0.Height);
            var x1 = R.Next(room1.X, room1.X + room1.Width);
            var y1 = R.Next(room1.Y, room1.Y + room1.Height);

            Connect(x0, y0, x1, y1);
        }

        private void Connect(int x0, int y0, int x1, int y1) {
            Map[x0, y0] = CellType.Door;
            Map[x1, y1] = CellType.Door;

            if (x0 == x1 || y0 == y1) {
                InitializeMap(Rect.FromCorners(x0, y0, x1, y1), CellType.Empty);
                return;
            }

            int midpointX;
            int midpointY;
            if (Config.StraightPassageways) {
                midpointX = R.Next(1) * (x1 - x0) + x0;
                midpointY = R.Next(1) * (y1 - y0) + y1;
            } else {
                midpointX = R.Dice(x0, x1);
                midpointY = R.Dice(y0, y1);
            }
            Map[midpointX, midpointY] = CellType.Door;
            Connect(x0, y0, midpointX, midpointY);
            Connect(midpointX, midpointY, x1, y1);
        }
    }


    public class Rect {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private Rect() {}

        public Rect(int x, int y, int width, int height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            if (Width < 0) X += Width;
            if (Height < 0) Y += Height;
            Width = Math.Abs(Width);
            Height = Math.Abs(Height);
        }

        public static Rect FromCorners(int x0, int y0, int x1, int y1) {
            var x = new List<int> { x0, x1 };
            var y = new List<int> { y0, y1 };
            x.Sort();
            y.Sort();

            return new Rect {
                X = x[0],
                Y = y[0],
                Width = x[1] - x[0] + 1,
                Height = y[1] - y[0] + 1
            };
        }


        //public Rect Normalized() {
        //    var r = new Rect {
        //        X = X,
        //        Y = Y,
        //        Width = Width,
        //        Height = Height
        //    };
        //    if (r.Width < 0) r.X += r.Width;
        //    if (r.Height < 0) r.Y += r.Height;
        //    r.Width = Math.Abs(r.Width);
        //    r.Height = Math.Abs(r.Height);
        //    return r;
        //}
    }

    public enum CellType {
        Wall,
        Empty,
        StairsUp,
        StairsDown,
        Door
    }
}