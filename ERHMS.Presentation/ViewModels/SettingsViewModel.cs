using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mantin.Controls.Wpf.Notification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Binding = ERHMS.EpiInfo.Web.Binding;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public class EmailSettingsViewModel : ViewModelBase
        {
            private string host;
            public string Host
            {
                get { return host; }
                set { Set(() => Host, ref host, value); }
            }

            private int? port;
            public int? Port
            {
                get { return port; }
                set { Set(() => Port, ref port, value); }
            }

            private string sender;
            public string Sender
            {
                get { return sender; }
                set { Set(() => Sender, ref sender, value); }
            }
        }

        public class WebSurveySettingsViewModel : ViewModelBase
        {
            public ICollection<Binding> Bindings { get; private set; }

            private string address;
            public string Address
            {
                get { return address; }
                set { Set(() => Address, ref address, value); }
            }

            private bool windowsAuthentication;
            public bool WindowsAuthentication
            {
                get { return windowsAuthentication; }
                set { Set(() => WindowsAuthentication, ref windowsAuthentication, value); }
            }

            private Binding binding;
            public Binding Binding
            {
                get { return binding; }
                set { Set(() => Binding, ref binding, value); }
            }

            private string organizationName;
            public string OrganizationName
            {
                get { return organizationName; }
                set { Set(() => OrganizationName, ref organizationName, value); }
            }

            private Guid? organizationKey;
            public Guid? OrganizationKey
            {
                get { return organizationKey; }
                set { Set(() => OrganizationKey, ref organizationKey, value); }
            }

            public WebSurveySettingsViewModel()
            {
                Bindings = EnumExtensions.GetValues<Binding>().ToList();
            }
        }

        public ICollection<string> LogLevels { get; private set; }

        private string logLevel;
        public string LogLevel
        {
            get { return logLevel; }
            set { Set(() => LogLevel, ref logLevel, value); }
        }

        private string rootDirectory;
        public string RootDirectory
        {
            get { return rootDirectory; }
            set { Set(() => RootDirectory, ref rootDirectory, value); }
        }

        private bool rootDirectoryChanged;
        public bool RootDirectoryChanged
        {
            get { return rootDirectoryChanged; }
            set { Set(() => RootDirectoryChanged, ref rootDirectoryChanged, value); }
        }

        public EmailSettingsViewModel Email { get; private set; }

        private string mapLicenseKey;
        public string MapLicenseKey
        {
            get { return mapLicenseKey; }
            set { Set(() => MapLicenseKey, ref mapLicenseKey, value); }
        }

        public WebSurveySettingsViewModel WebSurvey { get; private set; }

        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public SettingsViewModel()
        {
            Title = "Settings";
            LogLevels = new List<string>
            {
                "DEBUG",
                "INFO",
                "WARN",
                "ERROR",
                "FATAL"
            };
            Configuration configuration = Configuration.GetNewInstance();
            LogLevel = Settings.Default.LogLevel;
            RootDirectory = Settings.Default.RootDirectory;
            Email = new EmailSettingsViewModel
            {
                Host = Settings.Default.EmailHost,
                Port = Settings.Default.EmailPort,
                Sender = Settings.Default.EmailSender
            };
            MapLicenseKey = Settings.Default.MapLicenseKey;
            WebSurvey = new WebSurveySettingsViewModel
            {
                Address = configuration.Settings.WebServiceEndpointAddress,
                WindowsAuthentication = configuration.Settings.WebServiceAuthMode == 1,
                Binding = BindingExtensions.FromEpiInfoName(configuration.Settings.WebServiceBindingMode),
                OrganizationName = Settings.Default.OrganizationName,
                OrganizationKey = Settings.Default.WebSurveyKey
            };
            BrowseCommand = new RelayCommand(Browse);
            SaveCommand = new RelayCommand(Save);
        }

        public void Browse()
        {
            using (FolderBrowserDialog dialog = RootDirectoryDialog.GetDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ConfirmMessage msg = new ConfirmMessage(
                        "Change?",
                        string.Format("Change root directory? {0} will copy your documents and restart when settings are saved.", App.Title),
                        "Change",
                        "Don't Change");
                    msg.Confirmed += (sender, e) =>
                    {
                        RootDirectory = dialog.GetRootDirectory();
                        RootDirectoryChanged = true;
                    };
                    Messenger.Default.Send(msg);
                }
            }
        }

        public void Save()
        {
            // TODO: Validate fields
            Configuration configuration = Configuration.GetNewInstance();
            Settings.Default.LogLevel = LogLevel;
            Settings.Default.EmailHost = Email.Host;
            Settings.Default.EmailPort = Email.Port;
            Settings.Default.EmailSender = Email.Sender;
            Settings.Default.MapLicenseKey = MapLicenseKey;
            configuration.Settings.WebServiceEndpointAddress = WebSurvey.Address;
            configuration.Settings.WebServiceAuthMode = WebSurvey.WindowsAuthentication ? 1 : 0;
            configuration.Settings.WebServiceBindingMode = WebSurvey.Binding.ToEpiInfoName();
            Settings.Default.OrganizationName = WebSurvey.OrganizationName;
            Settings.Default.WebSurveyKey = WebSurvey.OrganizationKey;
            if (RootDirectoryChanged)
            {
                configuration = ConfigurationExtensions.ChangeRoot(configuration, new DirectoryInfo(RootDirectory));
                ICollection<string> dataSources = Settings.Default.DataSources.ToList();
                Settings.Default.DataSources.Clear();
                foreach (string dataSource in dataSources)
                {
                    Settings.Default.DataSources.Add(dataSource.Replace(Settings.Default.RootDirectory, RootDirectory));
                }
                Settings.Default.RootDirectory = RootDirectory;
                Settings.Default.Save();
                configuration.Save();
                App.Current.Shutdown();
                Application.Restart();
            }
            else
            {
                Settings.Default.Save();
                configuration.Refresh(true);
                Log.SetLevelName(Settings.Default.LogLevel);
                Messenger.Default.Send(new ToastMessage(NotificationType.Information, "Settings have been saved."));
            }
        }
    }
}
