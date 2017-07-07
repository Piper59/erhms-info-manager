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
        public class SqlServerOptionsViewModel : ViewModelBase
        {
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
                        if (IntegratedSecurity)
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

            public IDataDriver CreateDriver()
            {
                return SqlServerDriver.Create(DataSource, InitialCatalog, UserId, Password);
            }
        }

        public static void AddDataSource(string path)
        {
            Settings.Default.DataSourcePaths.Add(path);
            Settings.Default.Save();
            Messenger.Default.Send(new RefreshMessage<ProjectInfo>());
        }

        public static void RemoveDataSource(string path)
        {
            Settings.Default.DataSourcePaths.RemoveWhere(dataSource => dataSource.EqualsIgnoreCase(path));
            Settings.Default.Save();
            Messenger.Default.Send(new RefreshMessage<ProjectInfo>());
        }

        public ICollection<DataProvider> Providers { get; private set; }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
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

        private DataProvider provider;
        public DataProvider Provider
        {
            get { return provider; }
            set { Set(nameof(Provider), ref provider, value); }
        }

        public SqlServerOptionsViewModel SqlServerOptions { get; private set; }

        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public DataSourceViewModel()
        {
            Providers = new DataProvider[]
            {
                DataProvider.Access,
                DataProvider.SqlServer
            };
            Configuration configuration = Configuration.GetNewInstance();
            Location = configuration.Directories.Project;
            Provider = DataProvider.Access;
            SqlServerOptions = new SqlServerOptionsViewModel();
            BrowseCommand = new RelayCommand(Browse);
            CreateCommand = new RelayCommand(Create);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void Browse()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog(App.Current.MainWin32Window) == DialogResult.OK)
                {
                    Location = dialog.SelectedPath;
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
                if (string.IsNullOrWhiteSpace(SqlServerOptions.DataSource))
                {
                    fields.Add("Server Name");
                }
                if (string.IsNullOrWhiteSpace(SqlServerOptions.InitialCatalog))
                {
                    fields.Add("Database Name");
                }
            }
            if (fields.Count > 0)
            {
                ShowRequiredMessage(fields);
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
                    ShowInvalidMessage(fields);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void Create(IDataDriver driver, bool initialize)
        {
            try
            {
                if (initialize)
                {
                    driver.CreateDatabase();
                }
                Project project = Project.Create(new ProjectCreationInfo
                {
                    Name = Name,
                    Description = Description,
                    Location = Path.Combine(Location, Name),
                    Driver = driver.Provider.ToEpiInfoName(),
                    Builder = driver.Builder,
                    DatabaseName = driver.DatabaseName,
                    Initialize = initialize
                });
                if (initialize)
                {
                    DataAccess.DataContext.Create(project);
                }
                AddDataSource(project.FilePath);
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to create data source", ex);
                Messenger.Default.Send(new AlertMessage
                {
                    Message = "Failed to create data source."
                });
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
                string path = Path.Combine(Location, Name, Path.ChangeExtension(Name, Project.FileExtension));
                if (File.Exists(path))
                {
                    ConfirmMessage msg = new ConfirmMessage
                    {
                        Verb = "Add",
                        Message = "Data source already exists. Add it to your list of data sources?"
                    };
                    msg.Confirmed += (sender, e) =>
                    {
                        AddDataSource(path);
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
                            driver = AccessDriver.Create(Path.ChangeExtension(path, ".mdb"));
                            break;
                        case DataProvider.SqlServer:
                            driver = SqlServerOptions.CreateDriver();
                            break;
                        default:
                            throw new InvalidEnumValueException(Provider);
                    }
                    if (driver.DatabaseExists())
                    {
                        ConfirmMessage msg = new ConfirmMessage
                        {
                            Verb = "Add",
                            Message = "Database already exists. Add a data source using this database?"
                        };
                        msg.Confirmed += (sender, e) =>
                        {
                            Create(driver, false);
                            Active = false;
                        };
                        Messenger.Default.Send(msg);
                    }
                    else
                    {
                        BlockMessage msg = new BlockMessage
                        {
                            Message = "Creating data source \u2026"
                        };
                        msg.Executing += (sender, e) =>
                        {
                            Create(driver, true);
                            Active = false;
                        };
                        msg.Executed += (sender, e) =>
                        {
                            Messenger.Default.Send(new ToastMessage
                            {
                                Message = "Data source has been created."
                            });
                        };
                        Messenger.Default.Send(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to create data source", ex);
                Messenger.Default.Send(new AlertMessage
                {
                    Message = "Failed to create data source."
                });
            }
        }

        public void Cancel()
        {
            Active = false;
        }
    }
}
