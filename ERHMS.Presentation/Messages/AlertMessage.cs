using System;

namespace ERHMS.Presentation.Messages
{
    public class AlertMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public AlertMessage()
        {
            Title = "Error";
        }

        public event EventHandler Dismissed;

        public void OnDismissed()
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }
    }
}
