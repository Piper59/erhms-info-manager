using log4net;
using log4net.Appender;
using log4net.Repository;
using System.Linq;
using System.Security.Principal;

namespace ERHMS.Utility
{
    public static class Log
    {
        static Log()
        {
            try
            {
                GlobalContext.Properties["user"] = WindowsIdentity.GetCurrent().Name;
            }
            catch { }
        }

        public static ILog Default { get; } = LogManager.GetLogger(nameof(ERHMS));

        public static string GetFile(this ILoggerRepository @this)
        {
            return @this.GetAppenders()
                .OfType<FileAppender>()
                .FirstOrDefault()
                ?.File;
        }
    }
}
