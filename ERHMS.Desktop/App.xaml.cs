using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop
{
    public partial class App : Application
    {
        private static readonly FieldInfo MenuDropAlignmentField = typeof(SystemParameters).GetField(
            "_menuDropAlignment",
            BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly string ThemeName = SystemParameters.HighContrast ? "HighContrast" : "Default";

        private static App app;
        private static int unhandledErrorCount;

        public static Uri ThemeDictionarySource => new Uri($"pack://application:,,,/ERHMS Info Manager;component/Themes/{ThemeName}.xaml");

        [STAThread]
        private static void Main(string[] args)
        {
            ConfigureLog();
            Log.Default.Debug("Starting up");
            try
            {
                app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                OnUnhandledError(ex);
            }
            Log.Default.Debug("Shutting down");
        }

        internal static void ConfigureLog()
        {
            try
            {
                GlobalContext.Properties["user"] = WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException) { }
            GlobalContext.Properties["process"] = Process.GetCurrentProcess().Id;
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            PatternLayout layout = new PatternLayout("%date | %property{user} | %property{process}(%thread) | %level | %message%newline");
            layout.ActivateOptions();
            FileAppender appender = new FileAppender
            {
                File = Path.Combine("Logs", $"ERHMS.{DateTime.Now:yyyy-MM-dd}.txt"),
                LockingModel = new FileAppender.InterProcessLock(),
                Layout = layout
            };
            appender.ActivateOptions();
            hierarchy.Root.AddAppender(appender);
            hierarchy.Configured = true;
        }

        private static void OnUnhandledError(Exception ex)
        {
            if (ex != null)
            {
                Log.Default.Fatal(ex);
            }
            if (Interlocked.Increment(ref unhandledErrorCount) == 1)
            {
                StringBuilder message = new StringBuilder();
                message.Append(ResX.UnhandledErrorMessage);
                if (ex != null)
                {
                    message.AppendLine();
                    message.AppendLine();
                    message.Append(ex.Message);
                }
                MessageBox.Show(message.ToString(), ResX.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (app != null)
            {
                app.Shutdown(1);
            }
        }

        public App()
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => OnUnhandledError(e.ExceptionObject as Exception);
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            ConfigureServices();
            ConfigureEpiInfo();
            InitializeComponent();
            SetMenuDropAlignment();
            Command.GlobalError += (sender, e) => OnHandledError(e.Exception);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            OnUnhandledError(e.Exception);
            e.Handled = true;
        }

        private void ConfigureServices()
        {
            Log.Default.Debug("Configuring services");
            ServiceProvider.Install<IDialogService>(() => new DialogService(this));
            ServiceProvider.Install<IFileDialogService>(() => new FileDialogService(this));
            ServiceProvider.Install<IProgressService>(() => new ProgressService(this));
            ServiceProvider.Install<IWizardService>(() => new WizardService(this));
        }

        private void ConfigureEpiInfo()
        {
            Log.Default.Debug("Configuring Epi Info");
            if (!ConfigurationExtensions.Exists())
            {
                Log.Default.Debug($"Creating configuration file: {ConfigurationExtensions.FilePath}");
                Epi.Configuration configuration = ConfigurationExtensions.Create();
                Settings.Default.ApplyTo(configuration);
                configuration.Save();
            }
            Log.Default.Debug($"Loading configuration file: {ConfigurationExtensions.FilePath}");
            ConfigurationExtensions.Load();
            Epi.Configuration.Environment = Epi.ExecutionEnvironment.WindowsApplication;
        }

        private void SetMenuDropAlignment()
        {
            if (SystemParameters.MenuDropAlignment && MenuDropAlignmentField != null)
            {
                MenuDropAlignmentField.SetValue(null, false);
            }
        }

        private void OnHandledError(Exception ex)
        {
            Log.Default.Error(ex);
            ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Error)
            {
                Lead = ResX.HandledErrorLead,
                Body = ex.Message,
                Details = ex.ToString()
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bool resetting = false;
            if (e.Args.Contains("/reset", StringComparer.OrdinalIgnoreCase))
            {
                resetting = true;
                Log.Default.Debug("Resetting settings");
                Settings.Default.Reset();
            }
            MainViewModel.Current.Content = new HomeViewModel();
            Window window = new MainView
            {
                DataContext = MainViewModel.Current
            };
            window.Show();
            if (resetting)
            {
                ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Default)
                {
                    Lead = ResX.SettingsResetLead
                });
            }
        }
    }
}
