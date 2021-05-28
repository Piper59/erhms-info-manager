using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.Desktop.Wizards;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class WizardService : IWizardService
    {
        public bool? Show(IWizard wizard)
        {
            WizardViewModel dataContext = (WizardViewModel)wizard;
            Window owner = Application.Current.GetActiveWindow();
            Window window = new WizardView
            {
                DataContext = dataContext
            };
            window.SetOwner(owner);
            window.ShowDialog();
            return wizard.Result;
        }
    }
}
