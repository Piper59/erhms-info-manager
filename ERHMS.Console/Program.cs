using Epi;
using ERHMS.Console.Utilities;
using ERHMS.EpiInfo;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Linq;

namespace ERHMS.Console
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.Error.WriteLine(Help.GetUsage());
                return;
            }
            ConfigureLog();
            try
            {
                ConfigureEpiInfo();
                IUtility utility = Utility.Create(args[0], args.Skip(1).ToList());
                utility.Run();
            }
            catch (Exception ex)
            {
                Log.Default.Fatal(ex.Message);
                Log.Default.Debug(ex.StackTrace);
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
            if (ConfigurationExtensions.Exists())
            {
                ConfigurationExtensions.Load();
            }
            else
            {
                string path = Path.GetTempFileName();
                Configuration configuration = ConfigurationExtensions.Create(path);
                configuration.Save();
                ConfigurationExtensions.Load(path);
            }
            Configuration.Environment = ExecutionEnvironment.Console;
        }
    }
}
