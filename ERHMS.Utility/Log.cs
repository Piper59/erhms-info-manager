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
        public static readonly string FilePath;

        private static string name;
        private static Hierarchy hierarchy;

        public static ILog Logger
        {
            get { return LogManager.GetLogger(name); }
        }

        static Log()
        {
            name = Assembly.GetEntryAssembly().GetName().Name;
            FilePath = Path.Combine(AssemblyExtensions.GetEntryDirectoryPath(), "Logs", name + ".txt");
            hierarchy = (Hierarchy)LogManager.GetRepository();
            PatternLayout layout = new PatternLayout("%date %-5level - %message%newline");
            layout.ActivateOptions();
            TextWriterAppender appender = new FileAppender
            {
                File = FilePath,
                LockingModel = new FileAppender.InterProcessLock(),
                Layout = layout
            };
            appender.ActivateOptions();
            hierarchy.Root.AddAppender(appender);
            hierarchy.Configured = true;
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
