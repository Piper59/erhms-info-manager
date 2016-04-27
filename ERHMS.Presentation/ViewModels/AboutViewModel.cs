using ERHMS.Utility;
using System.Reflection;

namespace ERHMS.Presentation.ViewModels
{
    public class AboutViewModel : DocumentViewModel
    {
        private Assembly assembly = Assembly.GetExecutingAssembly();

        public override string Title
        {
            get { return "About"; }
        }

        public string Version
        {
            get { return assembly.GetVersion(); }
        }

        public string InformationalVersion
        {
            get { return assembly.GetInformationalVersion(); }
        }
    }
}
