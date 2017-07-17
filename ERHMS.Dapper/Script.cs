using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERHMS.Dapper
{
    public class Script : IEnumerable<string>
    {
        private static readonly Regex CommentPattern = new Regex(@"/\*.*?\*/", RegexOptions.Singleline);

        private static IEnumerable<string> Split(string text)
        {
            bool quoted = false;
            int start = 0;
            for (int index = 0; index < text.Length; index++)
            {
                switch (text[index])
                {
                    case '\'':
                        quoted = !quoted;
                        break;
                    case ';':
                        if (!quoted)
                        {
                            yield return text.Substring(start, index - start);
                            start = index + 1;
                        }
                        break;
                }
            }
            if (start < text.Length)
            {
                yield return text.Substring(start);
            }
        }

        private IList<string> sqls;

        public string Text { get; private set; }

        public Script(string text)
        {
            Text = text;
            sqls = Split(CommentPattern.Replace(text, "")).Select(sql => sql.Trim()).ToList();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return sqls.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
