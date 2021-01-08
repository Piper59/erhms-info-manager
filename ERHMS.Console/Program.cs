using Epi;
using ERHMS.Common.Logging;
using ERHMS.Console.Utilities;
using ERHMS.EpiInfo;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using System;
using System.Linq;

namespace ERHMS.Console
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                using (new Highlighter())
                {
                    System.Console.Error.WriteLine(Utility.GetUsage());
                }
                return;
            }
            Log.Initializing += Log_Initializing;
            try
            {
                Configure();
                IUtility utility = Utility.Create(args[0], args.Skip(1).ToList());
                utility.Run();
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal(ex.Message);
                Log.Instance.Debug(ex.StackTrace);
            }
        }

        private static void Log_Initializing(object sender, InitializingEventArgs e)
        {
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
            e.Hierarchy.Root.AddAppender(appender);
        }

        private static void Configure()
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
