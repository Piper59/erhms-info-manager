using System;

namespace ERHMS.Presentation.Messages
{
    public class ConfirmMessage
    {
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string AffirmativeButtonText { get; private set; }
        public string NegativeButtonText { get; private set; }
        public bool Async { get; private set; }

        public ConfirmMessage(string title, string message, string affirmativeButtonText, string negativeButtonText, bool async = false)
        {
            Title = title;
            Message = message;
            AffirmativeButtonText = affirmativeButtonText;
            NegativeButtonText = negativeButtonText;
            Async = async;
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
