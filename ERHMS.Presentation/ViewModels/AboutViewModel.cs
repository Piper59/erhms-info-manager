using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Reflection;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public string AppTitle { get; private set; }
        public string Version { get; private set; }
        public string BuildDate { get; private set; }

        private RelayCommand printCommand;
        public ICommand PrintCommand
        {
            get { return printCommand ?? (printCommand = new RelayCommand(Print)); }
        }

        public AboutViewModel(IServiceManager services)
            : base(services)
        {
            Title = "About";
            AppTitle = App.Title;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetName().Version.ToString();
            BuildDate = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public void Print()
        {
            PrintExtensions.PrintPlainText(
                string.Format("{0} License", App.Title),
                Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Presentation.LICENSE.txt"));
        }
    }
}
