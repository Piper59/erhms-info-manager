using ERHMS.Desktop.Dialogs;

namespace ERHMS.Desktop.Services
{
    public interface IDialogService
    {
        bool? Show(DialogInfo info);
    }
}
