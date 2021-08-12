using Epi;
using ERHMS.Desktop.ViewModels.Collections;
using System.Threading.Tasks;

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

        private ProjectViewModel(Project project)
        {
            Project = project;
        }

        private async Task InitializeAsync()
        {
            Views = await ViewCollectionViewModel.CreateAsync(Project);
            Canvases = await CanvasCollectionViewModel.CreateAsync(Project);
            Pgms = await PgmCollectionViewModel.CreateAsync(Project);
            Maps = await MapCollectionViewModel.CreateAsync(Project);
        }
    }
}
