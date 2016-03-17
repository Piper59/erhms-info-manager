using Epi;
using ERHMS.EpiInfo;
using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Windows;

namespace ERHMS.Sandbox
{
    public partial class App : Application
    {
        public static string Name
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name; }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            bool created;
            using (Mutex mutex = new Mutex(true, string.Format("Global\\{0}", Name), out created))
            {
                if (!created)
                {
                    MessageBox.Show("An instance of this application is already running.");
                    return;
                }
                App app = new App();
                MainWindow window = new MainWindow();
                app.Run(window);
            }
        }

        private ServiceHost host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Log.Current.Debug("Starting up");
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            DirectoryInfo root = new DirectoryInfo(Path.Combine(desktopPath, Name));
            if (!ConfigurationExtensions.TryLoad(root))
            {
                Configuration.Save(ConfigurationExtensions.Create(root));
                ConfigurationExtensions.Load(root);
            }
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
