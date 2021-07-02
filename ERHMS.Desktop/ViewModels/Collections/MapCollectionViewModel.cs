using Epi;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.EpiInfo;
using System.Threading.Tasks;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class MapCollectionViewModel : AssetCollectionViewModel
    {
        public static async Task<MapCollectionViewModel> CreateAsync(Project project)
        {
            MapCollectionViewModel result = new MapCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.Mapping;
        protected override string FileExtension => FileExtensions.Map;

        private MapCollectionViewModel(Project project)
            : base(project) { }

        protected override async Task<CreateAssetViewModel> GetCreateWizardAsync()
        {
            return await CreateMapViewModel.CreateAsync(Project);
        }
    }
}
