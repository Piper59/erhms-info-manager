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
            Log.Current.DebugFormat("Importing from view: {0}", target.Name);
            using (ImportDataForm form = new ImportDataForm(target))
            {
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        public static bool ImportFromPackage(View target)
        {
            Log.Current.DebugFormat("Importing from package: {0}", target.Name);
            using (ImportEncryptedDataPackageDialog dialog = new ImportEncryptedDataPackageDialog(target))
            {
                return dialog.ShowDialog() == DialogResult.OK;
            }
        }

        public static bool ImportFromMobile(View target)
        {
            Log.Current.DebugFormat("Importing from mobile: {0}", target.Name);
            using (ImportPhoneDataForm form = new ImportPhoneDataForm(target))
            {
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        public static bool ExportToPackage(View source)
        {
            Log.Current.DebugFormat("Exporting to package: {0}", source.Name);
            using (PackageForTransportDialog dialog = new PackageForTransportDialog(source.Project.FilePath, source))
            {
                return dialog.ShowDialog() == DialogResult.OK;
            }
        }
    }
}
