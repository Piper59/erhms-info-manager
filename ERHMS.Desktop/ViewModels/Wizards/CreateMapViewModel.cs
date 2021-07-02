using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using System.Threading.Tasks;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public class CreateMapViewModel : CreateAssetViewModel
    {
        public static async Task<CreateMapViewModel> CreateAsync(Project project)
        {
            CreateMapViewModel result = new CreateMapViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.Mapping;
        protected override string FileExtension => FileExtensions.Map;
        protected override string FileFilter => Strings.FileDialog_Filter_Maps;

        public CreateMapViewModel(Project project)
            : base(project) { }

        protected override Asset CreateCore()
        {
            return new Map(View);
        }
    }
}
