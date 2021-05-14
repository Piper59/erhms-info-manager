using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using System.Threading.Tasks;

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
        protected override string Extension => ".cvs7";
        protected override string RefreshingLead => ResXResources.Lead_RefreshingCanvases;

        private CanvasCollectionViewModel(Project project)
            : base(project) { }
    }
}
