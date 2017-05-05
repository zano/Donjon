namespace Donjon
{
    internal class Cell
    {
        public Monster Monster { get; set; }
        public string Symbol {
            get {
                if (Monster == null) return ".";
                return Monster.Symbol;
            }
        }
    }
}