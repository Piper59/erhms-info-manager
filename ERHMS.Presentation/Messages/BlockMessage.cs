using System;

namespace ERHMS.Presentation.Messages
{
    public class BlockMessage
    {
        public string Message { get; private set; }
        public Action Action { get; private set; }

        public BlockMessage(string message, Action action)
        {
            Message = message;
            Action = action;
        }
    }
}
