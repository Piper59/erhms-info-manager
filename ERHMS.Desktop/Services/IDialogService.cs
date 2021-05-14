using ERHMS.Desktop.Dialogs;

namespace ERHMS.Desktop.Services
{
    public interface IDialogService
    {
        DialogSeverity Severity { get; set; }
        string Lead { get; set; }
        string Body { get; set; }
        string Details { get; set; }
        DialogButtonCollection Buttons { get; set; }

        bool? Show();
    }
}
