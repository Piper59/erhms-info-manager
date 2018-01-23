using System;
using System.Text.RegularExpressions;

namespace ERHMS.Utility
{
    public static class StringExtensions
    {
        private static readonly Regex NewLinePattern = new Regex(@"\r\n|\r(?!\n)|(?<!\r)\n");

        public static bool Contains(this string @this, string value, StringComparison comparisonType)
        {
            return @this.IndexOf(value, comparisonType) >= 0;
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
