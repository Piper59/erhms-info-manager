using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System.Reflection;

namespace ERHMS.Presentation.ViewModels
{
    [ContextSafe]
    public class AboutViewModel : DocumentViewModel
    {
        public string Version { get; private set; }
        public string BuildDate { get; private set; }

        public ICommand PrintCommand { get; private set; }

        public AboutViewModel()
        {
            Title = "About";
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetName().Version.ToString();
            BuildDate = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            PrintCommand = new Command(Print);
        }

        public void Print()
        {
            ServiceLocator.Printer.Print(Resources.LicenseTitle, Resources.LICENSE);
        }
    }
}
