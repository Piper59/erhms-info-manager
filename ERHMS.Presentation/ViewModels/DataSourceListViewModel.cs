using Epi;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    public class DataSourceListViewModel : ListViewModelBase<ProjectInfo>
    {
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand AddNewCommand { get; private set; }
        public RelayCommand AddExistingCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public DataSourceListViewModel()
        {
            Title = "Data Sources";
            Refresh();
            OpenCommand = new RelayCommand(Open, HasOneSelectedItem);
            AddNewCommand = new RelayCommand(AddNew);
            AddExistingCommand = new RelayCommand(AddExisting);
            RemoveCommand = new RelayCommand(Remove, HasOneSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            };
            Messenger.Default.Register<RefreshMessage<ProjectInfo>>(this, msg => Refresh());
        }

        protected override IEnumerable<ProjectInfo> GetItems()
        {
            ICollection<ProjectInfo> items = new List<ProjectInfo>();
            foreach (string path in Settings.Default.DataSourcePaths.ToList())
            {
                ProjectInfo projectInfo;
                if (ProjectInfo.TryRead(path, out projectInfo))
                {
                    items.Add(projectInfo);
                }
                else
                {
                    DataSourceViewModel.RemoveDataSource(path);
                }
            }
            return items.OrderBy(item => item.Name).ThenBy(item => item.Description);
        }

        protected override IEnumerable<string> GetFilteredValues(ProjectInfo item)
        {
            yield return item.Name;
            yield return item.Description;
        }

        public void Open()
        {
            Main.OpenDataSource(SelectedItem.Path);
        }

        public void AddNew()
        {
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = new DataSourceViewModel
                {
                    Active = true
                }
            });
        }

        public void AddExisting()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Add Existing Data Source";
                Configuration configuration = Configuration.GetNewInstance();
                dialog.InitialDirectory = configuration.Directories.Project;
                dialog.Filter = FileDialogExtensions.GetFilter("Data Sources", Project.FileExtension);
                if (dialog.ShowDialog(App.Current.MainWin32Window) == DialogResult.OK)
                {
                    DataSourceViewModel.AddDataSource(dialog.FileName);
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
                DataSourceViewModel.RemoveDataSource(SelectedItem.Path);
            };
            Messenger.Default.Send(msg);
        }
    }
}
