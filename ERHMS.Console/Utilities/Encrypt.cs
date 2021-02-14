using Epi;
using static System.Console;

namespace ERHMS.Console.Utilities
{
    public class Encrypt : IUtility
    {
        public string Text { get; }

        public Encrypt(string text)
        {
            Text = text;
        }

        public void Run()
        {
            Out.WriteLine(Configuration.Encrypt(Text));
        }
    }
}
