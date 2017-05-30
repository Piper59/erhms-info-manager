using Epi;
using ERHMS.DataAccess;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Controls;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Infrastructure;
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
using Action = System.Action;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation
{
    public partial class App : Application
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
                Log.SetLevelName(Settings.Default.LogLevel);
                Log.Logger.Debug("Starting up");
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
        public new MainWindow MainWindow { get; private set; }
        public Win32Window MainWin32Window { get; private set; }
        public bool ShuttingDown { get; private set; }

        public App()
        {
            InitializeComponent();
            AddTextFileResource("COPYRIGHT");
            AddTextFileResource("LICENSE");
            AddTextFileResource("NOTICE");
        }

        private void AddTextFileResource(string key)
        {
            string resourceName = string.Format("ERHMS.Presentation.{0}.txt", key);
            Resources.Add(key, Assembly.GetExecutingAssembly().GetManifestResourceText(resourceName));
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
            MainWindow = new MainWindow();
            MainWin32Window = new Win32Window(MainWindow);
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

        private async void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            if (Settings.Default.LicenseAccepted)
            {
                if (LoadSettings())
                {
                    MainViewModel.Instance.OpenDataSourceListView();
                }
                else
                {
                    Shutdown();
                }
            }
            else
            {
                Log.Logger.Debug("Showing license");
                LicenseDialog dialog = new LicenseDialog(MainWindow);
                if (await dialog.ShowAsync() == MessageDialogResult.Affirmative)
                {
                    Log.Logger.Debug("License accepted");
                    Settings.Default.LicenseAccepted = true;
                    Settings.Default.Save();
                    if (LoadSettings())
                    {
                        MainViewModel.Instance.OpenDataSourceListView();
                        Messenger.Default.Send(new AlertMessage
                        {
                            Title = "Welcome",
                            Message = string.Format((string)FindResource("WelcomeText"), Title)
                        });
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

        private bool LoadSettings()
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
            if (!File.Exists(ConfigurationExtensions.FilePath) && File.Exists(Settings.Default.ConfigurationFile))
            {
                File.Copy(Settings.Default.ConfigurationFile, ConfigurationExtensions.FilePath);
            }
            Configuration configuration;
            if (!ConfigurationExtensions.TryLoad(out configuration) || !Directory.Exists(configuration.Directories.Project))
            {
                while (true)
                {
                    Log.Logger.Debug("Prompting for root directory");
                    using (FolderBrowserDialog dialog = RootDirectoryDialog.GetDialog())
                    {
                        if (dialog.Show(MainWin32Window))
                        {
                            string path = dialog.GetRootDirectory();
                            Log.Logger.DebugFormat("Root directory chosen: {0}", path);
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
                                        DataContext sampleDataContext = SampleDataContext.Create();
                                        Settings.Default.DataSources.Add(sampleDataContext.Project.FilePath);
                                        Settings.Default.Save();
                                    }
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                Log.Logger.Warn("Failed to initialize root directory", ex);
                                StringBuilder builder = new StringBuilder();
                                builder.AppendFormat("{0} failed to initialize the following folder. Please choose another location.", Title);
                                builder.AppendLine();
                                builder.AppendLine();
                                builder.Append(path);
                                MessageBox.Show(builder.ToString(), Title, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            Log.Logger.Debug("Root directory not chosen");
                            return false;
                        }
                    }
                }
            }
            Settings.Default.ConfigurationFile = ConfigurationExtensions.FilePath;
            Settings.Default.Save();
            return true;
        }

        private void CopyTextFileResource(string key, string directoryPath)
        {
            string fileName = key + ".txt";
            string resourceName = "ERHMS.Presentation." + fileName;
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, Path.Combine(directoryPath, fileName));
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
    }
}
