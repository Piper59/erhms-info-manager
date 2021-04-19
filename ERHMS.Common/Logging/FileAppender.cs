using log4net.Layout;

namespace ERHMS.Common.Logging
{
    public class FileAppender : log4net.Appender.FileAppender
    {
        public FileAppender(string path)
        {
            File = path;
            LockingModel = new InterProcessLock();
            PatternLayout layout =
                new PatternLayout("%date | %property{user} | %property{process}(%thread) | %level | %message%newline");
            layout.ActivateOptions();
            Layout = layout;
            ActivateOptions();
        }
    }
}
