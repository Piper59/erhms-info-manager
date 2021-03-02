using log4net;
using System;

namespace ERHMS.Common
{
    public static class Log
    {
        private class ProgressImpl : IProgress<string>
        {
            public void Report(string value)
            {
                Default.Debug(value);
            }
        }

        public static ILog Default => LogManager.GetLogger(nameof(ERHMS));
        public static IProgress<string> Progress { get; } = new ProgressImpl();
    }
}
