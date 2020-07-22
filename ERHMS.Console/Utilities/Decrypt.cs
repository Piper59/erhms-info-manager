using Epi;

namespace ERHMS.Console.Utilities
{
    public class Decrypt : Utility
    {
        public string Text { get; }

        public Decrypt(string text)
        {
            Text = text;
        }

        protected override void RunCore()
        {
            Out.WriteLine(Configuration.Decrypt(Text));
        }
    }
}
