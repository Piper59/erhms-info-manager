using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Collections.Generic;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class DialogService : IDialogService
    {
        public DialogSeverity Severity { get; set; }
        public string Lead { get; set; }
        public string Body { get; set; }
        public string Details { get; set; }
        public IReadOnlyCollection<DialogButton> Buttons { get; set; }

        public bool? Show()
        {
            Window owner = Application.Current.MainWindow;
            Window window = new DialogView
            {
                DataContext = new DialogViewModel(Severity, Lead, Body, Details, Buttons)
            };
            window.SetOwner(owner);
            return window.ShowDialog();
        }
    }
}
