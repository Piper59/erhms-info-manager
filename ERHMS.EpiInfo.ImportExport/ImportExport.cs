using Epi;
using Epi.Enter.Forms;
using Epi.Windows.ImportExport.Dialogs;

namespace ERHMS.EpiInfo.ImportExport
{
    public class ImportExport
    {
        public static void ImportFromView(View target)
        {
            using (ImportDataForm form = new ImportDataForm(target))
            {
                form.ShowDialog();
            }
        }

        public static void ImportFromPackage(View target)
        {
            using (ImportEncryptedDataPackageDialog dialog = new ImportEncryptedDataPackageDialog(target))
            {
                dialog.ShowDialog();
            }
        }

        public static void ExportToPackage(View source)
        {
            using (PackageForTransportDialog dialog = new PackageForTransportDialog(source.Project.FilePath, source))
            {
                dialog.ShowDialog();
            }
        }

        public static void ImportFromWeb(View target)
        {
            // TODO
        }
    }
}
