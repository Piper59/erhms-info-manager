using log4net;

namespace ERHMS.Data
{
    internal static class Log
    {
        public static ILog Default { get; } = LogManager.GetLogger(nameof(ERHMS));
    }
}
