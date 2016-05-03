using ERHMS.Utility;
using System.Reflection;

namespace ERHMS.Presentation.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public string Version { get; private set; }
        public string InformationalVersion { get; private set; }

        public AboutViewModel()
        {
            Title = "About";
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetVersion();
            InformationalVersion = assembly.GetInformationalVersion();
        }
    }
}
