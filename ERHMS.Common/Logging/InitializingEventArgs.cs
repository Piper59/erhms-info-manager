using log4net.Repository.Hierarchy;
using System;

namespace ERHMS.Common.Logging
{
    public class InitializingEventArgs : EventArgs
    {
        public Hierarchy Hierarchy { get; }

        public InitializingEventArgs(Hierarchy hierarchy)
        {
            Hierarchy = hierarchy;
        }
    }
}
