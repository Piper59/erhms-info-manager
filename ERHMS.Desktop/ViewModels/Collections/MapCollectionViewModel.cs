using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
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

        private MapCollectionViewModel(Project project)
            : base(project) { }

        protected override void CreateCore(View view, string path)
        {
            Map map = new Map(view);
            map.Save(path);
        }
    }
}
