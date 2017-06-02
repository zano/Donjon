using System;
using System.Collections.Generic;
using System.Text;

namespace Lib {
    public class Ui {
        public Ui(int width, int height) : this() {
            Console.WindowWidth = Math.Min(width, Console.LargestWindowWidth);
            Console.WindowHeight = Math.Min(height, Console.LargestWindowHeight);
        }

        public Ui() {
            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.Unicode;
            Console.CancelKeyPress += (sender, args) => CancelKeyPress?.Invoke(sender, args);
        }

        public event ConsoleCancelEventHandler CancelKeyPress;

        public int Left { get; private set; }
        public int Top { get; private set; }

        public int Width => Console.WindowWidth;
        public int Height => Console.WindowHeight;

        public ConsoleKey AskForKey(string question) {
            Write(question);
            return Console.ReadKey(intercept: true).Key;
        }

        public string AskForString(string question) {
            Write(question);
            return Console.ReadLine();
        }

        public bool AskForIntOnce(string question, out int number)
            => int.TryParse(AskForString(question), out number);

        public int AskForInt(string question) {
            do {
                if (AskForIntOnce(question, out int number)) return number;
                WriteLine("Value must be a number");
            } while (true);
        }

        public void WriteLine(IEnumerable<string> message) {
            foreach (var s in message) WriteLine(s);
        }

        public void WriteLine(string message = "") {
            Console.WriteLine(Pad(message));
            Console.CursorLeft = Left;
        }

        public void Write(string message) => Console.Write(message);

        public void Write(ConsoleColor color, string message) {
            Console.ForegroundColor = color; 
            Write(message);
            Console.ResetColor();
        }

        private string Pad(string message) => message.PadRight(Console.WindowWidth - Console.CursorLeft);

        public void SetCorner(int left, int top) {
            Left = left;
            Top = top;
            Console.SetCursorPosition(left, top);
        }
    }
}