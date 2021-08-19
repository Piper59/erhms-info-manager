using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace ERHMS.Desktop.Markdown
{
    public class InlineNode
    {
        public string Prefix { get; }
        public Inline Inline { get; }
        public string Suffix { get; }

        public InlineNode(Inline inline)
        {
            Prefix = "";
            Inline = inline;
            Suffix = "";
        }

        public InlineNode(string text, Match match, Inline inline)
        {
            Prefix = text.Substring(0, match.Index);
            Inline = inline;
            Suffix = text.Substring(match.Index + match.Length);
        }
    }
}
