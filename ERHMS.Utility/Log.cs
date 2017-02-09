using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class Log
    {
        private static string name;
        private static string fileName;
        private static DirectoryInfo directory;
        private static Hierarchy hierarchy;
        private static RollingFileAppender appender;

        public static ILog Current
        {
            get { return LogManager.GetLogger(name); }
        }

        static Log()
        {
            name = Assembly.GetEntryAssembly().GetName().Name;
            fileName = string.Format("{0}.txt", name);
            directory = GetDefaultDirectory();
            hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.Level = hierarchy.LevelMap[Settings.Default.LogLevel];
            Resume();
        }

        public static FileInfo GetFile()
        {
            return directory.GetFile(fileName);
        }

        public static DirectoryInfo GetDefaultDirectory()
        {
            return AssemblyExtensions.GetEntryDirectory().GetDirectory("Logs");
        }

        public static DirectoryInfo GetDirectory()
        {
            return new DirectoryInfo(directory.FullName);
        }

        public static void SetDirectory(DirectoryInfo directory)
        {
            Log.directory = directory;
            if (hierarchy.Configured)
            {
                appender.File = GetFile().FullName;
                appender.ActivateOptions();
            }
        }

        public static void Resume()
        {
            if (!hierarchy.Configured)
            {
                PatternLayout layout = new PatternLayout("%date %-5level - %message%newline");
                layout.ActivateOptions();
                appender = new RollingFileAppender
                {
                    File = GetFile().FullName,
                    RollingStyle = RollingFileAppender.RollingMode.Date,
                    DatePattern = ".yyyy-MM",
                    MaxSizeRollBackups = -1,
                    Layout = layout
                };
                appender.ActivateOptions();
                hierarchy.Root.AddAppender(appender);
                hierarchy.Configured = true;
            }
        }

        public static void Suspend()
        {
            if (hierarchy.Configured)
            {
                hierarchy.ResetConfiguration();
            }
        }

        public static string GetLevelName()
        {
            return hierarchy.Root.Level.Name;
        }

        public static void SetLevelName(string levelName)
        {
            hierarchy.Root.Level = hierarchy.LevelMap[levelName];
            hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
        }
    }
}
