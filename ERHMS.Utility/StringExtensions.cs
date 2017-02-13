using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ERHMS.Utility
{
    public static class StringExtensions
    {
        private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

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

        public static string Strip(this string @this, Regex regex)
        {
            return regex.Replace(@this, "");
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
