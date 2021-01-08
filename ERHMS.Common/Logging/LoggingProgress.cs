using System;

namespace ERHMS.Common.Logging
{
    public class LoggingProgress : IProgress<string>
    {
        public IProgress<string> UnderlyingProgress { get; }

        public LoggingProgress(IProgress<string> progress = null)
        {
            UnderlyingProgress = progress;
        }

        public void Report(string value)
        {
            Log.Instance.Debug(value);
            UnderlyingProgress?.Report(value);
        }
    }
}
