namespace Donjon
{
    internal class Hero
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Symbol { get; private set; }

        public Hero(int health)
        {
            Health = health;
            Symbol = "H";
        }
    }
}