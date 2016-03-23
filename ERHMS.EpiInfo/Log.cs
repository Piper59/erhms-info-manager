using Epi;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Reflection;
using Settings = ERHMS.Utility.Properties.Settings;

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
            hierarchy.Root.Level = GetInitialLevel(hierarchy.LevelMap);
            hierarchy.Configured = true;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static Level GetInitialLevel(LevelMap map)
        {
            Level level = map[Settings.Default.LogLevel];
            if (level == null)
            {
#if DEBUG
                return Level.Debug;
#else
                return Level.Warn;
#endif
            }
            else
            {
                return level;
            }
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
