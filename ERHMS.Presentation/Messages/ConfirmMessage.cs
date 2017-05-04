using System;

namespace ERHMS.Presentation.Messages
{
    public class ConfirmMessage
    {
        private string title;

        public string Title
        {
            get { return title ?? string.Format("{0}?", Verb); }
            set { title = value; }
        }

        public string Verb { get; set; }
        public string Message { get; set; }

        public event EventHandler Confirmed;
        public event EventHandler Canceled;

        public void OnConfirmed()
        {
            Confirmed?.Invoke(this, EventArgs.Empty);
        }

        public void OnCanceled()
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }
    }
}
