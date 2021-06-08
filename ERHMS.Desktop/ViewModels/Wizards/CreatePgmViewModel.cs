using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public class CreatePgmViewModel : CreateAssetViewModel
    {
        protected override Module Module => Module.Analysis;
        protected override string FileExtension => FileExtensions.Pgm;
        protected override string FileFilter => Strings.FileDialog_Filter_Pgms;

        public CreatePgmViewModel(Project project)
            : base(project) { }

        protected override Asset CreateCore()
        {
            return new Pgm(View);
        }
    }
}
