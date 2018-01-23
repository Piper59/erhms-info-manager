using System;

namespace ERHMS.Presentation
{
    public class RefreshingEventArgs : EventArgs
    {
        public Type Type { get; private set; }

        public RefreshingEventArgs(Type type)
        {
            Type = type;
        }
    }
}
