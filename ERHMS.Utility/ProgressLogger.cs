using System;

namespace ERHMS.Utility
{
    public class ProgressLogger<T> : IProgress<T>
    {
        public void Report(T value)
        {
            Log.Default.Debug(value);
        }
    }
}
