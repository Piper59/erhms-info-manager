using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using View = Epi.View;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ObservableObject
    {
        public class ViewChildViewModel : ObservableObject
        {
            public View View { get; }
            public int ViewId => View.Id;
            public string Name => View.Name;
            public int RecordCount { get; }

            public ViewChildViewModel(View view, ISet<string> tableNames)
            {
                View = view;
                RecordCount = tableNames.Contains(view.TableName) ? view.GetRecordCount() : 0;
            }

            public override int GetHashCode()
            {
                return ViewId;
            }

            public override bool Equals(object obj)
            {
                return obj is ViewChildViewModel view && view.ViewId == ViewId;
            }
        }

        public Project Project { get; }

        private ICollection<ViewChildViewModel> views;
        public ICollection<ViewChildViewModel> Views
        {
            get { return views; }
            set { SetProperty(ref views, value); }
        }

        private ViewChildViewModel selectedView;
        public ViewChildViewModel SelectedView
        {
            get
            {
                return selectedView;
            }
            set
            {
                SetProperty(ref selectedView, value);
                if (value != null)
                {
                    AppCommands.OpenViewCommand.Execute(value.View);
                }
            }
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
            views = Project.Views.Cast<View>()
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
            OnPropertyChanged(nameof(Views));
        }
    }
}
