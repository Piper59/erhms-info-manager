using ERHMS.Desktop.Dialogs;
using System.Collections.Generic;

namespace ERHMS.Desktop.Services
{
    public interface IDialogService
    {
        DialogSeverity Severity { get; set; }
        string Lead { get; set; }
        string Body { get; set; }
        string Details { get; set; }
        IReadOnlyCollection<DialogButton> Buttons { get; set; }

        bool? Show();
    }
}
