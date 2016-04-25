using ERHMS.DataAccess;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.Utility;
using System;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ERHMS.Presentation
{
    public partial class App : Application
    {
        public new static App Current
        {
            get { return (App)Application.Current; }
        }

        public static string Title
        {
            get { return "ERHMS Info Manager"; }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstanceExecuter executer = new SingleInstanceExecuter();
            executer.Executing += (sender, e) =>
            {
                Log.Current.Debug("Starting up");
                if (string.IsNullOrEmpty(Settings.Default.RootDirectory))
                {
                    Log.Current.Debug("Prompting for root directory");
                    using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                    {
                        dialog.Description = string.Format("Choose a location for your documents.  We'll create a folder named {0} in that location.", Title);
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string path = Path.Combine(dialog.SelectedPath, Title);
                            Log.Current.DebugFormat("Setting root directory: {0}", path);
                            Settings.Default.RootDirectory = path;
                            Settings.Default.Save();
                        }
                        else
                        {
                            Log.Current.Fatal("Cancelled setting root directory");
                            return;
                        }
                    }
                }
                ConfigurationExtensions.CreateAndOrLoad();
                App app = new App();
                app.InitializeComponent();
                app.Run(new MainWindow());
                Log.Current.Debug("Exiting");
            };
            try
            {
                executer.Execute();
            }
            catch (TimeoutException)
            {
                MessageBox.Show(string.Format("An instance of {0} is already running.", Title), Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ServiceHost host;

        public DataContext DataContext { get; set; }
        public Service Service { get; private set; }

        public App()
        {
            Service = new Service();
            host = Service.OpenHost();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (host != null)
            {
                host.Close();
            }
            base.OnExit(e);
        }
    }
}
