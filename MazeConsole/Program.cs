using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lib;
using Maze;
using Maze.DungeonBuilder;

namespace MazeConsole {
    internal class Program {
        private static void Main() {

            var width = 30;
            var height = 30;
            var ui = new Ui(width * 2 + 1, height + 2);

            var symbols = new Dictionary<MazeBuilder.CellType, string> {
                { MazeBuilder.CellType.Door, " D" },
                { MazeBuilder.CellType.Wall, " #" },
                { MazeBuilder.CellType.Open, "  " },
            };

            var tiles = new Dictionary<TileType, string> {
                { TileType.OpenDoor, " □" },
                { TileType.ClosedDoor, " ■" },
                { TileType.Wall, "█#" },
                { TileType.Floor, "  " },
            };

            do {
                Console.Clear();
                //var map = MazeBuilder.Generate(width, height).Map;
                //for (int y = 0; y < height; y++) {
                //    for (int x = 0; x < width; x++) {
                //        ui.Write(symbols[map[x, y]]);
                //    }                    
                //    ui.WriteLine();
                //}
                var level = Generator.Dungeon(width/2*2+1, height/2*2+1);
                foreach (var position in level.Bounds) {
                    ui.WriteAt(position.X, position.Y, tiles[level.CellAt(position).Tile]);
                }
            } while (ui.AskForKey("Q to quit") != ConsoleKey.Q);
        }
    }
}