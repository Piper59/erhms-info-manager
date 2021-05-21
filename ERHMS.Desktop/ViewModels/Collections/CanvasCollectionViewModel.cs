using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
using System.Threading.Tasks;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class CanvasCollectionViewModel : FileInfoCollectionViewModel
    {
        public static async Task<CanvasCollectionViewModel> CreateAsync(Project project)
        {
            CanvasCollectionViewModel result = new CanvasCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.AnalysisDashboard;
        protected override string Extension => FileExtensions.Canvas;

        private CanvasCollectionViewModel(Project project)
            : base(project) { }

        protected override void CreateCore(View view, string path)
        {
            Canvas canvas = new Canvas(view);
            canvas.Save(path);
        }
    }
}
