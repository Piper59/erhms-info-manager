using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ERHMS.Utility
{
    public static class StringExtensions
    {
        private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
        private static readonly Regex NewLinePattern = new Regex(@"\r\n|(?<!\r)\n|\r(?!\n)");

        public static bool EqualsIgnoreCase(this string @this, string value)
        {
            return string.Equals(@this, value, IgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string @this, string value)
        {
            return @this.IndexOf(value, IgnoreCase) >= 0;
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> @this, string value)
        {
            foreach (string item in @this)
            {
                if (string.Equals(item, value, IgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static string Strip(this string @this, Regex pattern)
        {
            return pattern.Replace(@this, "");
        }

        public static string NormalizeNewLines(this string @this)
        {
            return NewLinePattern.Replace(@this, Environment.NewLine);
        }

        public static string MakeUnique(this string @this, string format, Predicate<string> exists)
        {
            if (exists(@this))
            {
                for (int count = 2; ; count++)
                {
                    string value = string.Format(format, @this, count);
                    if (!exists(value))
                    {
                        return value;
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
