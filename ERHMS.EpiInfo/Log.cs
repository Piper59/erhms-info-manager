using Epi;
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

        public static string Name
        {
            get { return Assembly.GetEntryAssembly().GetName().Name; }
        }

        private static string FileName
        {
            get { return string.Format("{0}.txt", Name); }
        }

        public static ILog Current
        {
            get { return LogManager.GetLogger(Name); }
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
            set
            {
                Hierarchy.Root.Level = value;
                Hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
            }
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
            SetLevelName(Settings.Default.LogLevel);
            Hierarchy.Configured = true;
        }

        public static string GetLevelName()
        {
            return Hierarchy.Root.Level.Name;
        }

        public static void SetLevelName(string levelName)
        {
            Level = Hierarchy.LevelMap[levelName];
        }

        public static void Configure(Configuration configuration)
        {
            appender.File = Path.Combine(configuration.Directories.LogDir, FileName);
            appender.ActivateOptions();
        }

        public static Stream Read()
        {
            return new FileStream(appender.File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
