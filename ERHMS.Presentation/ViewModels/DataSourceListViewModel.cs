using Epi;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    [ContextSafe]
    public class DataSourceListViewModel : DocumentViewModel
    {
        public class DataSourceListChildViewModel : ListViewModel<ProjectInfo>
        {
            public DataSourceListChildViewModel()
            {
                Refresh();
            }

            protected override IEnumerable<ProjectInfo> GetItems()
            {
                ICollection<ProjectInfo> projectInfos = new List<ProjectInfo>();
                foreach (string path in Settings.Default.DataSourcePaths)
                {
                    ProjectInfo projectInfo;
                    if (ProjectInfo.TryRead(path, out projectInfo))
                    {
                        projectInfos.Add(projectInfo);
                    }
                }
                return projectInfos.OrderBy(projectInfo => projectInfo.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(ProjectInfo item)
            {
                yield return item.Name;
                yield return item.Description;
            }
        }

        public DataSourceListChildViewModel DataSources { get; private set; }

        public ICommand OpenCommand { get; private set; }
        public ICommand AddNewCommand { get; private set; }
        public ICommand AddExistingCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }

        public DataSourceListViewModel()
        {
            Title = "Data Sources";
            DataSources = new DataSourceListChildViewModel();
            OpenCommand = new AsyncCommand(OpenAsync, DataSources.HasSelectedItem);
            AddNewCommand = new AsyncCommand(AddNewAsync);
            AddExistingCommand = new Command(AddExisting);
            RemoveCommand = new AsyncCommand(RemoveAsync, DataSources.HasSelectedItem);
        }

        private void Add(string path)
        {
            Settings.Default.DataSourcePaths.Add(path);
            Settings.Default.Save();
            ServiceLocator.Data.Refresh(typeof(ProjectInfo));
        }

        private void Remove(string path)
        {
            Settings.Default.DataSourcePaths.Remove(path);
            Settings.Default.Save();
            ServiceLocator.Data.Refresh(typeof(ProjectInfo));
        }

        public async Task OpenAsync()
        {
            ProjectInfo projectInfo;
            if (ProjectInfo.TryRead(DataSources.SelectedItem.FilePath, out projectInfo))
            {
                await ServiceLocator.Document.SetContextAsync(projectInfo);
            }
            else
            {
                if (await ServiceLocator.Dialog.ConfirmAsync(Resources.DataSourceConfirmRemoveUnopenable, "Remove"))
                {
                    Remove(DataSources.SelectedItem.FilePath);
                }
            }
        }

        public async Task AddNewAsync()
        {
            DataSourceViewModel model = new DataSourceViewModel();
            model.Added += (sender, e) =>
            {
                Add(model.FilePath);
            };
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        public void AddExisting()
        {
            string path = ServiceLocator.Dialog.OpenFile(
                "Add Existing Data Source",
                Configuration.GetNewInstance().Directories.Project,
                FileDialogExtensions.Filters.DataSources);
            if (path != null)
            {
                Add(path);
            }
        }

        public async Task RemoveAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.DataSourceConfirmRemove, "Remove"))
            {
                Remove(DataSources.SelectedItem.FilePath);
            }
        }
    }
}
