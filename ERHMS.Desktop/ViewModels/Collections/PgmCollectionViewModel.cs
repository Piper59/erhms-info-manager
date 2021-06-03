using Epi;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class PgmCollectionViewModel : AssetCollectionViewModel
    {
        protected override Module Module => Module.Analysis;
        protected override string FileExtension => FileExtensions.Pgm;

        public PgmCollectionViewModel(Project project)
            : base(project) { }

        protected override CreateAssetViewModel GetCreateWizard()
        {
            return new CreatePgmViewModel(Project);
        }
    }
}
