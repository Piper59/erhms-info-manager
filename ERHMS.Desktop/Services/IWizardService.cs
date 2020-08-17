using ERHMS.Desktop.Wizards;

namespace ERHMS.Desktop.Services
{
    public interface IWizardService
    {
        bool? Run(IWizard wizard);
    }
}
