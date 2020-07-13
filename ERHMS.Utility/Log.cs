using log4net;
using System.Diagnostics;
using System.Security.Principal;

namespace ERHMS.Utility
{
    public static class Log
    {
        static Log()
        {
            try
            {
                GlobalContext.Properties["process"] = Process.GetCurrentProcess().Id;
                GlobalContext.Properties["user"] = WindowsIdentity.GetCurrent().Name;
            }
            catch { }
        }

        public const string LoggerName = nameof(ERHMS);
        public const string MainAppenderName = "Main";

        public static ILog Default { get; } = LogManager.GetLogger(LoggerName);
    }
}
