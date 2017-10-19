using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private Configuration configuration;

        public ICollection<string> LogLevelNames { get; private set; }

        private string logLevelName;
        [DirtyCheck]
        public string LogLevelName
        {
            get { return logLevelName; }
            set { Set(nameof(LogLevelName), ref logLevelName, value); }
        }

        private string rootPath;
        [DirtyCheck]
        public string RootPath
        {
            get { return rootPath; }
            set { Set(nameof(RootPath), ref rootPath, value); }
        }

        public ObservableCollection<Role> Roles { get; private set; }

        private string emailHost;
        [DirtyCheck]
        public string EmailHost
        {
            get { return emailHost; }
            set { Set(nameof(EmailHost), ref emailHost, value); }
        }

        private int? emailPort;
        [DirtyCheck]
        public int? EmailPort
        {
            get { return emailPort; }
            set { Set(nameof(EmailPort), ref emailPort, value); }
        }

        private bool emailUseSsl;
        [DirtyCheck]
        public bool EmailUseSsl
        {
            get { return emailUseSsl; }
            set { Set(nameof(EmailUseSsl), ref emailUseSsl, value); }
        }

        private string emailSender;
        [DirtyCheck]
        public string EmailSender
        {
            get { return emailSender; }
            set { Set(nameof(EmailSender), ref emailSender, value); }
        }

        private string emailPassword;
        [DirtyCheck]
        public string EmailPassword
        {
            get { return emailPassword; }
            set { Set(nameof(EmailPassword), ref emailPassword, value); }
        }

        private string mapApplicationId;
        [DirtyCheck]
        public string MapApplicationId
        {
            get { return mapApplicationId; }
            set { Set(nameof(MapApplicationId), ref mapApplicationId, value); }
        }

        private string endpointAddress;
        [DirtyCheck]
        public string EndpointAddress
        {
            get { return endpointAddress; }
            set { Set(nameof(EndpointAddress), ref endpointAddress, value); }
        }

        private bool windowsAuthentication;
        [DirtyCheck]
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
                    if (value)
                    {
                        BindingType = BindingType.BasicHttp;
                    }
                }
            }
        }

        public ICollection<BindingType> BindingTypes { get; private set; }

        private BindingType bindingType;
        [DirtyCheck]
        public BindingType BindingType
        {
            get { return bindingType; }
            set { Set(nameof(BindingType), ref bindingType, value); }
        }

        private string organizationName;
        [DirtyCheck]
        public string OrganizationName
        {
            get { return organizationName; }
            set { Set(nameof(OrganizationName), ref organizationName, value); }
        }

        private Guid? organizationKey;
        [DirtyCheck]
        public Guid? OrganizationKey
        {
            get { return organizationKey; }
            set { Set(nameof(OrganizationKey), ref organizationKey, value); }
        }

        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand ShowDataSourcesCommand { get; private set; }
        public RelayCommand AddRoleCommand { get; private set; }
        public RelayCommand<Role> RemoveRoleCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public SettingsViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Settings";
            configuration = Configuration.GetNewInstance();
            LogLevelNames = Log.LevelNames;
            LogLevelName = Settings.Default.LogLevelName;
            RootPath = configuration.GetRootPath();
            Roles = new ObservableCollection<Role>();
            if (Context != null)
            {
                Roles.AddRange(Context.Roles.Select().OrderBy(role => role.Name));
            }
            EmailHost = Settings.Default.EmailHost;
            EmailPort = Settings.Default.EmailPort;
            EmailUseSsl = Settings.Default.EmailUseSsl;
            EmailSender = Settings.Default.EmailSender;
            EmailPassword = ConfigurationExtensions.DecryptSafe(Settings.Default.EmailPassword);
            MapApplicationId = Settings.Default.MapApplicationId;
            EndpointAddress = configuration.Settings.WebServiceEndpointAddress;
            WindowsAuthentication = configuration.Settings.WebServiceAuthMode == 1;
            BindingTypes = EnumExtensions.GetValues<BindingType>().ToList();
            BindingType = BindingTypeExtensions.EpiInfoValues.Reverse(configuration.Settings.WebServiceBindingMode);
            OrganizationName = Settings.Default.OrganizationName;
            OrganizationKey = ConvertExtensions.ToNullableGuid(Settings.Default.OrganizationKey);
            Dirty = false;
            BrowseCommand = new RelayCommand(Browse);
            ShowDataSourcesCommand = new RelayCommand(ShowDataSources);
            AddRoleCommand = new RelayCommand(AddRole);
            RemoveRoleCommand = new RelayCommand<Role>(RemoveRole);
            SaveCommand = new RelayCommand(Save);
            Roles.CollectionChanged += (sender, e) =>
            {
                Dirty = true;
            };
        }

        public void Browse()
        {
            using (FolderBrowserDialog dialog = RootPathDialog.GetDialog())
            {
                if (dialog.ShowDialog(Dialogs.Win32Window) == DialogResult.OK)
                {
                    string path = dialog.GetRootPath();
                    if (!path.EqualsIgnoreCase(configuration.GetRootPath()))
                    {
                        ICollection<string> message = new List<string>();
                        message.Add("Change the root directory?");
                        message.Add("This may cause existing data sources to stop working properly.");
                        message.Add(string.Format("{0} will attempt to copy your documents and restart when settings are saved.", App.Title));
                        ConfirmMessage msg = new ConfirmMessage
                        {
                            Verb = "Change",
                            Message = string.Join(" ", message)
                        };
                        msg.Confirmed += (sender, e) =>
                        {
                            Log.Logger.DebugFormat("Root path chosen: {0}", path);
                            RootPath = path;
                        };
                        MessengerInstance.Send(msg);
                    }
                }
            }
        }

        public void ShowDataSources()
        {
            Services.Documents.ShowDataSources();
        }

        public void AddRole()
        {
            RoleViewModel role = new RoleViewModel(Services, "Add");
            role.Saved += (sender, e) =>
            {
                Roles.Add(new Role(true)
                {
                    Name = role.Name
                });
            };
            Dialogs.ShowAsync(role);
        }

        public void RemoveRole(Role role)
        {
            Roles.Remove(role);
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (!string.IsNullOrWhiteSpace(EmailSender) && !MailExtensions.IsValidAddress(EmailSender))
            {
                fields.Add("Sender Address");
            }
            Uri endpoint;
            if (!string.IsNullOrWhiteSpace(EndpointAddress) && !Uri.TryCreate(EndpointAddress, UriKind.Absolute, out endpoint))
            {
                fields.Add("Endpoint Address");
            }
            if (fields.Count > 0)
            {
                ShowValidationMessage(ValidationError.Invalid, fields);
                return false;
            }
            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            Settings.Default.LogLevelName = LogLevelName;
            if (Context != null)
            {
                Context.Database.Transact((connection, transaction) =>
                {
                    Context.Roles.Delete();
                    foreach (Role role in Roles)
                    {
                        Context.Roles.Insert(role);
                    }
                });
            }
            Settings.Default.EmailHost = EmailHost;
            Settings.Default.EmailPort = EmailPort;
            Settings.Default.EmailUseSsl = EmailUseSsl;
            Settings.Default.EmailSender = EmailSender;
            Settings.Default.EmailPassword = ConfigurationExtensions.EncryptSafe(EmailPassword);
            Settings.Default.MapApplicationId = MapApplicationId;
            configuration.Settings.WebServiceEndpointAddress = EndpointAddress;
            configuration.Settings.WebServiceAuthMode = WindowsAuthentication ? 1 : 0;
            configuration.Settings.WebServiceBindingMode = BindingTypeExtensions.EpiInfoValues.Forward(BindingType);
            Settings.Default.OrganizationName = OrganizationName;
            Settings.Default.OrganizationKey = OrganizationKey?.ToString();
            string rootPathInit = configuration.GetRootPath();
            if (!RootPath.EqualsIgnoreCase(rootPathInit))
            {
                try
                {
                    using (new WaitCursor())
                    {
                        IOExtensions.CopyDirectory(rootPathInit, RootPath);
                        configuration.SetUserDirectories(RootPath);
                        ICollection<string> paths = Settings.Default.DataSourcePaths.ToList();
                        Settings.Default.DataSourcePaths.Clear();
                        Regex rootPathInitPattern = new Regex(@"^" + Regex.Escape(rootPathInit), RegexOptions.IgnoreCase);
                        foreach (string path in paths)
                        {
                            Settings.Default.DataSourcePaths.Add(rootPathInitPattern.Replace(path, RootPath));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Warn("Failed to initialize root path", ex);
                    StringBuilder message = new StringBuilder();
                    message.AppendFormat("{0} failed to initialize the following directory. Settings have not been saved.", App.Title);
                    message.AppendLine();
                    message.AppendLine();
                    message.Append(RootPath);
                    Dialogs.ShowErrorAsync(message.ToString(), ex);
                    return;
                }
            }
            Settings.Default.Save();
            configuration.Save();
            Dirty = false;
            if (RootPath.EqualsIgnoreCase(rootPathInit))
            {
                Log.LevelName = Settings.Default.LogLevelName;
                ConfigurationExtensions.Load();
                MessengerInstance.Send(new ToastMessage
                {
                    Message = "Settings have been saved."
                });
            }
            else
            {
                MessengerInstance.Send(new ShutdownMessage
                {
                    Restart = true
                });
            }
        }
    }
}
