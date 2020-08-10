using System;

namespace ERHMS.Desktop.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public bool Handled { get; set; }

        public ErrorEventArgs(Exception ex)
        {
            Exception = ex;
        }
    }
}
