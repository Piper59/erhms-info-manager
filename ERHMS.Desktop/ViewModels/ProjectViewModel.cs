using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo.Projects;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public partial class ProjectViewModel : ObservableObject
    {
        public Project Project { get; }
        public ViewsChildViewModel Views { get; }

        public ICommand RefreshCommand { get; }

        public ProjectViewModel(Project project)
        {
            Project = project;
            Views = new ViewsChildViewModel(project);
            RefreshCommand = new AsyncCommand(RefreshAsync, Command.Always, ErrorBehavior.Raise);
        }

        private void RefreshData()
        {
            Views.RefreshData();
        }

        private void RefreshView()
        {
            Views.RefreshView();
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingProjectTaskName, false);
            await progress.RunAsync(RefreshData);
            RefreshView();
        }
    }
}
