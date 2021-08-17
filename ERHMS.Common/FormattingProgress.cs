using System;

namespace ERHMS.Common
{
    public class FormattingProgress<TValue> : IProgress<TValue>
    {
        public IProgress<string> BaseProgress { get; }
        public string Format { get; }

        public FormattingProgress(IProgress<string> progress, string format)
        {
            BaseProgress = progress;
            Format = format;
        }

        public void Report(TValue value)
        {
            BaseProgress.Report(string.Format(Format, value));
        }
    }
}
