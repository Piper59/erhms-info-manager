using Mantin.Controls.Wpf.Notification;

namespace ERHMS.Presentation.Messages
{
    public class ToastMessage
    {
        public NotificationType NotificationType { get; private set; }
        public string Message { get; private set; }

        public ToastMessage(NotificationType notificationType, string message)
        {
            NotificationType = notificationType;
            Message = message;
        }
    }
}
