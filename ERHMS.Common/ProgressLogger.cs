using System;

namespace ERHMS.Common
{
    public class ProgressLogger : IProgress<string>
    {
        public void Report(string value)
        {
            Log.Default.Debug(value);
        }
    }
}
