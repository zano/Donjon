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
            var cell = cells[4, 7];
            cell.Monster = new Monster();
        }

        internal void Print(Hero hero)
        {
            for (int y = 0; y < height; y++)
            {
                var row = "";
                for (int x = 0; x < width; x++)
                {
                    // spacer
                    row += " ";

                    if (x == hero.X && y == hero.Y)
                    {
                        // vi har en hjälte här
                        row += hero.Symbol;
                    }
                    else
                    {
                        // bara en cell
                        row += cells[x, y].Symbol;
                    }
                }
                Console.WriteLine(row);
            }
        }
    }
}