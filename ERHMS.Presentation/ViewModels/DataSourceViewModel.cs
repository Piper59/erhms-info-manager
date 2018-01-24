using Epi;
using ERHMS.Dapper;
using ERHMS.DataAccess;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Presentation.ViewModels
{
    public class DataSourceViewModel : DialogViewModel
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(nameof(Name), ref name, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(nameof(Description), ref description, value); }
        }

        private string location;
        public string Location
        {
            get { return location; }
            set { SetProperty(nameof(Location), ref location, value); }
        }

        public string FilePath
        {
            get { return Path.Combine(Location, Name, Name + Project.FileExtension); }
        }

        public ICollection<DbProvider> Providers { get; private set; }

        private DbProvider provider;
        public DbProvider Provider
        {
            get { return provider; }
            set { SetProperty(nameof(Provider), ref provider, value); }
        }

        private string dataSource;
        public string DataSource
        {
            get { return dataSource; }
            set { SetProperty(nameof(DataSource), ref dataSource, value); }
        }

        private string initialCatalog;
        public string InitialCatalog
        {
            get { return initialCatalog; }
            set { SetProperty(nameof(InitialCatalog), ref initialCatalog, value); }
        }

        private bool encrypt;
        public bool Encrypt
        {
            get { return encrypt; }
            set { SetProperty(nameof(Encrypt), ref encrypt, value); }
        }

        private bool integratedSecurity;
        public bool IntegratedSecurity
        {
            get
            {
                return integratedSecurity;
            }
            set
            {
                SetProperty(nameof(IntegratedSecurity), ref integratedSecurity, value);
                if (value)
                {
                    UserId = null;
                    Password = null;
                }
            }
        }

        private string userId;
        public string UserId
        {
            get { return userId; }
            set { SetProperty(nameof(UserId), ref userId, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(nameof(Password), ref password, value); }
        }

        public ICommand BrowseCommand { get; private set; }
        public ICommand CreateCommand { get; private set; }

        public DataSourceViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Create a Data Source";
            Location = Configuration.GetNewInstance().Directories.Project;
            Providers = EnumExtensions.GetValues<DbProvider>().ToList();
            Provider = DbProvider.Access;
            BrowseCommand = new Command(Browse);
            CreateCommand = new AsyncCommand(CreateAsync);
        }

        public event EventHandler Added;
        private void OnAdded(EventArgs e)
        {
            Added?.Invoke(this, e);
        }
        private void OnAdded()
        {
            OnAdded(EventArgs.Empty);
        }

        public void Browse()
        {
            string path = Services.Dialog.OpenFolder();
            if (path != null)
            {
                Location = path;
            }
        }

        private async Task<bool> ValidateAsync()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Name))
            {
                fields.Add("Name");
            }
            if (Provider == DbProvider.SqlServer)
            {
                if (string.IsNullOrWhiteSpace(DataSource))
                {
                    fields.Add("Server Name");
                }
                if (string.IsNullOrWhiteSpace(InitialCatalog))
                {
                    fields.Add("Database Name");
                }
            }
            if (fields.Count > 0)
            {
                await Services.Dialog.AlertAsync(ValidationError.Required, fields);
                return false;
            }
            if (Name.IndexOfAny(Path.GetInvalidPathChars()) != -1 || Name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                await Services.Dialog.AlertAsync(ValidationError.Invalid, fields);
                return false;
            }
            return true;
        }

        public async Task CreateAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            try
            {
                if (File.Exists(FilePath))
                {
                    if (await Services.Dialog.ConfirmAsync("Data source already exists. Add it to the list of data sources?", "Add"))
                    {
                        OnAdded();
                        Close();
                    }
                }
                else
                {
                    IDatabase database;
                    switch (Provider)
                    {
                        case DbProvider.Access:
                            database = AccessDatabase.Construct(Path.ChangeExtension(FilePath, OleDbExtensions.FileExtensions.Access));
                            break;
                        case DbProvider.SqlServer:
                            database = SqlServerDatabase.Construct(DataSource, InitialCatalog, Encrypt, UserId, Password);
                            break;
                        default:
                            throw new InvalidEnumValueException(Provider);
                    }
                    if (database.Exists())
                    {
                        if (await Services.Dialog.ConfirmAsync("Database already exists. Add a data source using this database?", "Add"))
                        {
                            await CreateAsync(database, false, !Project.IsInitialized(database));
                        }
                    }
                    else
                    {
                        await CreateAsync(database, true, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to create data source", ex);
                await Services.Dialog.AlertAsync("Failed to create data source.", ex);
            }
        }

        private async Task CreateAsync(IDatabase database, bool create, bool initialize)
        {
            await Services.Dialog.BlockAsync("Creating data source \u2026", () =>
            {
                if (create)
                {
                    database.Create();
                }
                Project project = Project.Create(new ProjectCreationInfo
                {
                    Name = Name,
                    Description = Description,
                    Location = Path.Combine(Location, Name),
                    Driver = DbProviderExtensions.EpiInfoValues.Forward(Provider),
                    Builder = database.Builder,
                    DatabaseName = database.Name,
                    Initialize = initialize
                });
                if (initialize)
                {
                    DataContext.Create(project);
                }
            });
            Services.Dialog.Notify("Data source has been created.");
            OnAdded();
            Close();
        }
    }
}
