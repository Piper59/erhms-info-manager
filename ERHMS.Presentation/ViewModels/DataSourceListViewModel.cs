using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    public class DataSourceListViewModel : ListViewModelBase<ProjectInfo>
    {
        public ICollection<DataProvider> Providers { get; private set; }

        private bool creating;
        public bool Creating
        {
            get { return creating; }
            set { Set(() => Creating, ref creating, value); }
        }

        public DataSourceViewModel DataSource { get; private set; }

        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand AddNewCommand { get; private set; }
        public RelayCommand AddExistingCommand { get; private set; }
        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public DataSourceListViewModel()
        {
            Title = "Data Sources";
            Refresh();
            Selecting += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            };
            Providers = new DataProvider[]
            {
                DataProvider.Access,
                DataProvider.SqlServer
            };
            DataSource = new DataSourceViewModel();
            DataSource.SqlServer.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IntegratedSecurity")
                {
                    if (DataSource.SqlServer.IntegratedSecurity)
                    {
                        DataSource.SqlServer.UserId = null;
                        DataSource.SqlServer.Password = null;
                    }
                }
            };
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            AddNewCommand = new RelayCommand(AddNew);
            AddExistingCommand = new RelayCommand(AddExisting);
            BrowseCommand = new RelayCommand(Browse);
            CreateCommand = new RelayCommand(Create);
            CancelCommand = new RelayCommand(Cancel);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshListMessage<ProjectInfo>>(this, OnRefreshDataSourceList);
        }

        protected override ICollectionView GetItems()
        {
            ICollection<ProjectInfo> items = new List<ProjectInfo>();
            foreach (string path in Settings.Default.DataSources)
            {
                ProjectInfo projectInfo;
                FileInfo file = new FileInfo(path);
                if (ProjectInfo.TryRead(file, out projectInfo))
                {
                    items.Add(projectInfo);
                }
                else
                {
                    Remove(file);
                }
            }
            return CollectionViewSource.GetDefaultView(items
                .OrderBy(projectInfo => projectInfo.Name)
                .ThenBy(projectInfo => projectInfo.Description));
        }

        protected override IEnumerable<string> GetFilteredValues(ProjectInfo item)
        {
            yield return item.Name;
            yield return item.Description;
        }

        public void Open()
        {
            Locator.Main.OpenDataSource(SelectedItem.File);
        }

        private void Add(FileInfo file)
        {
            Settings.Default.DataSources.Add(file.FullName);
            Settings.Default.Save();
        }

        public void AddNew()
        {
            DataSource.Reset();
            Creating = true;
        }

        public void AddExisting()
        {
            Configuration configuration = Configuration.GetNewInstance();
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Add Existing Data Source";
                dialog.InitialDirectory = configuration.Directories.Project;
                dialog.Filter = string.Format("Data Sources (*{0})|*{0}", Project.FileExtension);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Add(new FileInfo(dialog.FileName));
                    Messenger.Default.Send(new RefreshListMessage<ProjectInfo>());
                }
            }
        }

        public void Browse()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DataSource.Location = new DirectoryInfo(dialog.SelectedPath);
                }
            }
        }

        private void Create(FileInfo file, IDataDriver driver, bool initialize)
        {
            if (initialize)
            {
                driver.CreateDatabase();
            }
            Project project = Project.Create(
                DataSource.Name,
                DataSource.Description,
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

        public void Create()
        {
            // TODO: Validate fields
            // TODO: Handle errors
            FileInfo file = DataSource.Location.GetFile(Path.Combine(
                DataSource.Name,
                Path.ChangeExtension(DataSource.Name, Project.FileExtension)));
            if (file.Exists)
            {
                ConfirmMessage msg = new ConfirmMessage(
                    "Add?",
                    "Data source already exists. Add it to your list of data sources?",
                    "Add",
                    "Don't Add");
                msg.Confirmed += (sender, e) =>
                {
                    Add(file);
                    Messenger.Default.Send(new RefreshListMessage<ProjectInfo>());
                    Creating = false;
                };
                Messenger.Default.Send(msg);
            }
            else
            {
                IDataDriver driver;
                switch (DataSource.Provider)
                {
                    case DataProvider.Access:
                        driver = AccessDriver.Create(Path.ChangeExtension(file.FullName, ".mdb"));
                        break;
                    case DataProvider.SqlServer:
                        driver = SqlServerDriver.Create(
                            DataSource.SqlServer.DataSource,
                            DataSource.SqlServer.InitialCatalog,
                            DataSource.SqlServer.UserId,
                            DataSource.SqlServer.Password);
                        break;
                    default:
                        throw new NotSupportedException();
                }
                if (driver.DatabaseExists())
                {
                    ConfirmMessage msg = new ConfirmMessage(
                        "Add?",
                        "Database already exists. Add a data source using this database?",
                        "Add",
                        "Don't Add");
                    msg.Confirmed += (sender, e) =>
                    {
                        Create(file, driver, false);
                        Messenger.Default.Send(new RefreshListMessage<ProjectInfo>());
                        Creating = false;
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
                        Creating = false;
                    };
                    Messenger.Default.Send(msg);
                }
            }
        }

        public void Cancel()
        {
            Creating = false;
        }

        private void Remove(FileInfo file)
        {
            Settings.Default.DataSources.RemoveWhere(dataSource => dataSource.Equals(file.FullName, StringComparison.OrdinalIgnoreCase));
            Settings.Default.Save();
        }

        public void Remove()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Remove?",
                "Are you sure you want to remove this data source?",
                "Remove",
                "Don't Remove");
            msg.Confirmed += (sender, e) =>
            {
                Remove(SelectedItem.File);
                Messenger.Default.Send(new RefreshListMessage<ProjectInfo>());
            };
            Messenger.Default.Send(msg);
        }

        private void OnRefreshDataSourceList(RefreshListMessage<ProjectInfo> msg)
        {
            Refresh();
        }
    }
}
