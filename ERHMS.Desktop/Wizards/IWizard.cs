using ERHMS.Desktop.Services;

namespace ERHMS.Desktop.Wizards
{
    public interface IWizard
    {
        IStep Step { get; }
        bool? Result { get; }
    }

    public static class IWizardExtensions
    {
        public static bool? Show(this IWizard @this)
        {
            return ServiceLocator.Resolve<IWizardService>().Show(@this);
        }
    }
}
