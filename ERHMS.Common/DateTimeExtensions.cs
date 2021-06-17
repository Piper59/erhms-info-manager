using System;

namespace ERHMS.Common
{
    public static class DateTimeExtensions
    {
        public static DateTime Floor(this DateTime @this)
        {
            return new DateTime(@this.Year, @this.Month, @this.Day, @this.Hour, @this.Minute, @this.Second);
        }
    }
}
