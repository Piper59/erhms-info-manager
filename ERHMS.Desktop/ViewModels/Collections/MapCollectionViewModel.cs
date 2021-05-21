using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using System.Threading.Tasks;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class MapCollectionViewModel : FileInfoCollectionViewModel
    {
        public static async Task<MapCollectionViewModel> CreateAsync(Project project)
        {
            MapCollectionViewModel result = new MapCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.Mapping;
        protected override string Extension => FileExtensions.Map;
        protected override string RefreshingLead => ResXResources.Lead_RefreshingMaps;

        private MapCollectionViewModel(Project project)
            : base(project) { }
    }
}
