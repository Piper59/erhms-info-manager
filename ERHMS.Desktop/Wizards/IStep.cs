using System.Threading.Tasks;

namespace ERHMS.Desktop.Wizards
{
    public interface IStep
    {
        IWizard Wizard { get; }
        IStep Antecedent { get; }

        void Return();
        Task ContinueAsync();
        void Cancel();
    }
}
