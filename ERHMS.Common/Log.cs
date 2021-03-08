using log4net;
using log4net.Appender;
using System;
using System.IO;
using System.Linq;

namespace ERHMS.Common
{
    public static class Log
    {
        private class ProgressImpl : IProgress<string>
        {
            public void Report(string value)
            {
                Default.Debug(value);
            }
        }

        public static ILog Default => LogManager.GetLogger(nameof(ERHMS));
        public static IProgress<string> Progress { get; } = new ProgressImpl();

        public static string GetDefaultFilePath()
        {
            return Default.Logger.Repository.GetAppenders()
                .OfType<FileAppender>()
                .Single()
                .File;
        }

        public static string GetDefaultDirectoryPath()
        {
            return Path.GetDirectoryName(GetDefaultFilePath());
        }
    }
}
