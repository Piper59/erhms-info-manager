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
        public static bool? Run(this IWizard @this)
        {
            IWindowService window = ServiceLocator.Resolve<IWindowService>();
            window.ShowDialog(@this);
            return @this.Result;
        }
    }
}
