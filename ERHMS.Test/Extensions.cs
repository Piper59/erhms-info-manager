using System;

namespace ERHMS.Test
{
    public static class Extensions
    {
        public static bool IsRecent(this DateTime @this)
        {
            return (DateTime.Now - @this).TotalMinutes <= 1.0;
        }
    }
}
