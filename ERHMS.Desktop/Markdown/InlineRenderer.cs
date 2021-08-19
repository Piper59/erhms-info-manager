using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace ERHMS.Desktop.Markdown
{
    public abstract class InlineRenderer : TextElementRenderer
    {
        public class AsHyperlink : InlineRenderer
        {
            private static Regex Regex { get; } = new Regex(@"\[(?<text>[^]]+)\]\((?<uri>[^)]+)\)");

            public AsHyperlink(RenderingContext context)
                : base(context) { }

            public override bool TryRender(string text, out InlineNode node)
            {
                Match match = Regex.Match(text);
                if (match.Success)
                {
                    Run run = new Run(match.Groups["text"].Value);
                    Hyperlink hyperlink = new Hyperlink(run)
                    {
                        NavigateUri = new Uri(match.Groups["uri"].Value, UriKind.RelativeOrAbsolute)
                    };
                    node = new InlineNode(text, match, hyperlink);
                    return true;
                }
                else
                {
                    node = null;
                    return false;
                }
            }
        }

        public class AsEmphasis : InlineRenderer
        {
            private static Regex Regex { get; } = new Regex(@"\*(?<text>[^*]+)\*");

            public AsEmphasis(RenderingContext context)
                : base(context) { }

            public override bool TryRender(string text, out InlineNode node)
            {
                Match match = Regex.Match(text);
                if (match.Success)
                {
                    Run run = new Run(match.Groups["text"].Value);
                    SetStyle(run, Context.EmphasisStyleKey);
                    node = new InlineNode(text, match, run);
                    return true;
                }
                else
                {
                    node = null;
                    return false;
                }
            }
        }

        public class AsRun : InlineRenderer
        {
            public AsRun(RenderingContext context)
                : base(context) { }

            public override bool TryRender(string text, out InlineNode node)
            {
                Run run = new Run(text);
                node = new InlineNode(run);
                return true;
            }
        }

        public static IEnumerable<InlineRenderer> GetInstances(RenderingContext context)
        {
            yield return new AsHyperlink(context);
            yield return new AsEmphasis(context);
            yield return new AsRun(context);
        }

        protected InlineRenderer(RenderingContext context)
            : base(context) { }

        public abstract bool TryRender(string text, out InlineNode node);
    }
}
