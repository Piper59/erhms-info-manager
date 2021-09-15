using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
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

        public abstract class AsStyled : InlineRenderer
        {
            protected abstract Regex Regex { get; }
            protected abstract object StyleKey { get; }

            public AsStyled(RenderingContext context)
                : base(context) { }

            public override bool TryRender(string text, out InlineNode node)
            {
                Match match = Regex.Match(text);
                if (match.Success)
                {
                    Run run = new Run(match.Groups["text"].Value);
                    run.SetResourceReference(FrameworkElement.StyleProperty, StyleKey);
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

        public class AsEmphasized : AsStyled
        {
            protected override Regex Regex { get; } = new Regex(@"\*(?<text>[^*]+)\*");
            protected override object StyleKey => Context.EmphasizedStyleKey;

            public AsEmphasized(RenderingContext context)
                : base(context) { }
        }

        public class AsStrong : AsStyled
        {
            protected override Regex Regex { get; } = new Regex(@"\*\*(?<text>[^*]+)\*\*");
            protected override object StyleKey => Context.StrongStyleKey;

            public AsStrong(RenderingContext context)
                : base(context) { }
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
            yield return new AsStrong(context);
            yield return new AsEmphasized(context);
            yield return new AsRun(context);
        }

        protected InlineRenderer(RenderingContext context)
            : base(context) { }

        public abstract bool TryRender(string text, out InlineNode node);
    }
}
