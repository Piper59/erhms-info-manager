using Epi.Enter.Forms;
using Epi.Windows.ImportExport.Dialogs;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo.ImportExport
{
    public class ImportExport
    {
        public static bool ImportFromView(View target)
        {
            using (ImportDataForm form = new ImportDataForm(target))
            {
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        public static bool ImportFromPackage(View target)
        {
            using (ImportEncryptedDataPackageDialog dialog = new ImportEncryptedDataPackageDialog(target))
            {
                return dialog.ShowDialog() == DialogResult.OK;
            }
        }

        public static bool ExportToPackage(View source)
        {
            using (PackageForTransportDialog dialog = new PackageForTransportDialog(source.Project.FilePath, source))
            {
                return dialog.ShowDialog() == DialogResult.OK;
            }
        }
    }
}
