using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class DialogService : IDialogService
    {
        public DialogSeverity Severity { get; set; }
        public string Lead { get; set; }
        public string Body { get; set; }
        public string Details { get; set; }
        public DialogButtonCollection Buttons { get; set; }

        public bool? Show()
        {
            DialogViewModel dataContext = new DialogViewModel(Severity, Lead, Body, Details, Buttons);
            Window owner = Application.Current.GetActiveWindow();
            Window window = new DialogView
            {
                DataContext = dataContext
            };
            window.SetOwner(owner);
            window.ShowDialog();
            return dataContext.Result;
        }
    }
}
