using System;

namespace ERHMS.Presentation.Messages
{
    public class NotifyMessage
    {
        public string Title { get; private set; }
        public string Message { get; private set; }

        public NotifyMessage(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public NotifyMessage(string message)
            : this("Error", message) { }

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
