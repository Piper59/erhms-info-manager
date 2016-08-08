using ERHMS.Utility;
using System.Reflection;

namespace ERHMS.Presentation.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public string AppTitle { get; private set; }
        public string Version { get; private set; }
        public string InformationalVersion { get; private set; }

        public AboutViewModel()
        {
            Title = "About";
            AppTitle = string.Format("{0}™", App.Title);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetVersion();
            InformationalVersion = assembly.GetInformationalVersion();
        }
    }
}
