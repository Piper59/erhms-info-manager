using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class Log
    {
        public const string FileExtension = ".txt";
        public static readonly ICollection<string> LevelNames = new string[]
        {
            "DEBUG",
            "INFO",
            "WARN",
            "ERROR",
            "FATAL"
        };
        public static readonly string FilePath;

        private static string name;
        private static Hierarchy hierarchy;

        public static ILog Logger { get; private set; }

        public static string LevelName
        {
            get
            {
                return hierarchy.Root.Level.Name;
            }
            set
            {
                hierarchy.Root.Level = hierarchy.LevelMap[value];
                hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
            }
        }

        static Log()
        {
            GlobalContext.Properties["process"] = Process.GetCurrentProcess().Id;
            name = Assembly.GetEntryAssembly().GetName().Name;
            FilePath = Path.Combine(AssemblyExtensions.GetEntryDirectoryPath(), "Logs", name + FileExtension);
            hierarchy = (Hierarchy)LogManager.GetRepository();
            PatternLayout layout = new PatternLayout("%date %10property{process} %-5level - %message%newline");
            layout.ActivateOptions();
            TextWriterAppender appender = new FileAppender
            {
                File = FilePath,
                LockingModel = new FileAppender.MinimalLock(),
                Layout = layout
            };
            appender.ActivateOptions();
            hierarchy.Root.AddAppender(appender);
            hierarchy.Configured = true;
            Logger = LogManager.GetLogger(name);
        }
    }
}
