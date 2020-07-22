using System.Collections.Generic;

namespace ERHMS.Desktop.Dialogs
{
    public class DialogButtonCollection : List<DialogButton>
    {
        public static DialogButtonCollection OK => new DialogButtonCollection
        {
            new DialogButton(null, "OK", true, true)
        };
        public static DialogButtonCollection Close => new DialogButtonCollection
        {
            new DialogButton(null, "Close", true, true)
        };
    }
}
