using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using System.Threading.Tasks;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public class CreateCanvasViewModel : CreateAssetViewModel
    {
        public static async Task<CreateCanvasViewModel> CreateAsync(Project project)
        {
            CreateCanvasViewModel result = new CreateCanvasViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.AnalysisDashboard;
        protected override string FileExtension => FileExtensions.Canvas;
        protected override string FileFilter => ResXResources.FileDialog_Filter_Canvases;

        private CreateCanvasViewModel(Project project)
            : base(project) { }

        protected override Asset GetAsset()
        {
            return new Canvas(View);
        }
    }
}
