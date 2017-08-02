using Epi;
using ERHMS.Dapper;
using ERHMS.DataAccess;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    public class DataSourceViewModel : DialogViewModel
    {
        public static bool Add(string path)
        {
            if (Settings.Default.DataSourcePaths.Add(path))
            {
                Settings.Default.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Remove(string path)
        {
            if (Settings.Default.DataSourcePaths.Remove(path))
            {
                Settings.Default.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { Set(nameof(Name), ref name, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { Set(nameof(Description), ref description, value); }
        }

        private string location;
        public string Location
        {
            get { return location; }
            set { Set(nameof(Location), ref location, value); }
        }

        public string FullName
        {
            get { return Path.Combine(Location, Name, Path.ChangeExtension(Name, Project.FileExtension)); }
        }

        public ICollection<DbProvider> Providers { get; private set; }

        private DbProvider provider;
        public DbProvider Provider
        {
            get { return provider; }
            set { Set(nameof(Provider), ref provider, value); }
        }

        private string dataSource;
        public string DataSource
        {
            get { return dataSource; }
            set { Set(nameof(DataSource), ref dataSource, value); }
        }

        private string initialCatalog;
        public string InitialCatalog
        {
            get { return initialCatalog; }
            set { Set(nameof(InitialCatalog), ref initialCatalog, value); }
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
                if (Set(nameof(IntegratedSecurity), ref integratedSecurity, value))
                {
                    if (value)
                    {
                        UserId = null;
                        Password = null;
                    }
                }
            }
        }

        private string userId;
        public string UserId
        {
            get { return userId; }
            set { Set(nameof(UserId), ref userId, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { Set(nameof(Password), ref password, value); }
        }

        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand CreateCommand { get; private set; }
        public DataSourceViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Create a Data Source";
            Location = Configuration.GetNewInstance().Directories.Project;
            Providers = EnumExtensions.GetValues<DbProvider>().ToList();
            Provider = DbProvider.Access;
            BrowseCommand = new RelayCommand(Browse);
            CreateCommand = new RelayCommand(Create);
        }

        private bool Validate()
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
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            if (Name.IndexOfAny(Path.GetInvalidPathChars()) != -1 || Name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                ShowValidationMessage(ValidationError.Invalid, fields);
                return false;
            }
            return true;
        }

        public void Browse()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog(Dialogs.Win32Window) == DialogResult.OK)
                {
                    Location = dialog.SelectedPath;
                }
            }
        }

        public void Add()
        {
            Add(FullName);
            MessengerInstance.Send(new RefreshMessage(typeof(ProjectInfo)));
        }

        public void Create()
        {
            if (!Validate())
            {
                return;
            }
            try
            {
                if (File.Exists(FullName))
                {
                    ConfirmMessage msg = new ConfirmMessage
                    {
                        Verb = "Add",
                        Message = "Data source already exists. Add it to your list of data sources?"
                    };
                    msg.Confirmed += (sender, e) =>
                    {
                        Add();
                        Close();
                    };
                    MessengerInstance.Send(msg);
                }
                else
                {
                    IDatabase database;
                    switch (Provider)
                    {
                        case DbProvider.Access:
                            database = AccessDatabase.Construct(Path.ChangeExtension(FullName, ".mdb"));
                            break;
                        case DbProvider.SqlServer:
                            database = SqlServerDatabase.Construct(DataSource, InitialCatalog, UserId, Password);
                            break;
                        default:
                            throw new InvalidEnumValueException(Provider);
                    }
                    if (database.Exists())
                    {
                        ConfirmMessage msg = new ConfirmMessage
                        {
                            Verb = "Add",
                            Message = "Database already exists. Add a data source using this database?"
                        };
                        msg.Confirmed += (sender, e) =>
                        {
                            Create(database, false);
                            Close();
                        };
                        MessengerInstance.Send(msg);
                    }
                    else
                    {
                        BlockMessage msg = new BlockMessage
                        {
                            Message = "Creating data source \u2026"
                        };
                        msg.Executing += (sender, e) =>
                        {
                            Create(database, true);
                            Close();
                        };
                        msg.Executed += (sender, e) =>
                        {
                            MessengerInstance.Send(new ToastMessage
                            {
                                Message = "Data source has been created."
                            });
                        };
                        MessengerInstance.Send(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to create data source", ex);
                Dialogs.ShowErrorAsync("Failed to create data source.", ex);
            }
        }

        private void Create(IDatabase database, bool initialize)
        {
            try
            {
                if (initialize)
                {
                    database.Create();
                }
                Project project = Project.Create(new ProjectCreationInfo
                {
                    Name = Name,
                    Description = Description,
                    Location = Path.Combine(Location, Name),
                    Driver = DbProviderExtensions.EpiInfoNames.Forward(Provider),
                    Builder = database.Builder,
                    DatabaseName = database.Name,
                    Initialize = initialize
                });
                if (initialize)
                {
                    DataContext.Create(project);
                }
                Add();
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to create data source", ex);
                Dialogs.ShowErrorAsync("Failed to create data source.", ex);
            }
        }
    }
}
