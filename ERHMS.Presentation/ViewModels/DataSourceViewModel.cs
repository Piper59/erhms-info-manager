using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    public class DataSourceViewModel : ViewModelBase
    {
        public class SqlServerDataSourceViewModel : ViewModelBase
        {
            private string dataSource;
            public string DataSource
            {
                get { return dataSource; }
                set { Set(() => DataSource, ref dataSource, value); }
            }

            private string initialCatalog;
            public string InitialCatalog
            {
                get { return initialCatalog; }
                set { Set(() => InitialCatalog, ref initialCatalog, value); }
            }

            private bool integratedSecurity;
            public bool IntegratedSecurity
            {
                get { return integratedSecurity; }
                set { Set(() => IntegratedSecurity, ref integratedSecurity, value); }
            }

            private string userId;
            public string UserId
            {
                get { return userId; }
                set { Set(() => UserId, ref userId, value); }
            }

            private string password;
            public string Password
            {
                get { return password; }
                set { Set(() => Password, ref password, value); }
            }

            public SqlServerDataSourceViewModel()
            {
                PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(IntegratedSecurity))
                    {
                        if (IntegratedSecurity)
                        {
                            UserId = null;
                            Password = null;
                        }
                    }
                };
            }

            public void Reset()
            {
                DataSource = null;
                InitialCatalog = null;
                IntegratedSecurity = true;
                UserId = null;
                Password = null;
            }

            public SqlServerDriver CreateDriver()
            {
                return SqlServerDriver.Create(DataSource, InitialCatalog, UserId, Password);
            }
        }

        public static void Add(FileInfo file)
        {
            Settings.Instance.DataSources.Add(file.FullName);
            Settings.Instance.Save();
        }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(() => Active, ref active, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { Set(() => Name, ref name, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { Set(() => Description, ref description, value); }
        }

        private DirectoryInfo location;
        public DirectoryInfo Location
        {
            get { return location; }
            set { Set(() => Location, ref location, value); }
        }

        private DataProvider provider;
        public DataProvider Provider
        {
            get { return provider; }
            set { Set(() => Provider, ref provider, value); }
        }

        public SqlServerDataSourceViewModel SqlServer { get; private set; }

        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public DataSourceViewModel()
        {
            SqlServer = new SqlServerDataSourceViewModel();
            BrowseCommand = new RelayCommand(Browse);
            CreateCommand = new RelayCommand(Create);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void Reset()
        {
            Configuration configuration = Configuration.GetNewInstance();
            Name = null;
            Description = null;
            Location = new DirectoryInfo(configuration.Directories.Project);
            Provider = DataProvider.Access;
            SqlServer.Reset();
        }

        public void Browse()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Location = new DirectoryInfo(dialog.SelectedPath);
                }
            }
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Name))
            {
                fields.Add("Name");
            }
            if (Provider == DataProvider.SqlServer)
            {
                if (string.IsNullOrWhiteSpace(SqlServer.DataSource))
                {
                    fields.Add("Server Name");
                }
                if (string.IsNullOrWhiteSpace(SqlServer.InitialCatalog))
                {
                    fields.Add("Database Name");
                }
            }
            if (fields.Count > 0)
            {
                NotifyRequired(fields);
                return false;
            }
            else
            {
                if (Name.IndexOfAny(Path.GetInvalidPathChars()) != -1 || Name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    fields.Add("Name");
                }
                if (fields.Count > 0)
                {
                    NotifyInvalid(fields);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void Create(FileInfo file, IDataDriver driver, bool initialize)
        {
            try
            {
                if (initialize)
                {
                    driver.CreateDatabase();
                }
                Project project = Project.Create(
                    Name,
                    Description,
                    file.Directory,
                    driver.Provider.ToEpiInfoName(),
                    driver.Builder,
                    driver.DatabaseName,
                    initialize);
                if (initialize)
                {
                    DataAccess.DataContext.Create(project);
                }
                Add(file);
            }
            catch (Exception ex)
            {
                Log.Current.Warn("Failed to create data source", ex);
                Messenger.Default.Send(new NotifyMessage("Failed to create data source."));
            }
        }

        public void Create()
        {
            if (!Validate())
            {
                return;
            }
            try
            {
                FileInfo file = Location.GetFile(Path.Combine(Name, Path.ChangeExtension(Name, Project.FileExtension)));
                if (file.Exists)
                {
                    ConfirmMessage msg = new ConfirmMessage("Add", "Data source already exists. Add it to your list of data sources?");
                    msg.Confirmed += (sender, e) =>
                    {
                        Add(file);
                        Messenger.Default.Send(new RefreshListMessage<ProjectInfo>());
                        Active = false;
                    };
                    Messenger.Default.Send(msg);
                }
                else
                {
                    IDataDriver driver;
                    switch (Provider)
                    {
                        case DataProvider.Access:
                            driver = AccessDriver.Create(Path.ChangeExtension(file.FullName, ".mdb"));
                            break;
                        case DataProvider.SqlServer:
                            driver = SqlServer.CreateDriver();
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    if (driver.DatabaseExists())
                    {
                        ConfirmMessage msg = new ConfirmMessage("Add", "Database already exists. Add a data source using this database?");
                        msg.Confirmed += (sender, e) =>
                        {
                            Create(file, driver, false);
                            Messenger.Default.Send(new RefreshListMessage<ProjectInfo>());
                            Active = false;
                        };
                        Messenger.Default.Send(msg);
                    }
                    else
                    {
                        BlockMessage msg = new BlockMessage("Creating data source \u2026");
                        msg.Executing += (sender, e) =>
                        {
                            Create(file, driver, true);
                            Messenger.Default.Send(new RefreshListMessage<ProjectInfo>());
                            Active = false;
                        };
                        msg.Executed += (sender, e) =>
                        {
                            Messenger.Default.Send(new ToastMessage("Data source has been created."));
                        };
                        Messenger.Default.Send(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Current.Warn("Failed to create data source", ex);
                Messenger.Default.Send(new NotifyMessage("Failed to create data source."));
            }
        }

        public void Cancel()
        {
            Active = false;
        }
    }
}
