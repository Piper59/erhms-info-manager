using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public class CreateMapViewModel : CreateAssetViewModel
    {
        protected override Module Module => Module.Mapping;
        protected override string FileExtension => FileExtensions.Map;
        protected override string FileFilter => ResXResources.FileDialog_Filter_Maps;

        public CreateMapViewModel(Project project)
            : base(project) { }

        protected override Asset CreateCore()
        {
            return new Map(View);
        }
    }
}
