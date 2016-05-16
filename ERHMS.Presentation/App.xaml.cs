using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.Presentation.ViewModels;
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
                MainWindow window = new MainWindow(app.Locator.Main);
                window.ContentRendered += (_sender, _e) =>
                {
                    app.Locator.Main.OpenDataSourceListView();
                };
                app.Run(window);
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
        public ViewModelLocator Locator { get; private set; }

        public App()
        {
            DispatcherUnhandledException += (sender, e) =>
            {
                Log.Current.Fatal("Fatal error", e.Exception);
                MessageBox.Show(string.Format("{0} encountered an error and must shut down.", Title), Title, MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                Shutdown();
            };
            Service = new Service();
            host = Service.OpenHost();
            Locator = new ViewModelLocator();
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler((sender, e) =>
            {
                ((TextBox)sender).SelectAll();
            }));
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
