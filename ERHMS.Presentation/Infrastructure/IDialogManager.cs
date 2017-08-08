using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERHMS.Presentation
{
    public interface IDialogManager
    {
        IWin32Window Win32Window { get; }

        Task InvokeAsync(Wrapper wrapper);
        Task ShowAsync(DialogViewModel dataContext);
        Task ShowErrorAsync(string message, Exception exception);
    }
}
