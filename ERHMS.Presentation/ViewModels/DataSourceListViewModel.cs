using Epi;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    public class DataSourceListViewModel : ListViewModel<ProjectInfo>
    {
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand AddNewCommand { get; private set; }
        public RelayCommand AddExistingCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }

        public DataSourceListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Data Sources";
            Refresh();
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            AddNewCommand = new RelayCommand(AddNew);
            AddExistingCommand = new RelayCommand(AddExisting);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            SelectionChanged += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<ProjectInfo> GetItems()
        {
            ICollection<ProjectInfo> dataSources = new List<ProjectInfo>();
            foreach (string path in Settings.Default.DataSourcePaths.ToList())
            {
                ProjectInfo dataSource;
                if (ProjectInfo.TryRead(path, out dataSource))
                {
                    dataSources.Add(dataSource);
                }
                else
                {
                    DataSourceViewModel.Remove(path);
                }
            }
            return dataSources.OrderBy(dataSource => dataSource.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(ProjectInfo item)
        {
            yield return item.Name;
            yield return item.Description;
        }

        public void Open()
        {
            Documents.OpenDataSource(SelectedItem.FilePath);
        }

        public void AddNew()
        {
            Dialogs.ShowAsync(new DataSourceViewModel(Services));
        }

        public void AddExisting()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Add Existing Data Source";
                dialog.InitialDirectory = Configuration.GetNewInstance().Directories.Project;
                dialog.Filter = FileDialogExtensions.GetFilter("Data Sources", Project.FileExtension);
                if (dialog.ShowDialog(Dialogs.Win32Window) == DialogResult.OK)
                {
                    DataSourceViewModel.Add(dialog.FileName);
                    MessengerInstance.Send(new RefreshMessage(typeof(ProjectInfo)));
                }
            }
        }

        public void Remove()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Remove",
                Message = "Remove the selected data source?"
            };
            msg.Confirmed += (sender, e) =>
            {
                DataSourceViewModel.Remove(SelectedItem.FilePath);
                MessengerInstance.Send(new RefreshMessage(typeof(ProjectInfo)));
            };
            MessengerInstance.Send(msg);
        }
    }
}
