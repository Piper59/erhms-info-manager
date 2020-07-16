using log4net.Appender;
using log4net.Repository;
using System.Linq;

namespace ERHMS.Desktop.Infrastructure
{
    internal static class LogExtensions
    {
        public static string GetFile(ILoggerRepository @this)
        {
            return @this.GetAppenders()
                .OfType<FileAppender>()
                .FirstOrDefault()
                ?.File;
        }
    }
}
