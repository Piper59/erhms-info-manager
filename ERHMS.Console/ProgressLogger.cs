using System;

namespace ERHMS.Console
{
    public class ProgressLogger : IProgress<string>
    {
        public void Report(string value)
        {
            Log.Default.Debug(value);
        }
    }
}
