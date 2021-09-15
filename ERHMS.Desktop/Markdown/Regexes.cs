using System.Text.RegularExpressions;

namespace ERHMS.Desktop.Markdown
{
    public static class Regexes
    {
        public static Regex LineBreak { get; } = new Regex(@"\r\n|\r|\n");
        public static Regex BlockSeparator { get; } = new Regex(@"(?:\r\n\r\n|\r\r|\n\n)[\r\n]*");
    }
}
