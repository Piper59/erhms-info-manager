using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class WizardService : IWizardService
    {
        public bool? Show(object wizard)
        {
            Window owner = Application.Current.MainWindow;
            Window window = new WizardView
            {
                DataContext = (WizardViewModel)wizard
            };
            window.SetOwner(owner);
            return window.ShowDialog();
        }
    }
}
