using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo.Projects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using View = Epi.View;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ObservableObject
    {
        public class ViewChildViewModel : ObservableObject
        {
            public View View { get; }
            public int RecordCount { get; set; }

            public ViewChildViewModel(View view)
            {
                View = view;
            }

            public override int GetHashCode()
            {
                return View.Id;
            }

            public override bool Equals(object obj)
            {
                return obj is ViewChildViewModel viewChild && viewChild.View.Id == View.Id;
            }
        }

        public Project Project { get; }

        private ICollection<ViewChildViewModel> viewChildren;
        public ICollection<ViewChildViewModel> ViewChildren
        {
            get { return viewChildren; }
            set { SetProperty(ref viewChildren, value); }
        }

        private ViewChildViewModel selectedViewChild;
        public ViewChildViewModel SelectedViewChild
        {
            get
            {
                return selectedViewChild;
            }
            set
            {
                SetProperty(ref selectedViewChild, value);
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
            ISet<string> tableNames = new HashSet<string>(Project.Database.GetTableNames(), StringComparer.OrdinalIgnoreCase);
            viewChildren = new List<ViewChildViewModel>();
            foreach (View view in Project.Views)
            {
                viewChildren.Add(new ViewChildViewModel(view)
                {
                    RecordCount = tableNames.Contains(view.TableName) ? view.GetRecordCount() : 0
                });
            }
        }

        private async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingProjectTaskName);
            await progress.RunAsync(() =>
            {
                Project.LoadViews();
                RefreshInternal();
            });
            OnPropertyChanged(nameof(ViewChildren));
        }
    }
}
