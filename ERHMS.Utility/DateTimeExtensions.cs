using System;

namespace ERHMS.Utility
{
    public static class DateTimeExtensions
    {
        public static DateTime RemoveMilliseconds(this DateTime @this)
        {
            return new DateTime(@this.Year, @this.Month, @this.Day, @this.Hour, @this.Minute, @this.Second, @this.Kind);
        }
    }
}
