using System;

namespace ERHMS.Utility
{
    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string @this, string value)
        {
            return @this.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
