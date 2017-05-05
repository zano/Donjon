using System;

namespace Donjon
{
    internal class Map
    {
        private int height;
        private int width;
        private Cell[,] cells;

        public Map(int width, int height)
        {
            this.width = width;
            this.height = height;

            // instansiera 2d-Cellvektorn
            cells = new Cell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new Cell();
                }
            }
        }

        internal void Populate()
        {
            cells[4, 7].Monster = new Orc();
            cells[7, 4].Monster = new Goblin();
        }

        internal void Print(Hero hero)
        {
            for (int y = 0; y < height; y++)
            {               
                for (int x = 0; x < width; x++)
                {
                    IDrawable d;
                    if (x == hero.X && y == hero.Y)
                    {
                        // vi har en hjälte här
                        d = hero;
                    }
                    else
                    {
                        // bara en cell
                        d = cells[x, y];                       
                    }
                    Console.ForegroundColor = d.Color;
                    Console.Write(" " + d.Symbol);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        internal Cell Cell(int x, int y) => cells[x, y];
    }
}