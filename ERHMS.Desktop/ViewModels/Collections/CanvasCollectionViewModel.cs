using Epi;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using System.Threading.Tasks;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class CanvasCollectionViewModel : AssetCollectionViewModel
    {
        public static async Task<CanvasCollectionViewModel> CreateAsync(Project project)
        {
            CanvasCollectionViewModel result = new CanvasCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.AnalysisDashboard;
        protected override string FileExtension => FileExtensions.Canvas;

        private CanvasCollectionViewModel(Project project)
            : base(project) { }

        protected override async Task<CreateAssetViewModel> GetCreateWizardAsync()
        {
            return await CreateCanvasViewModel.CreateAsync(Project);
        }
    }
}
