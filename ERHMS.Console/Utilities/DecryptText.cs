using Epi;

namespace ERHMS.Console.Utilities
{
    public class DecryptText : Utility
    {
        public string Text { get; }

        public DecryptText(string text)
        {
            Text = text;
        }

        public override void Run()
        {
            Out.WriteLine(Configuration.Decrypt(Text));
        }
    }
}
