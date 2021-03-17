using ERHMS.Desktop.Dialogs;
using System.Collections.Generic;

namespace ERHMS.Desktop.Services
{
    public interface IDialogService
    {
        bool? Show(
            DialogType dialogType,
            string lead,
            string body,
            string details,
            IReadOnlyCollection<DialogButton> buttons);
    }
}
