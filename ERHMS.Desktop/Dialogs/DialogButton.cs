namespace ERHMS.Desktop.Dialogs
{
    public class DialogButton
    {
        public bool? Result { get; }
        public string Text { get; }
        public bool IsDefault { get; }
        public bool IsCancel { get; }

        public DialogButton(bool? result, string text, bool isDefault, bool isCancel)
        {
            Result = result;
            Text = text;
            IsDefault = isDefault;
            IsCancel = isCancel;
        }
    }
}
