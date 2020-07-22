using Epi;

namespace ERHMS.Console.Utilities
{
    public class Encrypt : Utility
    {
        public string Text { get; }

        public Encrypt(string text)
        {
            Text = text;
        }

        protected override void RunCore()
        {
            Out.WriteLine(Configuration.Encrypt(Text));
        }
    }
}
