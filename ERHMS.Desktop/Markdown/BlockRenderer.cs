using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace ERHMS.Desktop.Markdown
{
    public abstract class BlockRenderer : TextElementRenderer
    {
        public class AsHeading : BlockRenderer
        {
            public int Level { get; }
            private Regex Regex { get; }

            public AsHeading(RenderingContext context, int level)
                : base(context)
            {
                Level = level;
                Regex = new Regex($@"^#{{{level}}}\s+(?<text>.+)$");
            }

            public override bool TryRender(string text, out Block block)
            {
                Match match = Regex.Match(text);
                if (match.Success)
                {
                    Paragraph paragraph = new Paragraph();
                    paragraph.SetResourceReference(FrameworkElement.StyleProperty, Context.GetHeadingStyleKey(Level));
                    paragraph.Inlines.AddRange(Context.GetInlines(match.Groups["text"].Value));
                    block = paragraph;
                    return true;
                }
                else
                {
                    block = null;
                    return false;
                }
            }
        }

        public class AsList : BlockRenderer
        {
            private static Regex ItemRegex { get; } = new Regex(@"^-\s+(?<text>.+)$");

            public AsList(RenderingContext context)
                : base(context) { }

            public override bool TryRender(string text, out Block block)
            {
                IEnumerable<string> lines = Regexes.LineBreak.Split(text);
                if (lines.All(ItemRegex.IsMatch))
                {
                    List list = new List();
                    foreach (string line in lines)
                    {
                        Match match = ItemRegex.Match(line);
                        Paragraph body = new Paragraph();
                        body.Inlines.AddRange(Context.GetInlines(match.Groups["text"].Value));
                        ListItem listItem = new ListItem(body);
                        list.ListItems.Add(listItem);
                    }
                    block = list;
                    return true;
                }
                else
                {
                    block = null;
                    return false;
                }
            }
        }

        public class AsParagraph : BlockRenderer
        {
            public AsParagraph(RenderingContext context)
                : base(context) { }

            public override bool TryRender(string text, out Block block)
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.AddRange(Context.GetInlines(Regexes.LineBreak.Replace(text, " ")));
                block = paragraph;
                return true;
            }
        }

        public static IEnumerable<BlockRenderer> GetInstances(RenderingContext context)
        {
            yield return new AsHeading(context, 1);
            yield return new AsHeading(context, 2);
            yield return new AsHeading(context, 3);
            yield return new AsList(context);
            yield return new AsParagraph(context);
        }

        protected BlockRenderer(RenderingContext context)
            : base(context) { }

        public abstract bool TryRender(string text, out Block block);
    }
}
