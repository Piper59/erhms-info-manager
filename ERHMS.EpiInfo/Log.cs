using log4net;

namespace ERHMS.EpiInfo
{
    internal static class Log
    {
        public static ILog Default => LogManager.GetLogger(nameof(ERHMS));
    }
}
