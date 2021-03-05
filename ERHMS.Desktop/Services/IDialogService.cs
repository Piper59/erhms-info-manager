using ERHMS.Desktop.Dialogs;

namespace ERHMS.Desktop.Services
{
    public interface IDialogService
    {
        bool? Show(DialogSeverity severity, string lead, string body, string details, DialogButtonCollection buttons);
    }
}
