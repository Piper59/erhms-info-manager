using log4net;
using log4net.Core;
using System;

namespace ERHMS.Common.Logging
{
    public static class ILogExtensions
    {
        public static void Log(this ILog @this, Level level, object message, Exception exception = null)
        {
            if (@this.Logger.IsEnabledFor(level))
            {
                @this.Logger.Log(@this.Logger.GetType(), level, message, exception);
            }
        }
    }
}
