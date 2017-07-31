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
    }
}
