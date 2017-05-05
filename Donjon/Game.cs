using System;

namespace Donjon
{
    internal class Game
    {
        private int height;
        private int width;
        private bool quit = false;

        private Map map;
        private Hero hero;


        public Game(int width, int height)
        {
            this.width = width;
            this.height = height;
            Console.CursorVisible = false;
            Console.WindowWidth = width * 2 + 5;
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
            Draw();
          
            while (!quit)
            {
                // uppdatera
                //  hantera indata
                UserInput();

                //  updatera spelobjekt


                // rita spelplan och övrig information
                Draw();

                // avsluta slingan
                //quit = true;
            }
        }

        private void Draw()
        {
            Console.SetCursorPosition(0, 0);
            map.Print(hero);
            Console.WriteLine("Hero's health: " + hero.Health);
            Console.WriteLine("Hero's damage: " + hero.Damage);
            Cell currentCell = map.Cell(hero.X, hero.Y);
            var m = currentCell.Monster;
            var s = "";
            if (m != null)
            {
                Console.WriteLine($"You see a {m.Name} ({m.Health} hp)");
            }
            else
            {               
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }
        }

        private void UserInput()
        {
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
                case ConsoleKey.Q:
                    quit = true;
                    break;
            }
        }
    }
}