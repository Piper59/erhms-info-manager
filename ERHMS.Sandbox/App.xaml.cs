using Epi;
using System.IO;
using System.Windows;

namespace ERHMS.Sandbox
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!File.Exists(Configuration.DefaultConfigurationPath))
            {
                Configuration configuration = Configuration.CreateDefaultConfiguration();
                Configuration.Save(configuration);
            }
            Configuration.Load(Configuration.DefaultConfigurationPath);
        }
    }
}
