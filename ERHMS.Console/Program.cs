using Epi;
using ERHMS.Common;
using ERHMS.Console.Utilities;
using ERHMS.EpiInfo;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;

namespace ERHMS.Console
{
    public class Program
    {
        private static int Main(string[] args)
        {
            IUtility utility = Utility.ParseArgs(args);
            ConfigureLog();
            Log.Instance.Info("Running");
            try
            {
                ConfigureEpiInfo();
                utility.Run();
                Log.Instance.Info("Completed");
                return ErrorCodes.Success;
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal(ex.Message);
                Log.Instance.Error(ex);
                Log.Instance.Warn("Completed with errors");
                return ex.HResult;
            }
        }

        private static void ConfigureLog()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            PatternLayout layout = new PatternLayout("%message%newline");
            layout.ActivateOptions();
            ColoredConsoleAppender appender = new ColoredConsoleAppender
            {
                Target = ConsoleAppender.ConsoleError,
                Layout = layout
            };
            appender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Info,
                ForeColor = ColoredConsoleAppender.Colors.Green
            });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Warn,
                ForeColor = ColoredConsoleAppender.Colors.Yellow
            });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Error,
                ForeColor = ColoredConsoleAppender.Colors.Red
            });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Fatal,
                ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity
            });
            appender.ActivateOptions();
            hierarchy.Root.AddAppender(appender);
            hierarchy.Configured = true;
        }

        private static void ConfigureEpiInfo()
        {
            if (!ConfigurationExtensions.Exists())
            {
                Configuration configuration = ConfigurationExtensions.Create();
                configuration.Save();
            }
            ConfigurationExtensions.Load();
            Configuration.Environment = ExecutionEnvironment.Console;
        }
    }
}
