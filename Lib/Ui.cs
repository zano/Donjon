using System;
using System.Runtime.CompilerServices;

namespace Lib
{
    public static class Ui
    {
        private static int leftColumn;
        public static ConsoleKey AskForKey(string question) => Console.ReadKey(intercept: true).Key;

        public static void WriteLine(string message) {
            Console.WriteLine(Pad(message));
            Console.CursorLeft = leftColumn;
        }
        public static void Write(string message) => Console.Write(Pad(message));
        private static string Pad(string message) => message.PadRight(Console.WindowWidth - Console.CursorLeft);

        public static void SetCursorPosition(int left, int top) {
            leftColumn = left;
            Console.SetCursorPosition(left, top);
        }
    }
}