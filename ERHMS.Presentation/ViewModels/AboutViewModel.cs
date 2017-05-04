using ERHMS.Presentation.Infrastructure;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Reflection;

namespace ERHMS.Presentation.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public string AppTitle { get; private set; }
        public string Version { get; private set; }
        public string InformationalVersion { get; private set; }

        public RelayCommand PrintCommand { get; private set; }

        public AboutViewModel()
        {
            Title = "About";
            AppTitle = App.Title;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetName().Version.ToString();
            InformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            PrintCommand = new RelayCommand(Print);
        }

        public void Print()
        {
            string title = string.Format("{0} License", App.Title);
            string content = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Presentation.LICENSE.txt");
            PrintExtensions.PrintPlainText(title, content);
        }
    }
}
