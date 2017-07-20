using Epi.Enter.Forms;
using Epi.Windows.ImportExport.Dialogs;
using ERHMS.Utility;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo
{
    public static class ImportExport
    {
        private static bool ShowDialog(IWin32Window owner, Form form)
        {
            try
            {
                form.StartPosition = FormStartPosition.CenterParent;
                return form.ShowDialog(owner) == DialogResult.OK;
            }
            finally
            {
                form.Dispose();
            }
        }

        public static bool ImportFromView(IWin32Window owner, View target)
        {
            Log.Logger.DebugFormat("Importing from view: {0}", target.Name);
            return ShowDialog(owner, new ImportDataForm(target));
        }

        public static bool ImportFromPackage(IWin32Window owner, View target)
        {
            Log.Logger.DebugFormat("Importing from package: {0}", target.Name);
            return ShowDialog(owner, new ImportEncryptedDataPackageDialog(target));
        }

        public static bool ImportFromMobile(IWin32Window owner, View target)
        {
            Log.Logger.DebugFormat("Importing from mobile: {0}", target.Name);
            return ShowDialog(owner, new ImportPhoneDataForm(target));
        }

        public static bool ExportToPackage(IWin32Window owner, View source)
        {
            Log.Logger.DebugFormat("Exporting to package: {0}", source.Name);
            return ShowDialog(owner, new PackageForTransportDialog(source.Project.FilePath, source));
        }
    }
}
