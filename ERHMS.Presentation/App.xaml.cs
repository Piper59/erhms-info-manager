using Epi;
using ERHMS.DataAccess;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Controls;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using ERHMS.Presentation.ViewModels;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Controls;
using Action = System.Action;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation
{
    public partial class App : Application, IServiceManager
    {
        public const string BareTitle = "ERHMS Info Manager";
        public static readonly string Title = BareTitle + "\u2122";

        private static bool errored;
        private static object erroredLock = new object();

        public new static App Current
        {
            get { return (App)Application.Current; }
        }

        [STAThread]
        internal static void Main(string[] args)
        {
            try
            {
                Log.LevelName = Settings.Default.LogLevelName;
                Log.Logger.Debug("Starting up");
                DataContext.Configure();
                App app = new App();
                app.DispatcherUnhandledException += (sender, e) =>
                {
                    HandleError(e.Exception);
                    e.Handled = true;
                    app.Shutdown();
                };
                app.Run();
                Log.Logger.Debug("Exiting");
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static void HandleError(Exception ex)
        {
            Log.Logger.Fatal("Fatal error", ex);
            lock (erroredLock)
            {
                if (errored)
                {
                    return;
                }
                errored = true;
                string message = string.Format("{0} encountered an error and must shut down.", Title);
                MessageBox.Show(message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ResourceDictionary Accent { get; private set; }
        public MainViewModel MainViewModel { get; private set; }
        public new MainWindow MainWindow { get; private set; }

        public IDocumentManager Documents
        {
            get { return MainViewModel; }
        }

        public IDialogManager Dialogs
        {
            get { return MainWindow; }
        }

        public DataContext Context { get; set; }
        public bool ShuttingDown { get; private set; }

        public App()
        {
            InitializeComponent();
            Resources.Add("AppTitle", Title);
            AddTextFileResource("COPYRIGHT");
            AddTextFileResource("LICENSE");
            AddTextFileResource("NOTICE");
        }

        private void AddTextFileResource(string key)
        {
            string resourceName = string.Format("ERHMS.Presentation.{0}.txt", key);
            Resources.Add(key, Assembly.GetExecutingAssembly().GetManifestResourceText(resourceName));
        }

        private void CopyTextFileResource(string key, string directoryPath)
        {
            string fileName = key + ".txt";
            string resourceName = "ERHMS.Presentation." + fileName;
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, Path.Combine(directoryPath, fileName));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Accent = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/ERHMS.Presentation;component/Resources/Blue508.xaml")
            };
            EventManager.RegisterClassHandler(
                typeof(TextBox),
                UIElement.GotKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(TextBox_GotKeyboardFocus));
            EventManager.RegisterClassHandler(
                typeof(TextBox),
                UIElement.LostKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(TextBox_LostKeyboardFocus));
            EventManager.RegisterClassHandler(
                typeof(TabItem),
                UIElement.GotKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(TabItem_GotKeyboardFocus));
            Messenger.Default.Register<ShutdownMessage>(this, msg =>
            {
                Shutdown();
                if (msg.Restart)
                {
                    System.Windows.Forms.Application.Restart();
                }
            });
            MainViewModel = new MainViewModel(this);
            MainWindow = new MainWindow(MainViewModel);
            MainWindow.ContentRendered += MainWindow_ContentRendered;
            MainWindow.Show();
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Tab))
            {
                ((TextBox)sender).SelectAll();
            }
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Tab))
            {
                ((TextBox)sender).Select(0, 0);
            }
        }

        private void TabItem_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TabItem tabItem = e.OriginalSource as TabItem;
            if (tabItem != null && VisualTreeHelper.GetParent(tabItem).GetType() == typeof(DocumentPaneTabPanel))
            {
                Keyboard.ClearFocus();
            }
        }

        private async void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            if (Settings.Default.LicenseAccepted)
            {
                if (LoadConfiguration())
                {
                    Documents.ShowDataSources();
                }
                else
                {
                    Shutdown();
                }
            }
            else
            {
                Log.Logger.Debug("Showing license");
                if (await LicenseDialog.ShowAsync(MainWindow) == MessageDialogResult.Affirmative)
                {
                    Log.Logger.Debug("License accepted");
                    Settings.Default.LicenseAccepted = true;
                    Settings.Default.Save();
                    if (LoadConfiguration())
                    {
                        Documents.ShowDataSources();
                        await WelcomeDialog.ShowAsync(MainWindow);
                    }
                    else
                    {
                        Shutdown();
                    }
                }
                else
                {
                    Log.Logger.Debug("License not accepted");
                    Shutdown();
                }
            }
        }

        private bool LoadConfiguration()
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                string message = string.Format("Reset settings for {0}?", Title);
                if (MessageBox.Show(message, Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Settings.Default.Reset();
                    Settings.Default.Save();
                }
            }
            if (!File.Exists(ConfigurationExtensions.FilePath) && File.Exists(Settings.Default.ConfigurationFilePath))
            {
                File.Copy(Settings.Default.ConfigurationFilePath, ConfigurationExtensions.FilePath);
            }
            Configuration configuration;
            if (!ConfigurationExtensions.TryLoad(out configuration) || !Directory.Exists(configuration.GetRootPath()))
            {
                while (true)
                {
                    Log.Logger.Debug("Prompting for root path");
                    using (FolderBrowserDialog dialog = RootPathDialog.GetDialog())
                    {
                        if (dialog.Show(MainWindow.Win32Window))
                        {
                            string path = dialog.GetRootPath();
                            Log.Logger.DebugFormat("Root path chosen: {0}", path);
                            try
                            {
                                using (new WaitCursor())
                                {
                                    configuration = ConfigurationExtensions.Create(path);
                                    configuration.CreateUserDirectories();
                                    IOExtensions.CopyDirectory(
                                        Path.Combine(AssemblyExtensions.GetEntryDirectoryPath(), "Templates"),
                                        configuration.Directories.Templates);
                                    CopyTextFileResource("COPYRIGHT", path);
                                    CopyTextFileResource("LICENSE", path);
                                    CopyTextFileResource("NOTICE", path);
                                    configuration.Save();
                                    ConfigurationExtensions.Load();
                                    if (!SampleDataContext.Exists())
                                    {
                                        DataContext context = SampleDataContext.Create();
                                        Settings.Default.DataSourcePaths.Add(context.Project.FilePath);
                                        Settings.Default.Save();
                                    }
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                Log.Logger.Warn("Failed to initialize root path", ex);
                                StringBuilder message = new StringBuilder();
                                message.AppendFormat("{0} failed to initialize the following directory. Please choose another location.", Title);
                                message.AppendLine();
                                message.AppendLine();
                                message.Append(path);
                                MessageBox.Show(message.ToString(), Title, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            Log.Logger.Debug("Root path not chosen");
                            return false;
                        }
                    }
                }
            }
            Settings.Default.ConfigurationFilePath = ConfigurationExtensions.FilePath;
            Settings.Default.Save();
            return true;
        }

        public new void Shutdown()
        {
            ShuttingDown = true;
            base.Shutdown();
        }
    }
}
