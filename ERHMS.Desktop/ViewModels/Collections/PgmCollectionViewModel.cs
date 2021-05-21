using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analysis;
using System.Threading.Tasks;
using FileExtensions = ERHMS.EpiInfo.FileExtensions;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class PgmCollectionViewModel : FileInfoCollectionViewModel
    {
        public static async Task<PgmCollectionViewModel> CreateAsync(Project project)
        {
            PgmCollectionViewModel result = new PgmCollectionViewModel(project);
            await result.InitializeAsync();
            return result;
        }

        protected override Module Module => Module.Analysis;
        protected override string Extension => FileExtensions.Pgm;

        private PgmCollectionViewModel(Project project)
            : base(project) { }

        protected override void CreateCore(View view, string path)
        {
            Pgm pgm = new Pgm(view);
            pgm.Save(path);
        }
    }
}
