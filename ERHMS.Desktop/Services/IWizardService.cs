using ERHMS.Desktop.Wizards;

namespace ERHMS.Desktop.Services
{
    public interface IWizardService
    {
        bool? Show(IWizard wizard);
    }
}
