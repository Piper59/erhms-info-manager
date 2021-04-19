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
        private readonly IWindowingService windowing;

        public DialogSeverity Severity { get; set; }
        public string Lead { get; set; }
        public string Body { get; set; }
        public string Details { get; set; }
        public IReadOnlyCollection<DialogButton> Buttons { get; set; }

        public DialogService()
        {
            windowing = ServiceLocator.Resolve<IWindowingService>();
        }

        public bool? Show()
        {
            Window window = new DialogView
            {
                DataContext = new DialogViewModel(Severity, Lead, Body, Details, Buttons)
            };
            return windowing.ShowDialog(window);
        }
    }
}
