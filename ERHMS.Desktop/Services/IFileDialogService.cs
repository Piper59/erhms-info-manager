using System.ComponentModel;

namespace ERHMS.Desktop.Services
{
    public interface IFileDialogService
    {
        string InitialDirectory { get; set; }
        string InitialFileName { get; set; }
        string FileName { get; set; }
        string Filter { get; set; }

        event CancelEventHandler FileOk;

        bool? Open();
        bool? Save();
    }
}
