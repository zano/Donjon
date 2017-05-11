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
        private string updateMessage = "";

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
                Update();

                // rita spelplan och övrig information
                Draw();

            }
        }

        private void Update()
        {
            Cell cell = map.Cell(hero.X, hero.Y);
            GameObject obj = (GameObject)cell.Monster ?? cell.Item;
            if (obj != null) {
                updateMessage = hero.Action(obj);
                if (obj is Monster && (obj as Monster).Health > 0) updateMessage += obj.Action(hero);
                if (obj.RemoveFromCell) obj.RemoveFrom(cell);
            }
        }

        private void Draw()
        {
            Console.SetCursorPosition(0, 0);
            map.Print(hero);
            Console.WriteLine("Hero's health: " + hero.Health);
            Console.WriteLine("Hero's damage: " + hero.Damage);
            Cell currentCell = map.Cell(hero.X, hero.Y);

            GameObject content = (GameObject)currentCell.Monster ?? currentCell.Item;
            string message = "";

            if (content != null) {
                message = $"You see a {content.Name}";
                if (content is Creature) {
                    message += $" ({ (content as Creature).Health } hp)";
                }
            }
            Console.WriteLine(message.PadRight(Console.WindowWidth));
            Console.WriteLine(updateMessage.PadRight(Console.WindowWidth));
            updateMessage = "";
          
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
                case ConsoleKey.I:
                    updateMessage += hero.Inventory();
                    break;
            }
        }
    }
}