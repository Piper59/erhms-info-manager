namespace ERHMS.Desktop.Dialogs
{
    public class DialogButton
    {
        public object Content { get; }
        public bool? Result { get; }
        public bool IsDefault { get; }
        public bool IsCancel { get; }

        public DialogButton(object content, bool? result, bool isDefault, bool isCancel)
        {
            Content = content;
            Result = result;
            IsDefault = isDefault;
            IsCancel = isCancel;
        }
    }
}
