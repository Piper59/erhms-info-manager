using ERHMS.Desktop.Properties;
using System.Collections.Generic;

namespace ERHMS.Desktop.Dialogs
{
    public class DialogButtonCollection : List<DialogButton>
    {
        public static DialogButtonCollection Close => new DialogButtonCollection
        {
            { Strings.AccessText_Close, null, true, true }
        };

        public static DialogButtonCollection ActionOrCancel(object action)
        {
            return new DialogButtonCollection
            {
                { action, true, true, false },
                { Strings.AccessText_Cancel, false, false, true }
            };
        }

        public void Add(object content, bool? result, bool isDefault, bool isCancel)
        {
            Add(new DialogButton(content, result, isDefault, isCancel));
        }
    }
}
