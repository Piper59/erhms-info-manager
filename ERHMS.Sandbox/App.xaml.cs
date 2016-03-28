using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.Utility;
using log4net.Core;
using System;
using System.Reflection;
using System.ServiceModel;
using System.Windows;

namespace ERHMS.Sandbox
{
    public partial class App : Application
    {
        public new static App Current
        {
            get { return (App)Application.Current; }
        }

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

        private ServiceHost host;

        public Service Service { get; private set; }

        public App()
        {
            Log.Level = Level.Debug;
            Log.Current.Debug("Starting up");
            ConfigurationExtensions.CreateAndOrLoad();
            Service = new Service();
            host = Service.OpenHost();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Log.Current.Debug("Exiting");
            if (host != null)
            {
                host.Close();
            }
        }
    }
}
