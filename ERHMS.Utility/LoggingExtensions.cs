using log4net.Appender;
using log4net.Repository;
using System.Linq;

namespace ERHMS.Utility
{
    public static class LoggingExtensions
    {
        public static string GetFile(this ILoggerRepository @this, string appenderName)
        {
            return @this.GetAppenders()
                .OfType<FileAppender>()
                .SingleOrDefault(appender => appender.Name == appenderName)
                ?.File;
        }
    }
}
