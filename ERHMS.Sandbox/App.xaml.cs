using Epi;
using System.IO;
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
            host = new ServiceHost(typeof(Service));
            host.Open();
            if (!File.Exists(Configuration.DefaultConfigurationPath))
            {
                Configuration configuration = Configuration.CreateDefaultConfiguration();
                Configuration.Save(configuration);
            }
            Configuration.Load(Configuration.DefaultConfigurationPath);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            host.Close();
            base.OnExit(e);
        }
    }
}
