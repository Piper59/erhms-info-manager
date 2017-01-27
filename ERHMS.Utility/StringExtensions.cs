using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ERHMS.Utility
{
    public static class StringExtensions
    {
        private static readonly Regex NonPrintable = new Regex(@"[^\u0020-\u007e]");

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
            foreach (string item in @this)
            {
                if (string.Equals(item, value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static string ToPrintable(this string @this)
        {
            return NonPrintable.Replace(@this, "");
        }
    }
}
