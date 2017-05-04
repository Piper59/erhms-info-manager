using Mantin.Controls.Wpf.Notification;

namespace ERHMS.Presentation.Messages
{
    public class ToastMessage
    {
        public NotificationType Type { get; set; }
        public string Message { get; set; }

        public ToastMessage()
        {
            Type = NotificationType.Information;
        }
    }
}
