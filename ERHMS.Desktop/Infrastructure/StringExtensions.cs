using System;

namespace ERHMS.Desktop.Infrastructure
{
    public static class StringExtensions
    {
        public static bool Search(this string @this, string value)
        {
            return @this.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
        }
    }
}
