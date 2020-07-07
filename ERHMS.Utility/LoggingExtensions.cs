using log4net;
using log4net.Appender;
using log4net.Repository;
using System.Linq;
using System.Security.Principal;

namespace ERHMS.Utility
{
    public static class LoggingExtensions
    {
        static LoggingExtensions()
        {
            try
            {
                GlobalContext.Properties["user"] = WindowsIdentity.GetCurrent().Name;
            }
            catch { }
        }

        public static ILog GetLog()
        {
            return LogManager.GetLogger(nameof(ERHMS));
        }

        public static string GetFile(this ILoggerRepository @this)
        {
            return @this.GetAppenders()
                .OfType<FileAppender>()
                .FirstOrDefault()
                ?.File;
        }
    }
}
