using System.Threading.Tasks;

namespace ERHMS.Desktop.Wizards
{
    public interface IStep
    {
        IWizard Wizard { get; }

        void Return();
        Task ContinueAsync();
        void Cancel();
    }
}
