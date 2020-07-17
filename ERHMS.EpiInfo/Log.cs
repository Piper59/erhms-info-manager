using log4net;

namespace ERHMS.EpiInfo
{
    internal static class Log
    {
        public static ILog Default { get; } = LogManager.GetLogger(nameof(ERHMS));
    }
}
