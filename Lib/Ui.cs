using System;
using System.Collections.Generic;

namespace Lib {
    public class Ui {
        public int Left { get; private set; }
        public int Top { get; private set; }

        public ConsoleKey AskForKey(string question) {
            WriteLine(question);
            return Console.ReadKey(intercept: true).Key;
        }

        public string AskForString(string question) {
            WriteLine(question);
            return Console.ReadLine();
        }

        public int AskForInt(string question) {
            int result;
            do {
                if (int.TryParse(AskForString(question), out result)) return result;
                WriteLine("Value must be a number");
            } while (true);
        }

        public void WriteLine(IEnumerable<string> message)
        {
            foreach (var s in message) WriteLine(s);
        }
        public void WriteLine(string message)
        {
            Console.WriteLine(Pad(message));
            Console.CursorLeft = Left;
        }

        public void Write(string message) => Console.Write(message);

        private string Pad(string message) => message.PadRight(Console.WindowWidth - Console.CursorLeft);

        public void SetCorner(int left, int top) {
            Left = left;
            Top = top;
            Console.SetCursorPosition(left, top);
        }

    }
}