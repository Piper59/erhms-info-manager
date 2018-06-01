using CommonServiceLocator;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;
using Unity.ServiceLocation;
using Resx = ERHMS.Presentation.Properties.Resources;
using ServiceLocator = ERHMS.Presentation.Services.ServiceLocator;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation
{
    public partial class App : Application, IAppService
    {
        private static InterlockedBoolean errored;

        [STAThread]
        internal static void Main(string[] args)
        {
            errored = new InterlockedBoolean(false);
            try
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    if (MessageBox.Show(Resx.AppConfirmReset, Resx.AppTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                MessageBox.Show(Resx.AppError, Resx.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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
            Resources.Add("AppTitle", Resx.AppTitle);
            Resources.Add("COPYRIGHT", Resx.COPYRIGHT);
            Resources.Add("LICENSE", Resx.LICENSE);
            Resources.Add("NOTICE", Resx.NOTICE);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EventManager.RegisterClassHandler(
                typeof(TextBox),
                UIElement.GotKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(TextBox_GotKeyboardFocus));
            EventManager.RegisterClassHandler(
                typeof(TextBox),
                UIElement.LostFocusEvent,
                new RoutedEventHandler(TextBox_LostFocus));
            MainViewModel model = new MainViewModel();
            MainView view = new MainView(model);
            InitializeServices(view);
            MainWindow = view;
            MainWindow.ContentRendered += MainWindow_ContentRendered;
            MainWindow.Show();
#if DEBUG_FULL
            DebugDataBinding();
#endif
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Tab))
            {
                ((TextBox)sender).SelectAll();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Select(0, 0);
        }

        private void InitializeServices(MainView view)
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<IAppService>(this);
            container.RegisterInstance<IBusyService>(new BusyService());
            container.RegisterInstance<IDataService>(new DataService());
            container.RegisterInstance<IDialogService>(view);
            container.RegisterInstance<IDispatcherService>(new DispatcherService());
            container.RegisterInstance<IDocumentService>(view.DataContext);
            container.RegisterInstance<IPrinterService>(new PrinterService());
            container.RegisterInstance<IProcessService>(new ProcessService());
            container.RegisterInstance<IWrapperService>(view);
            IServiceLocator serviceLocator = new UnityServiceLocator(container);
            CommonServiceLocator.ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        private async void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            if (Settings.Default.LicenseAccepted)
            {
                await StartupAsync();
            }
            else
            {
                if (await ServiceLocator.Dialog.ShowLicenseAsync())
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
                ServiceLocator.Document.Show(() => new StartViewModel());
                ProjectInfo projectInfo;
                if (ProjectInfo.TryRead(Settings.Default.LastDataSourcePath, out projectInfo))
                {
                    string message = string.Format(Resx.DataSourceConfirmReopen, projectInfo.Name);
                    if (await ServiceLocator.Dialog.ConfirmAsync(message, "Reopen"))
                    {
                        await ServiceLocator.Document.SetContextAsync(projectInfo);
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
                    string path = ServiceLocator.Dialog.GetRootPath();
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
                            string message = string.Format(Resx.RootPathInitializeFailed, path);
                            await ServiceLocator.Dialog.ShowErrorAsync(message, ex);
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
            string text = string.Format(Resx.AppInstallDirectory, AssemblyExtensions.GetEntryDirectoryPath());
            File.WriteAllText(Path.Combine(configuration.GetRootPath(), "INSTALL.txt"), text);
            return true;
        }

        private Configuration CreateConfiguration(string path)
        {
            using (ServiceLocator.Busy.Begin())
            {
                Configuration configuration = ConfigurationExtensions.Create(path);
                configuration.CreateUserDirectories();
                IOExtensions.CopyDirectory(
                    Path.Combine(AssemblyExtensions.GetEntryDirectoryPath(), "Templates"),
                    configuration.Directories.Templates);
                File.WriteAllText(Path.Combine(path, "COPYRIGHT.txt"), Resx.COPYRIGHT);
                File.WriteAllText(Path.Combine(path, "LICENSE.txt"), Resx.LICENSE);
                File.WriteAllText(Path.Combine(path, "NOTICE.txt"), Resx.NOTICE);
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
