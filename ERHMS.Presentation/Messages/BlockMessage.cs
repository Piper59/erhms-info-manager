using System;

namespace ERHMS.Presentation.Messages
{
    public class BlockMessage
    {
        public string Message { get; private set; }

        public BlockMessage(string message)
        {
            Message = message;
        }

        public event EventHandler Executing;
        public void OnExecuting(EventArgs e)
        {
            Executing?.Invoke(this, e);
        }
        public void OnExecuting()
        {
            OnExecuting(EventArgs.Empty);
        }
    }
}
