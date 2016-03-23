using Epi;
using Epi.Enter.Forms;
using Epi.Windows.ImportExport.Dialogs;

namespace ERHMS.EpiInfo.ImportExport
{
    public class ImportExport
    {
        public static void ImportFromView(View view)
        {
            using (ImportDataForm form = new ImportDataForm(view))
            {
                form.ShowDialog();
            }
        }

        public static void ImportFromPackage(View view)
        {
            using (ImportEncryptedDataPackageDialog dialog = new ImportEncryptedDataPackageDialog(view))
            {
                dialog.ShowDialog();
            }
        }

        public static void ImportFromWeb(View view)
        {
            // TODO
        }

        public static void ExportToPackage(View view)
        {
            using (PackageForTransportDialog dialog = new PackageForTransportDialog(view.Project.FilePath, view))
            {
                dialog.ShowDialog();
            }
        }
    }
}
