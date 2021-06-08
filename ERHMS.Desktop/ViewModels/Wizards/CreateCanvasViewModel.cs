using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public class CreateCanvasViewModel : CreateAssetViewModel
    {
        protected override Module Module => Module.AnalysisDashboard;
        protected override string FileExtension => FileExtensions.Canvas;
        protected override string FileFilter => Strings.FileDialog_Filter_Canvases;

        public CreateCanvasViewModel(Project project)
            : base(project) { }

        protected override Asset CreateCore()
        {
            return new Canvas(View);
        }
    }
}
