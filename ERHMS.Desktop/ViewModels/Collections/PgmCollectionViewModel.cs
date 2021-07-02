using Epi;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using System.Threading.Tasks;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class PgmCollectionViewModel : AssetCollectionViewModel
    {
        public static async Task<PgmCollectionViewModel> CreateAsync(Project project)
        {
            PgmCollectionViewModel result = new PgmCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.Analysis;
        protected override string FileExtension => FileExtensions.Pgm;

        private PgmCollectionViewModel(Project project)
            : base(project) { }

        protected override async Task<CreateAssetViewModel> GetCreateWizardAsync()
        {
            return await CreatePgmViewModel.CreateAsync(Project);
        }
    }
}
