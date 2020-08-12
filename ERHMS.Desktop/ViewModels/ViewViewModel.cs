using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo.Projects;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public partial class ViewViewModel : ObservableObject
    {
        public Project Project { get; }
        public Epi.View View { get; }
        public RecordsChildViewModel Records { get; }

        public ICommand RefreshCommand { get; }
        public ICommand GoUpCommand { get; }

        public ViewViewModel(Project project, Epi.View view)
        {
            Project = project;
            View = view;
            Records = new RecordsChildViewModel(view);
            RefreshCommand = new AsyncCommand(RefreshAsync, Command.Always, ErrorBehavior.Raise);
            GoUpCommand = new AsyncCommand(GoUpAsync, Command.Always, ErrorBehavior.Raise);
        }

        private void RefreshData()
        {
            Records.RefreshData();
        }

        private void RefreshView()
        {
            Records.RefreshView();
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingViewTaskName, false);
            await progress.RunAsync(RefreshData);
            RefreshView();
        }

        public async Task GoUpAsync()
        {
            ProjectViewModel content = null;
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningProjectTaskName, false);
            await progress.RunAsync(() =>
            {
                Project.LoadViews();
                content = new ProjectViewModel(Project);
            });
            MainViewModel.Current.Content = content;
        }
    }
}
