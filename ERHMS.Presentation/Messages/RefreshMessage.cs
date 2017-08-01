using System;

namespace ERHMS.Presentation.Messages
{
    public class RefreshMessage
    {
        public Type Type { get; set; }

        public RefreshMessage(Type type)
        {
            Type = type;
        }
    }
}
