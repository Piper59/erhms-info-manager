using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using ERHMS.Presentation.ViewModels;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using Action = System.Action;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;
using TextBox = System.Windows.Controls.TextBox;

namespace ERHMS.Presentation
{
    public partial class App : Application
    {
        public const string BareTitle = "ERHMS Info Manager";

        public static string Title
        {
            get { return string.Format("{0}\u2122", BareTitle); }
        }

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
                if (LoadSettings())
                {
                    App app = new App();
                    app.InitializeComponent();
                    MainWindow window = new MainWindow(app.Locator.Main);
                    window.Loaded += (_sender, _e) =>
                    {
                        app.Locator.Main.OpenDataSourceListView();
                        window.Activate();
                        if (Settings.Default.InitialExecution)
                        {
                            ConfirmMessage msg = new ConfirmMessage("Terms of Use", "Accept", app.TermsOfUse);
                            msg.Confirmed += (__sender, __e) =>
                            {
                                Messenger.Default.Send(new NotifyMessage("Welcome", app.Welcome));
                                Settings.Default.InitialExecution = false;
                                Settings.Default.Save();
                            };
                            msg.Canceled += (__sender, __e) =>
                            {
                                app.Shutdown();
                            };
                            Messenger.Default.Send(msg);
                        }
                    };
                    app.Run(window);
                }
                Log.Current.Debug("Exiting");
            };
            try
            {
                executer.Execute();
            }
            catch (TimeoutException)
            {
                ShowErrorMessage(string.Format("An instance of {0} is already running.", Title));
            }
            catch (Exception ex)
            {
                Log.Current.Fatal("Fatal error", ex);
                ShowErrorMessage(string.Format("{0} encountered an error and must shut down.", Title));
            }
        }

        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static bool LoadSettings()
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                MessageBoxResult result = MessageBox.Show(
                    string.Format("Reset settings for {0}?", Title),
                    Title,
                    MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Settings.Reset();
                }
            }
            if (!string.IsNullOrEmpty(Settings.Default.RootDirectory))
            {
                try
                {
                    ConfigurationExtensions.CreateAndOrLoad();
                }
                catch (UnauthorizedAccessException)
                {
                    Log.Current.WarnFormat("Access denied to root directory: {0}", Settings.Default.RootDirectory);
                    Settings.Default.RootDirectory = null;
                }
            }
            while (string.IsNullOrEmpty(Settings.Default.RootDirectory))
            {
                Log.Current.Debug("Prompting for root directory");
                using (FolderBrowserDialog dialog = RootDirectoryDialog.GetDialog())
                {
                    if (dialog.ShowDialog(true) == DialogResult.OK)
                    {
                        string path = dialog.GetRootDirectory();
                        Log.Current.DebugFormat("Setting root directory: {0}", path);
                        Settings.Default.RootDirectory = path;
                        try
                        {
                            ConfigurationExtensions.CreateAndOrLoad();
                            Configuration configuration = Configuration.GetNewInstance();
                            DirectoryInfo projects = new DirectoryInfo(configuration.Directories.Project);
                            foreach (FileInfo project in projects.SearchByExtension(Project.FileExtension))
                            {
                                XmlDocument document = new XmlDocument();
                                document.Load(project.FullName);
                                XmlNode databaseNode = document.SelectSingleNode("/Project/CollectedData/Database");
                                if (databaseNode.Attributes["connectionString"].Value == "")
                                {
                                    FileInfo database = new FileInfo(Path.ChangeExtension(project.FullName, ".mdb"));
                                    if (database.Exists)
                                    {
                                        databaseNode.Attributes["connectionString"].Value = Configuration.Encrypt(
                                            string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\"", database.FullName));
                                        document.Save(project.FullName);
                                    }
                                }
                                Settings.Default.DataSources.Add(project.FullName);
                            }
                            Settings.Default.Save();
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Log.Current.WarnFormat("Access denied to root directory: {0}", path);
                            ShowErrorMessage(string.Format("You do not have access to {0}. Please choose another location.", path));
                            Settings.Default.RootDirectory = null;
                        }
                    }
                    else
                    {
                        Log.Current.Debug("Canceled setting root directory");
                        return false;
                    }
                }
            }
            return true;
        }

        private ServiceHost host;

        public Service Service { get; private set; }
        public ViewModelLocator Locator { get; private set; }
        public bool ShuttingDown { get; private set; }

        public string TermsOfUse
        {
            get { return (string)FindResource("TermsOfUse"); }
        }

        public string License
        {
            get { return (string)FindResource("License"); }
        }

        public string Welcome
        {
            get
            {
                return string.Join(" ", new string[]
                {
                    string.Format("Welcome to {0}!", Title),
                    "To get started, select a data source from the list and click Open.",
                    "To add a new data source to the list, click Add > New.",
                    "To add an existing data source to the list, click Add > Existing."
                });
            }
        }

        public App()
        {
            DispatcherUnhandledException += (sender, e) =>
            {
                Log.Current.Fatal("Fatal error", e.Exception);
                ShowErrorMessage(string.Format("{0} encountered an error and must shut down.", Title));
                e.Handled = true;
                Shutdown();
            };
            Service = new Service();
            host = Service.OpenHost();
            Locator = new ViewModelLocator();
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler((sender, e) =>
            {
                if (e.KeyboardDevice.IsKeyDown(Key.Tab))
                {
                    ((TextBox)sender).SelectAll();
                }
            }));
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler((sender, e) =>
            {
                ((TextBox)sender).Select(0, 0);
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

        public new void Shutdown()
        {
            ShuttingDown = true;
            base.Shutdown();
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
