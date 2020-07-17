using log4net;

namespace ERHMS.Desktop
{
    internal static class Log
    {
        public static ILog Default { get; } = LogManager.GetLogger(nameof(ERHMS));
    }
}
