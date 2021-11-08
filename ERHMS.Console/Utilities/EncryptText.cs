using Epi;

namespace ERHMS.Console.Utilities
{
    public class EncryptText : Utility
    {
        public string Text { get; }

        public EncryptText(string text)
        {
            Text = text;
        }

        public override void Run()
        {
            Out.WriteLine(Configuration.Encrypt(Text));
        }
    }
}
