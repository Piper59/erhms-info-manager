using Epi;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class CanvasCollectionViewModel : AssetCollectionViewModel
    {
        protected override Module Module => Module.AnalysisDashboard;
        protected override string FileExtension => FileExtensions.Canvas;

        public CanvasCollectionViewModel(Project project)
            : base(project) { }

        protected override CreateAssetViewModel GetCreateWizard()
        {
            return new CreateCanvasViewModel(Project);
        }
    }
}
