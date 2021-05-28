using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using System.Threading.Tasks;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public class CreatePgmViewModel : CreateAssetViewModel
    {
        public static async Task<CreatePgmViewModel> CreateAsync(Project project)
        {
            CreatePgmViewModel result = new CreatePgmViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.Analysis;
        protected override string FileExtension => FileExtensions.Pgm;
        protected override string FileFilter => ResXResources.FileDialog_Filter_Pgms;

        private CreatePgmViewModel(Project project)
            : base(project) { }

        protected override Asset GetAsset()
        {
            return new Pgm(View);
        }
    }
}
