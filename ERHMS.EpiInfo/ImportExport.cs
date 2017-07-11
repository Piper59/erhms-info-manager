using Epi.Enter.Forms;
using Epi.Windows.ImportExport.Dialogs;
using ERHMS.Utility;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo
{
    public static class ImportExport
    {
        private static bool ShowDialog(Form form)
        {
            try
            {
                form.StartPosition = FormStartPosition.CenterParent;
                return form.ShowDialog() == DialogResult.OK;
            }
            finally
            {
                form.Dispose();
            }
        }

        public static bool ImportFromView(View target)
        {
            Log.Logger.DebugFormat("Importing from view: {0}", target.Name);
            return ShowDialog(new ImportDataForm(target));
        }

        public static bool ImportFromPackage(View target)
        {
            Log.Logger.DebugFormat("Importing from package: {0}", target.Name);
            return ShowDialog(new ImportEncryptedDataPackageDialog(target));
        }

        public static bool ImportFromMobile(View target)
        {
            Log.Logger.DebugFormat("Importing from mobile: {0}", target.Name);
            return ShowDialog(new ImportPhoneDataForm(target));
        }

        public static bool ExportToPackage(View source)
        {
            Log.Logger.DebugFormat("Exporting to package: {0}", source.Name);
            return ShowDialog(new PackageForTransportDialog(source.Project.FilePath, source));
        }
    }
}
