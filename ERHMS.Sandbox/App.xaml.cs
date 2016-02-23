using ERHMS.EpiInfo;
using System.ServiceModel;
using System.Windows;

namespace ERHMS.Sandbox
{
    public partial class App : Application
    {
        private ServiceHost host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigurationExtensions.LoadDefaultConfiguration();
            host = new ServiceHost(typeof(Service));
            host.Open();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            host.Close();
            base.OnExit(e);
        }
    }
}
