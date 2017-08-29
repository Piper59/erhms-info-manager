using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERHMS.Utility
{
    public static class StringExtensions
    {
        private static readonly Regex NewLinePattern = new Regex(@"\r\n|(?<!\r)\n|\r(?!\n)");

        public static bool EqualsIgnoreCase(this string @this, string value)
        {
            return string.Equals(@this, value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string @this, string value)
        {
            return @this.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> @this, string value)
        {
            return @this.Contains(value, StringComparer.OrdinalIgnoreCase);
        }

        public static string[] SplitLines(this string @this)
        {
            return NewLinePattern.Split(@this);
        }

        public static string MakeUnique(this string @this, string format, Func<string, bool> exists)
        {
            if (exists(@this))
            {
                for (int count = 2; ; count++)
                {
                    string result = string.Format(format, @this, count);
                    if (!exists(result))
                    {
                        return result;
                    }
                }
            }
            else
            {
                return @this;
            }
        }

        // https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
        public static int GetEditDistance(string str1, string str2)
        {
            string a = (str1 ?? "").ToLower();
            string b = (str2 ?? "").ToLower();
            if (a == b)
            {
                return 0;
            }
            int m = a.Length;
            int n = b.Length;
            if (m == 0)
            {
                return n;
            }
            if (n == 0)
            {
                return m;
            }
            int[,] d = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }
            for (int j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    int deletion = d[i - 1, j] + 1;
                    int insertion = d[i, j - 1] + 1;
                    int substitution = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(deletion, insertion), substitution);
                    if (i > 1 && j > 1 && a[i - 1] == b[j - 2] && a[i - 2] == b[j - 1])
                    {
                        int transposition = d[i - 2, j - 2] + cost;
                        d[i, j] = Math.Min(d[i, j], transposition);
                    }
                }
            }
            return d[m, n];
        }
    }
}
