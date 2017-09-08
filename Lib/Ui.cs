using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Text;

namespace Lib {
    public class Ui {
        private ConsoleColor backgroundColor;
        private ConsoleColor foregroundColor;

        public int Left { get; private set; }
        public int Top { get; private set; }

        public int Width => Console.WindowWidth;
        public int Height => Console.WindowHeight;

        public ConsoleColor ForegroundColor {
            get { return foregroundColor; }
            set {
                if (foregroundColor != value) {
                    foregroundColor = value;
                    Console.ForegroundColor = value;
                }
            }
        }

        public ConsoleColor BackgroundColor {
            get { return backgroundColor; }
            set {
                if (backgroundColor != value) {
                    backgroundColor = value;
                    Console.BackgroundColor = value;
                }
            }
        }

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

        public ConsoleKey AskForKey(string question) {
            Write(question);
            return Console.ReadKey(intercept: true).Key;
        }

        public string AskForString(string question) {
            Write(question);
            return Console.ReadLine();
        }

        public bool AskForIntOnce(string question, out int number)
            => Int32.TryParse(AskForString(question), out number);

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

        public void WriteAt(int left, int top, string message) {
            Console.SetCursorPosition(Left + left, Top + top);
            Console.Write(message);
        }
            

        public void Write(string message) => Console.Write(message);

        public void Write(string message, ConsoleColor color) {
            ForegroundColor = color;
            Write(message);
        }

        public void Write(string message, ConsoleColor color, ConsoleColor background) {
            BackgroundColor = background;
            Write(message, color);
        }

        public void SetCorner(int left, int top) {
            Left = left;
            Top = top;
            Console.SetCursorPosition(left, top);
        }

        private string Pad(string message) => message.PadRight(Math.Max(0, Console.WindowWidth - Console.CursorLeft));
    }
}