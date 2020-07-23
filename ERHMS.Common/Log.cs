using log4net;

namespace ERHMS.Common
{
    internal static class Log
    {
        public static ILog Default => LogManager.GetLogger(nameof(ERHMS));
    }
}
