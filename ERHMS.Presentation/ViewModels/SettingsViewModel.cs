using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    public class SettingsViewModel : DocumentViewModel
    {
        private Configuration configuration;

        public ICollection<string> LogLevelNames { get; private set; }

        private string logLevelName;
        [DirtyCheck]
        public string LogLevelName
        {
            get { return logLevelName; }
            set { SetProperty(nameof(LogLevelName), ref logLevelName, value); }
        }

        private string rootPath;
        [DirtyCheck]
        public string RootPath
        {
            get { return rootPath; }
            set { SetProperty(nameof(RootPath), ref rootPath, value); }
        }

        public ObservableCollection<Role> Roles { get; private set; }

        private string emailHost;
        [DirtyCheck]
        public string EmailHost
        {
            get { return emailHost; }
            set { SetProperty(nameof(EmailHost), ref emailHost, value); }
        }

        private int? emailPort;
        [DirtyCheck]
        public int? EmailPort
        {
            get { return emailPort; }
            set { SetProperty(nameof(EmailPort), ref emailPort, value); }
        }

        private bool emailUseSsl;
        [DirtyCheck]
        public bool EmailUseSsl
        {
            get { return emailUseSsl; }
            set { SetProperty(nameof(EmailUseSsl), ref emailUseSsl, value); }
        }

        private string emailSender;
        [DirtyCheck]
        public string EmailSender
        {
            get { return emailSender; }
            set { SetProperty(nameof(EmailSender), ref emailSender, value); }
        }

        private string emailPassword;
        [DirtyCheck]
        public string EmailPassword
        {
            get { return emailPassword; }
            set { SetProperty(nameof(EmailPassword), ref emailPassword, value); }
        }

        private string mapApplicationId;
        [DirtyCheck]
        public string MapApplicationId
        {
            get { return mapApplicationId; }
            set { SetProperty(nameof(MapApplicationId), ref mapApplicationId, value); }
        }

        private string endpointAddress;
        [DirtyCheck]
        public string EndpointAddress
        {
            get { return endpointAddress; }
            set { SetProperty(nameof(EndpointAddress), ref endpointAddress, value); }
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
                SetProperty(nameof(WindowsAuthentication), ref windowsAuthentication, value);
                if (value)
                {
                    BindingType = BindingType.BasicHttp;
                }
            }
        }

        public ICollection<BindingType> BindingTypes { get; private set; }

        private BindingType bindingType;
        [DirtyCheck]
        public BindingType BindingType
        {
            get { return bindingType; }
            set { SetProperty(nameof(BindingType), ref bindingType, value); }
        }

        private string organizationName;
        [DirtyCheck]
        public string OrganizationName
        {
            get { return organizationName; }
            set { SetProperty(nameof(OrganizationName), ref organizationName, value); }
        }

        private Guid? organizationKey;
        [DirtyCheck]
        public Guid? OrganizationKey
        {
            get { return organizationKey; }
            set { SetProperty(nameof(OrganizationKey), ref organizationKey, value); }
        }

        public ICommand BrowseCommand { get; private set; }
        public ICommand ShowDataSourcesCommand { get; private set; }
        public ICommand AddRoleCommand { get; private set; }
        public ICommand RemoveRoleCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public SettingsViewModel()
        {
            Title = "Settings";
            configuration = Configuration.GetNewInstance();
            LogLevelNames = Log.LevelNames;
            LogLevelName = Settings.Default.LogLevelName;
            RootPath = configuration.GetRootPath();
            Roles = new ObservableCollection<Role>();
            if (Context != null)
            {
                Roles.AddRange(Context.Roles.Select().OrderBy(role => role.Name, StringComparer.OrdinalIgnoreCase));
            }
            Roles.CollectionChanged += (sender, e) =>
            {
                Dirty = true;
            };
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
            BrowseCommand = new AsyncCommand(BrowseAsync);
            ShowDataSourcesCommand = new Command(ShowDataSources);
            AddRoleCommand = new AsyncCommand(AddRoleAsync);
            RemoveRoleCommand = new Command<Role>(RemoveRole);
            SaveCommand = new AsyncCommand(SaveAsync);
        }

        public async Task BrowseAsync()
        {
            string path = ServiceLocator.Dialog.GetRootPath();
            if (path != null && !path.Equals(configuration.GetRootPath(), StringComparison.OrdinalIgnoreCase))
            {
                if (await ServiceLocator.Dialog.ConfirmAsync(Resources.SettingsConfirmChangeRootDirectory, "Change"))
                {
                    Log.Logger.DebugFormat("Root path chosen: {0}", path);
                    RootPath = path;
                }
            }
        }

        public void ShowDataSources()
        {
            ServiceLocator.Document.ShowByType(() => new DataSourceListViewModel());
        }

        public async Task AddRoleAsync()
        {
            RoleViewModel model = new RoleViewModel("Add");
            model.Saved += (sender, e) =>
            {
                Roles.Add(new Role(true)
                {
                    Name = model.Name
                });
            };
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        public void RemoveRole(Role role)
        {
            Roles.Remove(role);
        }

        private async Task<bool> ValidateAsync()
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
                await ServiceLocator.Dialog.AlertAsync(ValidationError.Invalid, fields);
                return false;
            }
            return true;
        }

        public async Task SaveAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            Settings.Default.LogLevelName = LogLevelName;
            if (Context != null)
            {
                using (ServiceLocator.Busy.Begin())
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
            try
            {
                bool restart = SetRootPath(configuration.GetRootPath(), RootPath);
                Settings.Default.Save();
                configuration.Save();
                Dirty = false;
                if (restart)
                {
                    ServiceLocator.App.Restart();
                }
                else
                {
                    Log.LevelName = Settings.Default.LogLevelName;
                    ConfigurationExtensions.Load();
                    ServiceLocator.Dialog.Notify(Resources.SettingsSaved);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to initialize root path", ex);
                string message = string.Format(Resources.SettingsInitializeFailed, RootPath);
                await ServiceLocator.Dialog.ShowErrorAsync(message, ex);
            }
        }

        private bool SetRootPath(string original, string modified)
        {
            if (original.Equals(modified, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            using (ServiceLocator.Busy.Begin())
            {
                IOExtensions.CopyDirectory(original, modified);
                configuration.SetUserDirectories(modified);
                ICollection<string> paths = Settings.Default.DataSourcePaths.ToList();
                Settings.Default.DataSourcePaths.Clear();
                Regex originalPattern = new Regex(@"^" + Regex.Escape(original), RegexOptions.IgnoreCase);
                foreach (string path in paths)
                {
                    Settings.Default.DataSourcePaths.Add(originalPattern.Replace(path, modified));
                }
                Settings.Default.LastDataSourcePath = null;
            }
            return true;
        }
    }
}
