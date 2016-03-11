using Epi;
using ERHMS.EpiInfo;
using System;
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
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            DirectoryInfo root = new DirectoryInfo(Path.Combine(desktopPath, "ERHMS.Sandbox"));
            if (!ConfigurationExtensions.TryLoad(root))
            {
                Configuration.Save(ConfigurationExtensions.Create(root));
                ConfigurationExtensions.Load(root);
            }
            Log.Current.Debug("Starting up");
            host = new ServiceHost(typeof(Service));
            host.Open();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Current.Debug("Exiting");
            host.Close();
            base.OnExit(e);
        }
    }
}
