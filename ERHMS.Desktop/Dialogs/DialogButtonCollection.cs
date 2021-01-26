using System.Collections.Generic;

namespace ERHMS.Desktop.Dialogs
{
    public class DialogButtonCollection : List<DialogButton>
    {
        public void Add(object content, bool? result, bool isDefault, bool isCancel)
        {
            Add(new DialogButton(content, result, isDefault, isCancel));
        }
    }
}
