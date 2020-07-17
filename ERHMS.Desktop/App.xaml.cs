using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ResXResources = ERHMS.Desktop.Properties.Resources;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop
{
    public partial class App : Application
    {
        private static int unhandledErrorCount;

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                ConfigureLog();
                Log.Default.Debug("Starting up");
                ConfigureEpiInfo();
                App app = new App();
                app.Run();
                Log.Default.Debug("Shutting down");
            }
            catch (Exception ex)
            {
                OnUnhandledError(ex);
            }
        }

        private static void ConfigureLog()
        {
            try
            {
                GlobalContext.Properties["user"] = WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException) { }
            GlobalContext.Properties["process"] = Process.GetCurrentProcess().Id;
            XmlConfigurator.Configure();
        }

        private static void ConfigureEpiInfo()
        {
            Log.Default.Debug("Configuring Epi Info");
            if (!ConfigurationExtensions.Exists())
            {
                Log.Default.Debug($"Creating configuration file: {ConfigurationExtensions.FilePath}");
                Configuration configuration = ConfigurationExtensions.Create();
                Settings.Default.Apply(configuration);
                configuration.Save();
            }
            Log.Default.Debug($"Loading configuration file: {ConfigurationExtensions.FilePath}");
            ConfigurationExtensions.Load();
            Configuration.Environment = ExecutionEnvironment.WindowsApplication;
        }

        private static void OnUnhandledError(Exception ex)
        {
            Log.Default.Fatal(ex);
            if (Interlocked.Increment(ref unhandledErrorCount) == 1)
            {
                MessageBox.Show(ex.Message, ResXResources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            CommandBase.GlobalError += CommandBase_GlobalError;
            InitializeComponent();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            OnUnhandledError(e.Exception);
            e.Handled = true;
            Shutdown(1);
        }

        private void CommandBase_GlobalError(object sender, ErrorEventArgs e)
        {
            Log.Default.Error(e.Exception);
            MessageBox.Show(e.Exception.Message, ResXResources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainViewModel.Current.Content = new HomeViewModel();
            Window window = new MainView
            {
                DataContext = MainViewModel.Current
            };
            window.Show();
        }
    }
}
