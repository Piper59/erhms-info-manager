using log4net.Layout;
using System;
using System.IO;

namespace ERHMS.Common.Logging
{
    public class FileAppender : log4net.Appender.FileAppender
    {
        public static string Directory { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        public static string Extension => ".txt";

        private readonly PatternLayout layout;

        public FileAppender()
        {
            File = Path.Combine(Directory, $"ERHMS.{DateTime.Now:yyyy-MM-dd}{Extension}");
            LockingModel = new InterProcessLock();
            layout = new PatternLayout(string.Join(
                " | ",
                "%date",
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
