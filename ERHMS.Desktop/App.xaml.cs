using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
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
        private static bool reset;

        [STAThread]
        public static void Main(string[] args)
        {
            ConfigureLog();
            Log.Default.Debug("Starting up");
            try
            {
                ParseArgs(args);
                ConfigureServices();
                ConfigureEpiInfo();
                App app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                OnUnhandledError(ex);
            }
            Log.Default.Debug("Shutting down");
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

        private static void ParseArgs(IList<string> args)
        {
            if (args.Contains("/reset", StringComparer.OrdinalIgnoreCase))
            {
                Log.Default.Debug("Resetting settings");
                Settings.Default.Reset();
                reset = true;
            }
        }

        private static void ConfigureServices()
        {
            ServiceLocator.Dialog = new DialogService();
        }

        private static void ConfigureEpiInfo()
        {
            Log.Default.Debug("Configuring Epi Info");
            if (!ConfigurationExtensions.Exists())
            {
                Log.Default.Debug($"Creating configuration file: {ConfigurationExtensions.FilePath}");
                Configuration configuration = ConfigurationExtensions.Create();
                Settings.Default.WriteTo(configuration);
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
                StringBuilder message = new StringBuilder();
                message.AppendLine($"{ResXResources.AppTitle} has encountered an error and must shut down.");
                message.AppendLine();
                message.Append(ex.Message);
                MessageBox.Show(message.ToString(), ResXResources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ResourceDictionary ThemeDictionary => Resources.MergedDictionaries[0];

        public App()
        {
            InitializeComponent();
            SetTheme();
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            Command.GlobalError += Command_GlobalError;
        }

        private void SetTheme()
        {
            string themeName = SystemParameters.HighContrast ? "HighContrast" : "Default";
            Uri source = new Uri($"/Themes/{themeName}.xaml", UriKind.Relative);
            if (ThemeDictionary.Source != source)
            {
                ThemeDictionary.Source = source;
            }
        }

        private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SystemParameters.HighContrast))
            {
                SetTheme();
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            OnUnhandledError(e.Exception);
            e.Handled = true;
            Shutdown(1);
        }

        private void Command_GlobalError(object sender, ErrorEventArgs e)
        {
            OnHandledError(e.Exception);
        }

        private void OnHandledError(Exception ex)
        {
            Log.Default.Error(ex);
            ServiceLocator.Dialog.Show(new DialogInfo(DialogInfoPreset.Error)
            {
                Lead = $"{ResXResources.AppTitle} has encountered an error",
                Body = ex.Message,
                Details = ex.ToString()
            });
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
            if (reset)
            {
                ServiceLocator.Dialog.Show(new DialogInfo(DialogInfoPreset.Normal)
                {
                    Lead = "Settings have been reset"
                });
            }
        }
    }
}
