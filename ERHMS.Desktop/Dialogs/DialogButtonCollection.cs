using System.Collections.Generic;

namespace ERHMS.Desktop.Dialogs
{
    public class DialogButtonCollection : List<DialogButton>
    {
        public static DialogButtonCollection Ok => new DialogButtonCollection
        {
            { "_OK", null, true, true }
        };

        public static DialogButtonCollection Close => new DialogButtonCollection
        {
            { "_Close", null, true, true }
        };

        public void Add(object content, bool? result, bool isDefault, bool isCancel)
        {
            Add(new DialogButton(content, result, isDefault, isCancel));
        }
    }
}
