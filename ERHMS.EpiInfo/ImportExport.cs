using Epi.Enter.Forms;
using Epi.Windows.ImportExport.Dialogs;
using ERHMS.Utility;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo
{
    public static class ImportExport
    {
        public static bool ImportFromView(View target)
        {
            Log.Logger.DebugFormat("Importing from view: {0}", target.Name);
            using (ImportDataForm form = new ImportDataForm(target))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        public static bool ImportFromPackage(View target)
        {
            Log.Logger.DebugFormat("Importing from package: {0}", target.Name);
            using (ImportEncryptedDataPackageDialog dialog = new ImportEncryptedDataPackageDialog(target))
            {
                dialog.StartPosition = FormStartPosition.CenterParent;
                return dialog.ShowDialog() == DialogResult.OK;
            }
        }

        public static bool ImportFromMobile(View target)
        {
            Log.Logger.DebugFormat("Importing from mobile: {0}", target.Name);
            using (ImportPhoneDataForm form = new ImportPhoneDataForm(target))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        public static bool ExportToPackage(View source)
        {
            Log.Logger.DebugFormat("Exporting to package: {0}", source.Name);
            using (PackageForTransportDialog dialog = new PackageForTransportDialog(source.Project.FilePath, source))
            {
                dialog.StartPosition = FormStartPosition.CenterParent;
                return dialog.ShowDialog() == DialogResult.OK;
            }
        }
    }
}
