using ERHMS.EpiInfo;
using ERHMS.Utility;
using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Windows;

namespace ERHMS.Sandbox
{
    public partial class App : Application
    {
        public static string Name
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name; }
        }

        public static string Title
        {
            get { return "ERHMS Info Manager"; }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                new SingleInstanceApplication(() =>
                {
                    App app = new App();
                    app.InitializeComponent();
                    app.Run(new MainWindow());
                }).Execute();
            }
            catch (TimeoutException)
            {
                MessageBox.Show(string.Format("An instance of {0} is already running.", Title), Title);
            }
        }

        private Service service;
        private ServiceHost host = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Log.Current.Debug("Starting up");
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            DirectoryInfo root = new DirectoryInfo(Path.Combine(desktopPath, Name));
            ConfigurationExtensions.CreateAndOrLoad(root);
            service = new Service();
            service.SayingHello += (sender, _e) =>
            {
                MessageBox.Show(string.Format("Hello, {0}", _e.Name), Title);
            };
            try
            {
                host = service.OpenHost();
            }
            catch { }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Current.Debug("Exiting");
            if (host != null)
            {
                host.Close();
            }
            base.OnExit(e);
        }
    }
}
