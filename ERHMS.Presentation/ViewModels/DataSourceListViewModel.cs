using Epi;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Presentation.ViewModels
{
    [ContextSafe]
    public class DataSourceListViewModel : DocumentViewModel
    {
        public class DataSourceListChildViewModel : ListViewModel<ProjectInfo>
        {
            public DataSourceListChildViewModel(IServiceManager services)
                : base(services)
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

        public DataSourceListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Data Sources";
            DataSources = new DataSourceListChildViewModel(services);
            OpenCommand = new AsyncCommand(OpenAsync, DataSources.HasSelectedItem);
            AddNewCommand = new AsyncCommand(AddNewAsync);
            AddExistingCommand = new Command(AddExisting);
            RemoveCommand = new AsyncCommand(RemoveAsync, DataSources.HasSelectedItem);
        }

        private void Add(string path)
        {
            Settings.Default.DataSourcePaths.Add(path);
            Settings.Default.Save();
            Services.Data.Refresh(typeof(ProjectInfo));
        }

        private void Remove(string path)
        {
            Settings.Default.DataSourcePaths.Remove(path);
            Settings.Default.Save();
            Services.Data.Refresh(typeof(ProjectInfo));
        }

        public async Task OpenAsync()
        {
            ProjectInfo projectInfo;
            if (ProjectInfo.TryRead(DataSources.SelectedItem.FilePath, out projectInfo))
            {
                await Services.Document.SetContextAsync(projectInfo);
            }
            else
            {
                if (await Services.Dialog.ConfirmAsync("Data source could not be opened. Remove it from the list of data sources?", "Remove"))
                {
                    Remove(DataSources.SelectedItem.FilePath);
                }
            }
        }

        public async Task AddNewAsync()
        {
            using (DataSourceViewModel model = new DataSourceViewModel(Services))
            {
                model.Added += (sender, e) =>
                {
                    Add(model.FilePath);
                };
                await Services.Dialog.ShowAsync(model);
            }
        }

        public void AddExisting()
        {
            string path = Services.Dialog.OpenFile(
                "Add Existing Data Source",
                Configuration.GetNewInstance().Directories.Project,
                FileDialogExtensions.GetFilter("Data Sources", Project.FileExtension));
            if (path != null)
            {
                Add(path);
            }
        }

        public async Task RemoveAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Remove the selected data source?", "Remove"))
            {
                Remove(DataSources.SelectedItem.FilePath);
            }
        }

        public override void Dispose()
        {
            DataSources.Dispose();
            base.Dispose();
        }
    }
}
