using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using ERHMS.Presentation.ViewModels;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
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

        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstanceExecuter executer = new SingleInstanceExecuter();
            executer.Executing += (sender, e) =>
            {
                Log.Current.Debug("Starting up");
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    MessageBoxResult result = MessageBox.Show(
                        string.Format("Reset settings for {0}?", Title),
                        Title,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
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
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string path = dialog.GetRootDirectory();
                            Log.Current.DebugFormat("Setting root directory: {0}", path);
                            Settings.Default.RootDirectory = path;
                            try
                            {
                                ConfigurationExtensions.CreateAndOrLoad();
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
                            return;
                        }
                    }
                }
                App app = new App();
                app.InitializeComponent();
                MainWindow window = new MainWindow(app.Locator.Main);
                window.Loaded += (_sender, _e) =>
                {
                    app.Locator.Main.OpenDataSourceListView();
                    window.Activate();
                    if (Settings.Default.InitialExecution)
                    {
                        string message;
                        message = string.Join(Environment.NewLine, new string[]
                        {
                            "Confidentiality and Non-Disclosure",
                            "",
                            "By agreeing to provide peer review for this software, you acknowledge that you fully understand the confidential nature of the review process and agree: (1) to destroy or return all materials related to it; (2) not to discuss the materials associated with the review, your evaluation, or other information associated with your review with any other individual except as authorized by the designated NIOSH official; and (3) to refer all inquiries concerning the review to the designated NIOSH official.",
                            "",
                            "Disclaimer of Liability",
                            "",
                            "This NIOSH-developed software is provided \"as-is\" without warranty of any kind, including express or implied warranties of merchantability or fitness for a particular purpose. By acceptance and use of this software, which is conveyed to the user without consideration by NIOSH, the user expressly waives any and all claims for damage and/or suits for personal injury or property damage resulting from any direct, indirect, incidental, special or consequential damages, or damages for loss of profits, revenue, data or property use, incurred by you or any third party, whether in an action in contract or tort, arising from your access to, or use of, this software in whole or in part."
                        });
                        ConfirmMessage msg = new ConfirmMessage("Terms of Use", "Accept", message);
                        msg.Confirmed += (__sender, __e) =>
                        {
                            message = string.Join(" ", new string[]
                            {
                                "Welcome to ERHMS Info Manager!",
                                "To get started, select a data source from the list and click Open.",
                                "To add a new data source to the list, click Add > New.",
                                "To add an existing data source to the list, click Add > Existing."
                            });
                            Messenger.Default.Send(new NotifyMessage("Welcome", message));
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

        private ServiceHost host;

        public Service Service { get; private set; }
        public ViewModelLocator Locator { get; private set; }
        public bool ShuttingDown { get; private set; }

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
