using ERHMS.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERHMS.Presentation.Services
{
    public interface IDialogService
    {
        IWin32Window GetOwner();
        Task AlertAsync(string message, string title = null);
        Task AlertAsync(string message, Exception exception);
        Task AlertAsync(ValidationError error, IEnumerable<string> fields);
        Task BlockAsync(string message, Action action);
        Task<bool> ConfirmAsync(string message, string verb = null, string title = null);
        void Notify(string message);
        Task ShowAsync(DialogViewModel model);
        Task<bool> ShowLicenseAsync();
        string OpenFolder();
        string OpenFile(string title = null, string initialDirectory = null, string filter = null);
        IEnumerable<string> OpenFiles(string title = null, string initialDirectory = null, string filter = null);
        string SaveFile(string title = null, string initialDirectory = null, string filter = null, string fileName = null);
        string GetRootPath();
    }
}
