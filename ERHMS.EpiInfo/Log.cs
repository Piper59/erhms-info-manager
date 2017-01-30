using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Reflection;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.EpiInfo
{
    public static class Log
    {
        private static RollingFileAppender appender;

        private static string Name
        {
            get { return Assembly.GetEntryAssembly().GetName().Name; }
        }

        private static string FileName
        {
            get { return string.Format("{0}.txt", Name); }
        }

        private static Hierarchy Hierarchy
        {
            get { return (Hierarchy)LogManager.GetRepository(); }
        }

        public static Level Level
        {
            get
            {
                return Hierarchy.Root.Level;
            }
            private set
            {
                Hierarchy.Root.Level = value;
                Hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
            }
        }

        public static string LevelName
        {
            get { return Level.Name; }
            set { Level = Hierarchy.LevelMap[value]; }
        }

        public static ILog Current
        {
            get { return LogManager.GetLogger(Name); }
        }

        static Log()
        {
            PatternLayout layout = new PatternLayout("%date %-5level - %message%newline");
            layout.ActivateOptions();
            appender = new RollingFileAppender
            {
                File = Path.Combine("Logs", FileName),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = ".yyyy-MM",
                MaxSizeRollBackups = -1,
                Layout = layout
            };
            appender.ActivateOptions();
            Hierarchy.Root.AddAppender(appender);
            LevelName = Settings.Default.LogLevel;
            Hierarchy.Configured = true;
        }

        public static void SetDirectory(DirectoryInfo directory)
        {
            appender.File = Path.Combine(directory.FullName, FileName);
            appender.ActivateOptions();
        }
    }
}
