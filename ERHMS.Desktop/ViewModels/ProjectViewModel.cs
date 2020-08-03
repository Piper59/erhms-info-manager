using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project = ERHMS.EpiInfo.Projects.Project;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ObservableObject
    {
        public class ViewChildViewModel : ObservableObject
        {
            public View View { get; }
            public string Name { get; }
            public int RecordCount { get; }

            public ViewChildViewModel(View view, ISet<string> tableNames)
            {
                View = view;
                Name = view.Name;
                RecordCount = tableNames.Contains(view.TableName) ? view.GetRecordCount() : 0;
            }
        }

        public Project Project { get; }

        private ICollection<ViewChildViewModel> views;
        public ICollection<ViewChildViewModel> Views
        {
            get { return views; }
            set { SetProperty(ref views, value); }
        }

        public Command RefreshCommand { get; }

        public ProjectViewModel(Project project)
        {
            Project = project;
            RefreshInternal();
            RefreshCommand = new SimpleAsyncCommand(RefreshAsync);
        }

        private void RefreshInternal()
        {
            ISet<string> tableNames = Project.GetTableNameSet();
            Views = Project.Views.Cast<View>()
                .OrderBy(view => view.Name, StringComparer.OrdinalIgnoreCase)
                .Select(view => new ViewChildViewModel(view, tableNames))
                .ToList();
        }

        private async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingProjectTaskName);
            await progress.RunAsync(() =>
            {
                Project.LoadViews();
                RefreshInternal();
            });
        }
    }
}
