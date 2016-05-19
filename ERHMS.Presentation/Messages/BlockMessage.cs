using System;

namespace ERHMS.Presentation.Messages
{
    public class BlockMessage
    {
        public string Title { get; private set; }
        public string Message { get; private set; }

        public BlockMessage(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public BlockMessage(string message)
            : this("Working \u2026", message)
        { }

        public event EventHandler Executing;
        public void OnExecuting(EventArgs e)
        {
            Executing?.Invoke(this, e);
        }
        public void OnExecuting()
        {
            OnExecuting(EventArgs.Empty);
        }

        public event EventHandler Executed;
        public void OnExecuted(EventArgs e)
        {
            Executed?.Invoke(this, e);
        }
        public void OnExecuted()
        {
            OnExecuted(EventArgs.Empty);
        }
    }
}
