using System;
using System.Collections.Generic;

namespace ERHMS.Utility
{
    public static class StringExtensions
    {
        public static bool Contains(this string @this, string value, StringComparison comparisonType)
        {
            return @this.IndexOf(value, comparisonType) >= 0;
        }

        public static bool Contains(this IEnumerable<string> @this, string value, StringComparison comparisonType)
        {
            foreach (string item in @this)
            {
                if (string.Equals(item, value, comparisonType))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool EqualsIgnoreCase(this string @this, string value)
        {
            return string.Equals(@this, value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
