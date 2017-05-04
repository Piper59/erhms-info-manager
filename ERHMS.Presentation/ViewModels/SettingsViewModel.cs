using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Infrastructure;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
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
                set { Set(nameof(Host), ref host, value); }
            }

            private int? port;
            public int? Port
            {
                get { return port; }
                set { Set(nameof(Port), ref port, value); }
            }

            private string sender;
            public string Sender
            {
                get { return sender; }
                set { Set(nameof(Sender), ref sender, value); }
            }
        }

        public class WebSurveySettingsViewModel : ViewModelBase
        {
            public ICollection<BindingType> BindingTypes { get; private set; }

            private string address;
            public string Address
            {
                get { return address; }
                set { Set(nameof(Address), ref address, value); }
            }

            private bool windowsAuthentication;
            public bool WindowsAuthentication
            {
                get
                {
                    return windowsAuthentication;
                }
                set
                {
                    if (Set(nameof(WindowsAuthentication), ref windowsAuthentication, value))
                    {
                        if (WindowsAuthentication)
                        {
                            BindingType = BindingType.BasicHttp;
                        }
                    }
                }
            }

            private BindingType bindingType;
            public BindingType BindingType
            {
                get { return bindingType; }
                set { Set(nameof(BindingType), ref bindingType, value); }
            }

            private string organizationName;
            public string OrganizationName
            {
                get { return organizationName; }
                set { Set(nameof(OrganizationName), ref organizationName, value); }
            }

            private Guid? organizationKey;
            public Guid? OrganizationKey
            {
                get { return organizationKey; }
                set { Set(nameof(OrganizationKey), ref organizationKey, value); }
            }

            public WebSurveySettingsViewModel()
            {
                BindingTypes = EnumExtensions.GetValues<BindingType>().ToList();
            }
        }

        public ICollection<string> LogLevels { get; private set; }

        private string logLevel;
        [DirtyCheck]
        public string LogLevel
        {
            get { return logLevel; }
            set { Set(nameof(LogLevel), ref logLevel, value); }
        }

        private string rootDirectoryInit;

        private string rootDirectory;
        [DirtyCheck]
        public string RootDirectory
        {
            get { return rootDirectory; }
            set { Set(nameof(RootDirectory), ref rootDirectory, value); }
        }

        public EmailSettingsViewModel EmailSettings { get; private set; }

        private string mapLicenseKey;
        [DirtyCheck]
        public string MapLicenseKey
        {
            get { return mapLicenseKey; }
            set { Set(nameof(MapLicenseKey), ref mapLicenseKey, value); }
        }

        public WebSurveySettingsViewModel WebSurveySettings { get; private set; }

        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand GetMapLicenseKeyCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public SettingsViewModel()
        {
            Title = "Settings";
            LogLevels = Log.LevelNames.ToList();
            LogLevel = Settings.Default.LogLevel;
            Configuration configuration = Configuration.GetNewInstance();
            rootDirectoryInit = new DirectoryInfo(configuration.Directories.Project).Parent.FullName;
            RootDirectory = rootDirectoryInit;
            EmailSettings = new EmailSettingsViewModel
            {
                Host = Settings.Default.EmailHost,
                Port = Settings.Default.EmailPort,
                Sender = Settings.Default.EmailSender
            };
            AddDirtyCheck(EmailSettings);
            MapLicenseKey = Settings.Default.MapLicenseKey;
            WebSurveySettings = new WebSurveySettingsViewModel
            {
                Address = configuration.Settings.WebServiceEndpointAddress,
                WindowsAuthentication = configuration.Settings.WebServiceAuthMode == 1,
                BindingType = BindingTypeExtensions.FromEpiInfoName(configuration.Settings.WebServiceBindingMode),
                OrganizationName = Settings.Default.OrganizationName,
                OrganizationKey = ConvertExtensions.ToNullableGuid(Settings.Default.OrganizationKey)
            };
            AddDirtyCheck(WebSurveySettings);
            Dirty = false;
            BrowseCommand = new RelayCommand(Browse);
            GetMapLicenseKeyCommand = new RelayCommand(GetMapLicenseKey);
            SaveCommand = new RelayCommand(Save);
        }

        public void Browse()
        {
            using (FolderBrowserDialog dialog = RootDirectoryDialog.GetDialog())
            {
                if (dialog.ShowDialog(App.Current.MainWin32Window) == DialogResult.OK)
                {
                    string path = dialog.GetRootDirectory();
                    if (!path.EqualsIgnoreCase(rootDirectoryInit))
                    {
                        ConfirmMessage msg = new ConfirmMessage
                        {
                            Verb = "Change",
                            Message = string.Format((string)App.Current.FindResource("ChangeRootDirectoryText"), App.Title)
                        };
                        msg.Confirmed += (sender, e) =>
                        {
                            Log.Logger.DebugFormat("Root directory chosen: {0}", path);
                            RootDirectory = path;
                        };
                        Messenger.Default.Send(msg);
                    }
                }
            }
        }

        public void GetMapLicenseKey()
        {
            Process.Start("https://www.bingmapsportal.com/");
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (!string.IsNullOrWhiteSpace(EmailSettings.Sender) && !MailExtensions.IsValidAddress(EmailSettings.Sender))
            {
                fields.Add("Sender Address");
            }
            Uri result;
            if (!string.IsNullOrWhiteSpace(WebSurveySettings.Address) && !Uri.TryCreate(WebSurveySettings.Address, UriKind.Absolute, out result))
            {
                fields.Add("Endpoint Address");
            }
            if (fields.Count > 0)
            {
                ShowInvalidMessage(fields);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            Settings.Default.LogLevel = LogLevel;
            Settings.Default.EmailHost = EmailSettings.Host;
            Settings.Default.EmailPort = EmailSettings.Port;
            Settings.Default.EmailSender = EmailSettings.Sender;
            Settings.Default.MapLicenseKey = MapLicenseKey;
            Configuration configuration = Configuration.GetNewInstance();
            configuration.Settings.WebServiceEndpointAddress = WebSurveySettings.Address;
            configuration.Settings.WebServiceAuthMode = WebSurveySettings.WindowsAuthentication ? 1 : 0;
            configuration.Settings.WebServiceBindingMode = WebSurveySettings.BindingType.ToEpiInfoName();
            Settings.Default.OrganizationName = WebSurveySettings.OrganizationName;
            Settings.Default.OrganizationKey = WebSurveySettings.OrganizationKey.ToString();
            if (!RootDirectory.EqualsIgnoreCase(rootDirectoryInit))
            {
                // TODO: Close data source?
                try
                {
                    configuration.ChangeUserDirectories(RootDirectory);
                    ICollection<string> paths = Settings.Default.DataSources.ToList();
                    Settings.Default.DataSources.Clear();
                    Regex rootDirectoryInitPattern = new Regex("^" + Regex.Escape(rootDirectoryInit), RegexOptions.IgnoreCase);
                    foreach (string path in paths)
                    {
                        Settings.Default.DataSources.Add(rootDirectoryInitPattern.Replace(path, RootDirectory));
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Warn("Failed to initialize root directory", ex);
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("{0} failed to initialize the following folder. Settings have not been saved.", App.Title);
                    builder.AppendLine();
                    builder.AppendLine();
                    builder.Append(RootDirectory);
                    Messenger.Default.Send(new AlertMessage
                    {
                        Message = builder.ToString()
                    });
                    return;
                }
            }
            Settings.Default.Save();
            configuration.Save();
            Dirty = false;
            if (RootDirectory.EqualsIgnoreCase(rootDirectoryInit))
            {
                Log.SetLevelName(Settings.Default.LogLevel);
                ConfigurationExtensions.Load();
                Messenger.Default.Send(new ToastMessage
                {
                    Message = "Settings have been saved."
                });
            }
            else
            {
                App.Current.Shutdown();
                Application.Restart();
            }
        }
    }
}
