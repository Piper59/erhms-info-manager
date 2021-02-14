using Epi;
using static System.Console;

namespace ERHMS.Console.Utilities
{
    public class Decrypt : IUtility
    {
        public string Text { get; }

        public Decrypt(string text)
        {
            Text = text;
        }

        public void Run()
        {
            Out.WriteLine(Configuration.Decrypt(Text));
        }
    }
}
