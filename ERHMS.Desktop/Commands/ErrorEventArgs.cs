using System;

namespace ERHMS.Desktop.Commands
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public bool Handled { get; set; }

        public ErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
