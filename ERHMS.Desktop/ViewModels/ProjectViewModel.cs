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
        public ICommand OpenProjectCommand { get; }

        public ProjectViewModel(Project project)
        {
            Project = project;
            Views = new ViewsChildViewModel(project);
            RefreshCommand = new AsyncCommand(RefreshAsync);
            OpenProjectCommand = new AsyncCommand(OpenProjectAsync);
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
            IProgressService progress = ServiceProvider.Resolve<IProgressService>();
            progress.Title = ResX.LoadingProjectTitle;
            await progress.RunAsync(RefreshData);
            RefreshView();
        }

        public async Task OpenProjectAsync()
        {
            await Task.CompletedTask;
            throw new System.NotImplementedException();
        }
    }
}
