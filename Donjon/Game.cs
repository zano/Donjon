using System;

namespace Donjon
{
    internal class Game
    {
        private int height;
        private int width;

        private Map map;
        private Hero hero;

        public Game(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        internal void Run()
        {
            // Initialisera
            // skapa hjälte
            hero = new Hero(health: 100) {
                Damage = 10
            };

            // Karta, 
            map = new Map(width, height);

            // placera monster
            map.Populate();

            // gör en första utskrift
            map.Print(hero);

            bool quit = false;
            while (!quit)
            {
                // uppdatera
                //  hantera indata
                ConsoleKey key = Console.ReadKey(intercept: true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (hero.Y > 0) hero.Y--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (hero.Y < height - 1) hero.Y++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (hero.X > 0) hero.X--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (hero.X < width - 1) hero.X++;
                        break;
                }

                //  updatera spelobjekt

                // rita spelplan och övrig information
                Console.Clear();
                map.Print(hero);

                // avsluta slingan
                //quit = true;
            }
        }
    }
}