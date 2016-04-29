using System;

namespace ERHMS.Presentation.Messages
{
    public class ConfirmMessage
    {
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string AffirmativeButtonText { get; private set; }
        public string NegativeButtonText { get; private set; }

        public ConfirmMessage(string title, string message, string affirmativeButtonText, string negativeButtonText)
        {
            Title = title;
            Message = message;
            AffirmativeButtonText = affirmativeButtonText;
            NegativeButtonText = negativeButtonText;
        }

        public event EventHandler Confirmed;
        public void OnConfirmed(EventArgs e)
        {
            EventHandler handler = Confirmed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public void OnConfirmed()
        {
            OnConfirmed(EventArgs.Empty);
        }
    }
}
