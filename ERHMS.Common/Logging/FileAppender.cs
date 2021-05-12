using log4net.Layout;
using System;
using System.IO;

namespace ERHMS.Common.Logging
{
    public class FileAppender : log4net.Appender.FileAppender
    {
        private readonly PatternLayout layout;

        public FileAppender()
        {
            File = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Logs",
                $"ERHMS.{DateTime.Now:yyyy-MM-dd}.txt");
            LockingModel = new InterProcessLock();
            layout = new PatternLayout(string.Join(
                " | ",
                "%timestamp",
                "%property{user}",
                "%property{process}(%thread)",
                "%level",
                "%message%newline"));
            Layout = layout;
        }

        public override void ActivateOptions()
        {
            layout.ActivateOptions();
            base.ActivateOptions();
        }
    }
}
