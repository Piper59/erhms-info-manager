using System;

namespace ERHMS.Presentation.Messages
{
    public class ConfirmMessage
    {
        public string Verb { get; private set; }
        public string Message { get; private set; }

        public ConfirmMessage(string verb, string message)
        {
            Verb = verb;
            Message = message;
        }

        public event EventHandler Confirmed;
        public void OnConfirmed(EventArgs e)
        {
            Confirmed?.Invoke(this, e);
        }
        public void OnConfirmed()
        {
            OnConfirmed(EventArgs.Empty);
        }
    }
}
