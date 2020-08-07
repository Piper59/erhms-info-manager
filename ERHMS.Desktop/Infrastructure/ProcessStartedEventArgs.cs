using System;
using System.Diagnostics;

namespace ERHMS.Desktop.Infrastructure
{
    public class ProcessStartedEventArgs : EventArgs
    {
        public Process Process { get; }

        public ProcessStartedEventArgs(Process process)
        {
            Process = process;
        }
    }
}
