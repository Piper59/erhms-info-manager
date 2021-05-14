using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class ProgramCollectionViewModel : FileInfoCollectionViewModel
    {
        public static async Task<ProgramCollectionViewModel> CreateAsync(Project project)
        {
            ProgramCollectionViewModel result = new ProgramCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.Analysis;
        protected override string Extension => ".pgm7";
        protected override string RefreshingLead => ResXResources.Lead_RefreshingPrograms;

        private ProgramCollectionViewModel(Project project)
            : base(project) { }
    }
}
