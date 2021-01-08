using System;

namespace ERHMS.Console
{
    public class Highlighter : IDisposable
    {
        public Highlighter(ConsoleColor color = ConsoleColor.White)
        {
            System.Console.ForegroundColor = color;
        }

        public void Dispose()
        {
            System.Console.ResetColor();
        }
    }
}
