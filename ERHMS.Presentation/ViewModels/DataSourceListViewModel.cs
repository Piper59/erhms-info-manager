using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
        public DataSourceViewModel DataSource { get; private set; }

        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand AddNewCommand { get; private set; }
        public RelayCommand AddExistingCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public DataSourceListViewModel()
        {
            Title = "Data Sources";
            Providers = new DataProvider[]
            {
                DataProvider.Access,
                DataProvider.SqlServer
            };
            Refresh();
            Selecting += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            };
            DataSource = new DataSourceViewModel();
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            AddNewCommand = new RelayCommand(AddNew);
            AddExistingCommand = new RelayCommand(AddExisting);
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
            return CollectionViewSource.GetDefaultView(items.OrderBy(projectInfo => projectInfo.Name)
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

        public void AddNew()
        {
            DataSource.Reset();
            DataSource.Active = true;
        }

        public void AddExisting()
        {
            Configuration configuration = Configuration.GetNewInstance();
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Add Existing Data Source";
                dialog.InitialDirectory = configuration.Directories.Project;
                dialog.Filter = FileDialogExtensions.GetFilter("Data Sources", Project.FileExtension);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DataSourceViewModel.Add(new FileInfo(dialog.FileName));
                    Messenger.Default.Send(new RefreshListMessage<ProjectInfo>());
                }
            }
        }

        private void Remove(FileInfo file)
        {
            Settings.Default.DataSources.RemoveWhere(dataSource => dataSource.EqualsIgnoreCase(file.FullName));
            Settings.Default.Save();
        }

        public void Remove()
        {
            ConfirmMessage msg = new ConfirmMessage("Remove", "Remove the selected data source?");
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
