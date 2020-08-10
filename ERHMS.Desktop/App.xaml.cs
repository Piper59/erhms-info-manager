using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using ERHMS.EpiInfo;
using log4net;
using log4net.Config;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        private static readonly FieldInfo MenuDropAlignmentField;

        private static int unhandledErrorCount;

        static App()
        {
            MenuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
        }

        [STAThread]
        private static void Main(string[] args)
        {
            ConfigureLog();
            Log.Default.Debug("Starting up");
            try
            {
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

        private static void OnUnhandledError(Exception ex)
        {
            if (ex != null)
            {
                Log.Default.Fatal(ex);
            }
            if (Interlocked.Increment(ref unhandledErrorCount) == 1)
            {
                StringBuilder message = new StringBuilder();
                message.Append(ResXResources.UnhandledErrorMessage);
                if (ex != null)
                {
                    message.AppendLine();
                    message.AppendLine();
                    message.Append(ex.Message);
                }
                MessageBox.Show(message.ToString(), ResXResources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ResourceDictionary ThemeDictionary => Resources.MergedDictionaries[0];

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => OnUnhandledError(e.ExceptionObject as Exception);
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            ConfigureServices();
            ConfigureEpiInfo();
            InitializeComponent();
            SetTheme();
            SetMenuDropAlignment();
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
            Command.GlobalError += (sender, e) => OnHandledError(e.Exception);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            OnUnhandledError(e.Exception);
            e.Handled = true;
            Shutdown(1);
        }

        private void ConfigureServices()
        {
            Log.Default.Debug("Configuring services");
            ServiceProvider.GetDialogService = info => new DialogService(this, info);
            ServiceProvider.GetProgressService = (taskName, canUserCancel) => new ProgressService(this, taskName, canUserCancel);
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

        private void SetTheme()
        {
            string themeName = SystemParameters.HighContrast ? "HighContrast" : "Default";
            Uri source = new Uri($"/Themes/{themeName}.xaml", UriKind.Relative);
            if (ThemeDictionary.Source != source)
            {
                ThemeDictionary.Source = source;
            }
        }

        private void SetMenuDropAlignment()
        {
            if (SystemParameters.MenuDropAlignment && MenuDropAlignmentField != null)
            {
                MenuDropAlignmentField.SetValue(null, false);
            }
        }

        private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SystemParameters.HighContrast):
                    SetTheme();
                    break;
                case nameof(SystemParameters.MenuDropAlignment):
                    SetMenuDropAlignment();
                    break;
            }
        }

        private void OnHandledError(Exception ex)
        {
            Log.Default.Error(ex);
            IDialogService dialog = ServiceProvider.GetDialogService(new DialogInfo(DialogInfoPreset.Error)
            {
                Lead = ResXResources.HandledErrorLead,
                Body = ex.Message,
                Details = ex.ToString()
            });
            dialog.Show();
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
                IDialogService dialog = ServiceProvider.GetDialogService(new DialogInfo(DialogInfoPreset.Default)
                {
                    Lead = ResXResources.SettingsResetLead
                });
                dialog.Show();
            }
        }
    }
}
