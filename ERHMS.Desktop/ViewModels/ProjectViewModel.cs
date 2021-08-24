using Epi;
using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo;
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
        public AssetCollectionViewModel Canvases { get; private set; }
        public AssetCollectionViewModel Pgms { get; private set; }
        public AssetCollectionViewModel Maps { get; private set; }

        private ProjectViewModel(Project project)
        {
            Project = project;
        }

        private async Task InitializeAsync()
        {
            Views = await ViewCollectionViewModel.CreateAsync(Project);
            Canvases = await AssetCollectionViewModel.CreateAsync(Module.AnalysisDashboard, Project);
            Pgms = await AssetCollectionViewModel.CreateAsync(Module.Analysis, Project);
            Maps = await AssetCollectionViewModel.CreateAsync(Module.Mapping, Project);
        }
    }
}
