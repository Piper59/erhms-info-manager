using log4net;

namespace ERHMS.Console
{
    internal static class Log
    {
        public static ILog Default => LogManager.GetLogger(nameof(ERHMS));
    }
}
