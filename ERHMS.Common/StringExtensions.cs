using System;

namespace ERHMS.Common
{
    public static class StringExtensions
    {
        public static bool StartsWith(this string @this, string value, StringComparer comparer)
        {
            return @this.Length >= value.Length && comparer.Equals(@this.Substring(0, value.Length), value);
        }
    }
}
