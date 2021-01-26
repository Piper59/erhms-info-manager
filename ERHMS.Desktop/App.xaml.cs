using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Services.Implementation;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using log4net;
using log4net.Appender;
using log4net.Layout;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop
{
    public partial class App : Application
    {
        private static App app;
        private static int errorCount;

        [STAThread]
        private static void Main()
        {
            Log.Initializing += Log_Initializing;
            Log.Instance.Debug("Entering application");
            try
            {
                ConfigureEpiInfo();
                app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                OnUnhandledError(ex);
            }
            Log.Instance.Debug("Exiting application");
        }

        private static string GetUserName()
        {
            try
            {
                return WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                return "?";
            }
        }

        private static int GetProcessId()
        {
            try
            {
                return Process.GetCurrentProcess().Id;
            }
            catch
            {
                return -1;
            }
        }

        private static void Log_Initializing(object sender, InitializingEventArgs e)
        {
            GlobalContext.Properties["user"] = GetUserName();
            GlobalContext.Properties["process"] = GetProcessId();
            PatternLayout layout = new PatternLayout("%date | %property{user} | %property{process}(%thread) | %level | %message%newline");
            layout.ActivateOptions();
            FileAppender appender = new FileAppender
            {
                File = Path.Combine("Logs", $"ERHMS.{DateTime.Now:yyyy-MM-dd}.txt"),
                LockingModel = new FileAppender.InterProcessLock(),
                Layout = layout
            };
            appender.ActivateOptions();
            e.Hierarchy.Root.AddAppender(appender);
        }

        private static void ConfigureEpiInfo()
        {
            if (!ConfigurationExtensions.Exists())
            {
                Epi.Configuration configuration = ConfigurationExtensions.Create();
                configuration.Save();
            }
            ConfigurationExtensions.Load();
            Epi.Configuration.Environment = Epi.ExecutionEnvironment.WindowsApplication;
        }

        private static void OnUnhandledError(Exception exception)
        {
            if (exception != null)
            {
                Log.Instance.Fatal(exception);
            }
            if (Interlocked.Increment(ref errorCount) == 1)
            {
                StringBuilder message = new StringBuilder();
                message.Append(ResX.UnhandledErrorMessage);
                if (exception != null)
                {
                    message.AppendLine();
                    message.AppendLine();
                    message.Append(exception.Message);
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
            ConfigureServices();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => OnUnhandledError(e.ExceptionObject as Exception);
            DispatcherUnhandledException += (sender, e) =>
            {
                OnUnhandledError(e.Exception);
                e.Handled = true;
            };
            Command.GlobalError += (sender, e) => OnHandledError(e.Exception);
            InitializeComponent();
        }

        private void ConfigureServices()
        {
            ServiceProvider.Install<IDialogService>(() => new DialogService(this));
        }

        private void OnHandledError(Exception exception)
        {
            Log.Instance.Error(exception);
            ServiceProvider.Resolve<IDialogService>().Show(new Dialog(DialogPreset.Error)
            {
                Lead = ResX.HandledErrorLead,
                Body = exception.Message,
                Details = exception.ToString()
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bool reset = false;
            if (e.Args.Contains("/reset", StringComparer.OrdinalIgnoreCase))
            {
                Log.Instance.Debug("Resetting application settings");
                Settings.Default.Reset();
                reset = true;
            }
            MainViewModel.Instance.Content = new HomeViewModel();
            Window window = new MainView
            {
                DataContext = MainViewModel.Instance
            };
            window.Show();
            if (reset)
            {
                ServiceProvider.Resolve<IDialogService>().Show(new Dialog(DialogPreset.Default)
                {
                    Lead = ResX.SettingsResetLead
                });
            }
        }
    }
}
