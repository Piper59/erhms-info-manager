using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace ERHMS.Common.Logging
{
    public class ConsoleAppender : ColoredConsoleAppender
    {
        private readonly PatternLayout layout;

        public ConsoleAppender()
        {
            Target = ConsoleError;
            AddMapping(Level.Info, Colors.Green);
            AddMapping(Level.Warn, Colors.Yellow);
            AddMapping(Level.Error, Colors.Red);
            AddMapping(Level.Fatal, Colors.Red | Colors.HighIntensity);
            layout = new PatternLayout("%message%newline");
            Layout = layout;
        }

        private void AddMapping(Level level, Colors foreColor)
        {
            AddMapping(new LevelColors
            {
                Level = level,
                ForeColor = foreColor
            });
        }

        public override void ActivateOptions()
        {
            layout.ActivateOptions();
            base.ActivateOptions();
        }
    }
}
