using Epi;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class MapCollectionViewModel : AssetCollectionViewModel
    {
        protected override Module Module => Module.Mapping;
        protected override string FileExtension => FileExtensions.Map;

        public MapCollectionViewModel(Project project)
            : base(project) { }

        protected override CreateAssetViewModel GetCreateWizard()
        {
            return new CreateMapViewModel(Project);
        }
    }
}
