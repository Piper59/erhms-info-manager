using Epi;
using ERHMS.DataAccess;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Services;
using ERHMS.Presentation.ViewModels;
using ERHMS.Presentation.Views;
using ERHMS.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation
{
    public partial class App : Application, IAppService
    {
        private static InterlockedBoolean errored;
        private static ServiceManager services;

        [STAThread]
        internal static void Main(string[] args)
        {
            errored = new InterlockedBoolean(false);
            services = new ServiceManager
            {
                Busy = new BusyService(),
                Data = new DataService(),
                Print = new PrintService(),
                Process = new ProcessService()
            };
            try
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    string message = string.Format("Reset settings for {0}?", services.String.AppTitle);
                    if (MessageBox.Show(message, services.String.AppTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ResetSettings();
                    }
                }
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
            if (errored.Exchange(true) == false)
            {
                string message = string.Format("{0} encountered an error and must shut down.", services.String.AppTitle);
                MessageBox.Show(message, services.String.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void ResetSettings()
        {
            try
            {
                Settings.Default.Reset();
                Settings.Default.Save();
                if (File.Exists(ConfigurationExtensions.FilePath))
                {
                    File.Copy(ConfigurationExtensions.FilePath, ConfigurationExtensions.FilePath + ".bak", true);
                    File.Delete(ConfigurationExtensions.FilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to reset settings", ex);
            }
        }

        public App()
        {
            InitializeComponent();
            ShutdownMode = ShutdownMode.OnMainWindowClose;
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
            string resourceName = string.Format("ERHMS.Presentation.{0}.txt", key);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, Path.Combine(directoryPath, fileName));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            services.App = this;
            services.Dispatch = new DispatchService(SynchronizationContext.Current);
            services.String = new StringService(Resources);
            EventManager.RegisterClassHandler(
                typeof(TextBox),
                UIElement.GotKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(TextBox_GotKeyboardFocus));
            EventManager.RegisterClassHandler(
                typeof(TextBox),
                UIElement.LostKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(TextBox_LostKeyboardFocus));
            MainViewModel model = new MainViewModel(services);
            services.Document = model;
            MainView view = new MainView(model);
            services.Dialog = view;
            services.Wrapper = view;
            MainWindow = view;
            MainWindow.ContentRendered += MainWindow_ContentRendered;
            MainWindow.Show();
#if DEBUG_FULL
            DebugDataBinding();
#endif
        }

        private void DebugDataBinding()
        {
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
            TraceDialog dialog = new TraceDialog(PresentationTraceSources.DataBindingSource)
            {
                Title = "Data Binding"
            };
            dialog.Show();
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
                await StartupAsync();
            }
            else
            {
                if (await services.Dialog.ShowLicenseAsync())
                {
                    Settings.Default.LicenseAccepted = true;
                    Settings.Default.Save();
                    await StartupAsync();
                }
                else
                {
                    Shutdown();
                }
            }
        }

        private async Task StartupAsync()
        {
            if (await LoadConfigurationAsync())
            {
                services.Document.Show(() => new StartViewModel(services));
                ProjectInfo projectInfo;
                if (ProjectInfo.TryRead(Settings.Default.LastDataSourcePath, out projectInfo))
                {
                    if (await services.Dialog.ConfirmAsync(string.Format("Reopen the previously used data source {0}?", projectInfo.Name), "Reopen"))
                    {
                        await services.Document.SetContextAsync(projectInfo);
                    }
                    else
                    {
                        Settings.Default.LastDataSourcePath = null;
                        Settings.Default.Save();
                    }
                }
            }
            else
            {
                Shutdown();
            }
        }

        private async Task<bool> LoadConfigurationAsync()
        {
            if (!File.Exists(ConfigurationExtensions.FilePath) && File.Exists(Settings.Default.ConfigurationFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationExtensions.FilePath));
                File.Copy(Settings.Default.ConfigurationFilePath, ConfigurationExtensions.FilePath);
            }
            Configuration configuration;
            if (!ConfigurationExtensions.TryLoad(out configuration) || !Directory.Exists(configuration.GetRootPath()))
            {
                while (true)
                {
                    string path = services.Dialog.GetRootPath();
                    if (path == null)
                    {
                        return false;
                    }
                    else
                    {
                        try
                        {
                            configuration = CreateConfiguration(path);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Warn("Failed to initialize root path", ex);
                            StringBuilder message = new StringBuilder();
                            message.AppendFormat(
                                "{0} failed to initialize the following directory. Please choose another location.",
                                services.String.AppTitle);
                            message.AppendLine();
                            message.AppendLine();
                            message.Append(path);
                            await services.Dialog.AlertAsync(message.ToString(), ex);
                        }
                    }
                }
            }
            Settings.Default.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Settings.Default.ConfigurationFilePath = ConfigurationExtensions.FilePath;
            Settings.Default.Save();
            configuration.Directories.LogDir = Path.GetDirectoryName(Log.FilePath);
            configuration.Save();
            configuration = ConfigurationExtensions.Load();
            using (TextWriter writer = new StreamWriter(Path.Combine(configuration.GetRootPath(), "INSTALL.txt")))
            {
                writer.WriteLine("{0} is installed in the following directory:", services.String.AppTitle);
                writer.WriteLine();
                writer.WriteLine(AssemblyExtensions.GetEntryDirectoryPath());
            }
            return true;
        }

        private Configuration CreateConfiguration(string path)
        {
            using (services.Busy.BeginTask())
            {
                Configuration configuration = ConfigurationExtensions.Create(path);
                configuration.CreateUserDirectories();
                IOExtensions.CopyDirectory(
                    Path.Combine(AssemblyExtensions.GetEntryDirectoryPath(), "Templates"),
                    configuration.Directories.Templates);
                CopyTextFileResource("COPYRIGHT", path);
                CopyTextFileResource("LICENSE", path);
                CopyTextFileResource("NOTICE", path);
                configuration.Save();
                configuration = ConfigurationExtensions.Load();
                if (!SampleDataContext.Exists())
                {
                    SampleDataContext.Create();
                }
                Settings.Default.DataSourcePaths.Add(SampleDataContext.GetFilePath());
                Settings.Default.Save();
                return configuration;
            }
        }

        void IAppService.Exit()
        {
            MainWindow.Close();
        }

        public void Restart()
        {
            Shutdown();
            System.Windows.Forms.Application.Restart();
        }
    }
}
