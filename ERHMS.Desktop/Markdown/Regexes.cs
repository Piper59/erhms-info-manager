using System.Text.RegularExpressions;

namespace ERHMS.Desktop.Markdown
{
    public static class Regexes
    {
        private const string LineBreakPattern = @"\r\n|\r|\n";

        public static Regex LineBreak { get; } = new Regex(LineBreakPattern);
        public static Regex BlockSeparator { get; } = new Regex($@"(?:{LineBreakPattern}){{2}}");
    }
}
