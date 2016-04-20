using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.Utility;
using log4net.Core;
using System;
using System.Reflection;
using System.ServiceModel;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

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
                    Log.Level = Level.Debug;
                    Log.Current.Debug("Starting up");
                    if (Settings.Default.DoFirstTimeSetup)
                    {
                        Log.Current.Debug("Doing first-time setup");
                        using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                        {
                            dialog.Description = "Choose a location for your ERHMS documents.";
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                Log.Current.DebugFormat("Setting root directory: {0}", dialog.SelectedPath);
                                Settings.Default.RootDirectory = dialog.SelectedPath;
                                Settings.Default.DoFirstTimeSetup = false;
                                Settings.Default.Save();
                            }
                            else
                            {
                                Log.Current.Fatal("First-time setup failed");
                                return;
                            }
                        }
                    }
                    ConfigurationExtensions.CreateAndOrLoad();
                    App app = new App();
                    app.InitializeComponent();
                    app.Run(new MainWindow());
                    Log.Current.Debug("Exiting");
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
            Service = new Service();
            host = Service.OpenHost();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (host != null)
            {
                host.Close();
            }
        }
    }
}
