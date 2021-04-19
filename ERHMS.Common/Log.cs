using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using ConsoleAppender = ERHMS.Common.Logging.ConsoleAppender;
using FileAppender = ERHMS.Common.Logging.FileAppender;

namespace ERHMS.Common
{
    public static class Log
    {
        public static class Appenders
        {
            public static IAppender File { get; } = new FileAppender(FilePath);
            public static IAppender Console { get; } = new ConsoleAppender();
        }

        private class ProgressImpl : IProgress<string>
        {
            public void Report(string value)
            {
                Instance.Debug(value);
            }
        }

        public static string DirectoryPath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        public static string FilePath { get; }
            = Path.Combine(DirectoryPath, $"{nameof(ERHMS)}.{DateTime.Today:yyyy-MM-dd}.txt");

        public static ILog Instance => LogManager.GetLogger(typeof(Log));
        public static IProgress<string> Progress { get; } = new ProgressImpl();

        static Log()
        {
            try
            {
                GlobalContext.Properties["user"] = WindowsIdentity.GetCurrent().Name;
            }
            catch { }
            GlobalContext.Properties["process"] = Process.GetCurrentProcess().Id;
        }

        public static void Configure(params IAppender[] appenders)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            foreach (IAppender appender in appenders)
            {
                hierarchy.Root.AddAppender(appender);
            }
            hierarchy.Configured = true;
        }
    }
}
