using System;

namespace ERHMS.Presentation.Messages
{
    public class NotifyMessage
    {
        public string Title { get; private set; }
        public string Message { get; private set; }
        public bool Async { get; private set; }

        public NotifyMessage(string title, string message, bool async = false)
        {
            Title = title;
            Message = message;
            Async = async;
        }

        public NotifyMessage(string message, bool async = false)
            : this("Error", message, async)
        { }

        public event EventHandler Dismissed;
        public void OnDismissed(EventArgs e)
        {
            Dismissed?.Invoke(this, e);
        }
        public void OnDismissed()
        {
            OnDismissed(EventArgs.Empty);
        }
    }
}
