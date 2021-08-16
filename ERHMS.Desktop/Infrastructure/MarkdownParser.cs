using ERHMS.Desktop.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace ERHMS.Desktop.Infrastructure
{
    public static class MarkdownParser
    {
        private static readonly IDictionary<int, Regex> headingRegexesByLevel = new Dictionary<int, Regex>
        {
            { 1, new Regex(@"^#\s+(?<text>.+)$") },
            { 2, new Regex(@"^##\s+(?<text>.+)$") }
        };
        private static readonly Regex listItemRegex = new Regex(@"^-\s+(?<text>.+)$");
        private static readonly Regex hyperlinkRegex = new Regex(@"\[(?<text>[^]]+)\]\((?<uri>[^)]+)\)");

        private static IEnumerable<IList<string>> Split(string text)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (StreamReader reader = new StreamReader(stream))
            {
                IList<string> lines = new List<string>();
                while (true)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        if (lines.Count > 0)
                        {
                            yield return lines;
                            lines.Clear();
                        }
                        if (line == null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        lines.Add(line.Trim());
                    }
                }
            }
        }

        private static IEnumerable<Inline> GetInlines(string text)
        {
            MatchCollection matches = hyperlinkRegex.Matches(text);
            if (matches.Count == 0)
            {
                yield return new Run(text);
                yield break;
            }
            int index = 0;
            foreach (Match match in matches)
            {
                if (match.Index > index)
                {
                    yield return new Run(text.Substring(index, match.Index - index));
                }
                Run run = new Run(match.Groups["text"].Value);
                yield return new WebHyperlink(run)
                {
                    NavigateUri = new Uri(match.Groups["uri"].Value)
                };
                index = match.Index + match.Length;
            }
            if (index < text.Length)
            {
                yield return new Run(text.Substring(index, text.Length - index));
            }
        }

        private static bool TryAddHeading(FlowDocument document, IList<string> lines, int level)
        {
            if (lines.Count == 1)
            {
                Match match = headingRegexesByLevel[level].Match(lines[0]);
                if (match.Success)
                {
                    Paragraph paragraph = new Paragraph
                    {
                        Style = (Style)Application.Current.FindResource($"Heading{level}")
                    };
                    paragraph.Inlines.AddRange(GetInlines(match.Groups["text"].Value));
                    document.Blocks.Add(paragraph);
                    return true;
                }
            }
            return false;
        }

        private static bool TryAddList(FlowDocument document, IList<string> lines)
        {
            if (lines.All(listItemRegex.IsMatch))
            {
                List list = new List();
                foreach (string line in lines)
                {
                    Match match = listItemRegex.Match(line);
                    Paragraph paragraph = new Paragraph();
                    paragraph.Inlines.AddRange(GetInlines(match.Groups["text"].Value));
                    ListItem listItem = new ListItem(paragraph);
                    list.ListItems.Add(listItem);
                }
                document.Blocks.Add(list);
                return true;
            }
            return false;
        }

        private static void AddParagraph(FlowDocument document, IList<string> lines)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.AddRange(GetInlines(string.Join(" ", lines)));
            document.Blocks.Add(paragraph);
        }

        public static FlowDocument Parse(string text)
        {
            FlowDocument document = new FlowDocument();
            foreach (IList<string> lines in Split(text))
            {
                if (TryAddHeading(document, lines, 1))
                {
                    continue;
                }
                if (TryAddHeading(document, lines, 2))
                {
                    continue;
                }
                if (TryAddList(document, lines))
                {
                    continue;
                }
                AddParagraph(document, lines);
            }
            return document;
        }
    }
}
