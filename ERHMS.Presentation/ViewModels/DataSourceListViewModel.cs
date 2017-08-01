using Epi;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    public class DataSourceListViewModel : ListViewModel<ProjectInfo>
    {
        private RelayCommand openCommand;
        public ICommand OpenCommand
        {
            get { return openCommand ?? (openCommand = new RelayCommand(Open, HasSelectedItem)); }
        }

        private RelayCommand addNewCommand;
        public ICommand AddNewCommand
        {
            get { return addNewCommand ?? (addNewCommand = new RelayCommand(AddNew)); }
        }

        private RelayCommand addExistingCommand;
        public ICommand AddExistingCommand
        {
            get { return addExistingCommand ?? (addExistingCommand = new RelayCommand(AddExisting)); }
        }

        private RelayCommand removeCommand;
        public ICommand RemoveCommand
        {
            get { return removeCommand ?? (removeCommand = new RelayCommand(Remove, HasSelectedItem)); }
        }

        public DataSourceListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Data Sources";
            SelectionChanged += (sender, e) =>
            {
                openCommand.RaiseCanExecuteChanged();
                removeCommand.RaiseCanExecuteChanged();
            };
            Refresh();
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
