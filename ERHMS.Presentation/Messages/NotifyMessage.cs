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
    }
}
