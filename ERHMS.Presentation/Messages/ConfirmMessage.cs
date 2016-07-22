using System;

namespace ERHMS.Presentation.Messages
{
    public class ConfirmMessage
    {
        public string Title { get; private set; }
        public string Verb { get; private set; }
        public string Message { get; private set; }

        public ConfirmMessage(string title, string verb, string message)
        {
            Title = title;
            Verb = verb;
            Message = message;
        }

        public ConfirmMessage(string verb, string message)
            : this(string.Format("{0}?", verb), verb, message) { }

        public event EventHandler Confirmed;
        public void OnConfirmed(EventArgs e)
        {
            Confirmed?.Invoke(this, e);
        }
        public void OnConfirmed()
        {
            OnConfirmed(EventArgs.Empty);
        }

        public event EventHandler Canceled;
        public void OnCanceled(EventArgs e)
        {
            Canceled?.Invoke(this, e);
        }
        public void OnCanceled()
        {
            OnCanceled(EventArgs.Empty);
        }
    }
}
