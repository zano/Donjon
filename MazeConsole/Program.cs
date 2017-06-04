using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;

namespace MazeConsole {
    class Program {
        static void Main(string[] args) {
            var symbols = new Dictionary<CellType, string> {
                { CellType.Door, "x" },
                { CellType.Wall, "#" },
                { CellType.Empty, "." },
            };

            var width = 100;
            var height = 30;
            var ui = new Ui(width+4, height+12);
            var maze = new Maze(width, height);
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    ui.Write(symbols[maze.Map[x, y]]);
                }
                ui.WriteLine();
            }
            ui.AskForKey("");
        }
    }
}