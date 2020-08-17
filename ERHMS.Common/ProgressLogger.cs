using System;

namespace ERHMS.Common
{
    public class ProgressLogger : IProgress<string>
    {
        private readonly IProgress<string> @base;

        public ProgressLogger(IProgress<string> @base = null)
        {
            this.@base = @base;
        }

        public void Report(string value)
        {
            Log.Default.Debug(value);
            @base?.Report(value);
        }
    }
}
