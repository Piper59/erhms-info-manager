using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace ERHMS.Common.Logging
{
    public class ConsoleAppender : ColoredConsoleAppender
    {
        public ConsoleAppender()
        {
            Target = ConsoleError;
            PatternLayout layout = new PatternLayout("%message%newline");
            layout.ActivateOptions();
            Layout = layout;
            AddMapping(new LevelColors
            {
                Level = Level.Info,
                ForeColor = Colors.Green
            });
            AddMapping(new LevelColors
            {
                Level = Level.Warn,
                ForeColor = Colors.Yellow
            });
            AddMapping(new LevelColors
            {
                Level = Level.Error,
                ForeColor = Colors.Red
            });
            AddMapping(new LevelColors
            {
                Level = Level.Fatal,
                ForeColor = Colors.Red | Colors.HighIntensity
            });
            ActivateOptions();
        }
    }
}
