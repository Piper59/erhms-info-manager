using ERHMS.DataAccess;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Utility;
using System;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace ERHMS.Presentation
{
    public partial class App : Application
    {
        public const string Title = "ERHMS Info Manager";

        public new static App Current
        {
            get { return (App)Application.Current; }
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
                        dialog.Description = string.Format("Choose a location for your documents. We'll create a folder named {0} in that location.", Title);
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string path = Path.Combine(dialog.SelectedPath, Title);
                            Log.Current.DebugFormat("Setting root directory: {0}", path);
                            Settings.Default.RootDirectory = path;
                            Settings.Default.Save();
                        }
                        else
                        {
                            Log.Current.Fatal("Canceled setting root directory");
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

        public Service Service { get; private set; }
        public DataContext DataContext { get; set; }

        public App()
        {
            Service = new Service();
            host = Service.OpenHost();
            // TODO: Allow data context selection
            FileInfo file = ConfigurationExtensions.GetConfigurationRoot().GetFile("Projects", "ERHMS", "ERHMS.prj");
            if (file.Exists)
            {
                DataContext = new DataContext(new Project(file));
            }
            else
            {
                AccessDriver driver = AccessDriver.Create(Path.ChangeExtension(file.FullName, ".mdb"));
                Project project = Project.Create(
                    "ERHMS",
                    "Emergency Responder Health Monitoring and Surveillance",
                    file.Directory,
                    driver.Provider.ToEpiInfoName(),
                    driver.Builder,
                    driver.DatabaseName);
                DataContext = DataContext.Create(project);
            }
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler(TextBox_GotFocus));
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        public void Invoke(Action action)
        {
            if (Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.Invoke(action);
            }
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
