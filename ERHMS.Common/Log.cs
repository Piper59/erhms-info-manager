using log4net;
using System;
using System.IO;

namespace ERHMS.Common
{
    public static class Log
    {
        private class ProgressImpl : IProgress<string>
        {
            public void Report(string value)
            {
                Instance.Debug(value);
            }
        }

        public static string DirectoryPath { get; } =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        public static string FilePath { get; } =
            Path.Combine(DirectoryPath, $"{nameof(ERHMS)}.{DateTime.Today:yyyy-MM-dd}.txt");

        public static ILog Instance => LogManager.GetLogger(nameof(ERHMS));
        public static IProgress<string> Progress { get; } = new ProgressImpl();
    }
}
