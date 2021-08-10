using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel
    {
        public static async Task<ProjectViewModel> CreateAsync(Project project)
        {
            ProjectViewModel result = new ProjectViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        public Project Project { get; }
        public ViewCollectionViewModel Views { get; private set; }
        public CanvasCollectionViewModel Canvases { get; private set; }
        public PgmCollectionViewModel Pgms { get; private set; }
        public MapCollectionViewModel Maps { get; private set; }

        public ICommand GoToHomeCommand { get; }
        public ICommand GoToHelpCommand { get; }
        public ICommand OpenLocationCommand { get; }

        private ProjectViewModel(Project project)
        {
            Project = project;
            GoToHomeCommand = new SyncCommand(GoToHome);
            OpenLocationCommand = new SyncCommand(OpenLocation);
        }

        private async Task InitializeAsync()
        {
            Views = await ViewCollectionViewModel.CreateAsync(Project);
            Canvases = await CanvasCollectionViewModel.CreateAsync(Project);
            Pgms = await PgmCollectionViewModel.CreateAsync(Project);
            Maps = await MapCollectionViewModel.CreateAsync(Project);
        }

        public void GoToHome()
        {
            MainViewModel.Instance.GoToHome();
        }

        public void OpenLocation()
        {
            Process.Start(Project.Location)?.Dispose();
        }
    }
}
