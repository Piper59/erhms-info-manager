using Epi;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Reflection;

namespace ERHMS.EpiInfo
{
    public static class Log
    {
        private static string name = Assembly.GetEntryAssembly().GetName().Name;
        private static string fileName = string.Format("{0}.txt", name);
        private static RollingFileAppender appender;

        public static ILog Current
        {
            get { return LogManager.GetLogger(name); }
        }

        public static Level Level
        {
            get { return Current.Logger.Repository.Threshold; }
            set { Current.Logger.Repository.Threshold = value; }
        }

        static Log()
        {
            PatternLayout layout = new PatternLayout("%date %-5level - %message%newline");
            layout.ActivateOptions();
            appender = new RollingFileAppender
            {
                File = Path.Combine("Logs", fileName),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = ".yyyy-MM",
                MaxSizeRollBackups = -1,
                Layout = layout
            };
            appender.ActivateOptions();
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(appender);
#if DEBUG
            hierarchy.Root.Level = Level.Debug;
#else
            hierarchy.Root.Level = Level.Warn;
#endif
            hierarchy.Configured = true;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string message = "Fatal error";
            Exception ex = e.ExceptionObject as Exception;
            if (ex == null)
            {
                Current.Fatal(message);
            }
            else
            {
                Current.Fatal(message, ex);
            }
        }

        public static void Configure(Configuration configuration)
        {
            appender.File = Path.Combine(configuration.Directories.LogDir, fileName);
            appender.ActivateOptions();
        }
    }
}
