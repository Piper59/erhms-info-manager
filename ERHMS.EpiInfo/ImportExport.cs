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
            form.StartPosition = FormStartPosition.CenterParent;
            return form.ShowDialog(owner) == DialogResult.OK;
        }

        public static bool ImportFromView(IWin32Window owner, View target)
        {
            Log.Logger.DebugFormat("Importing from view: {0}", target.Name);
            using (Form form = new ImportDataForm(target))
            {
                return ShowDialog(owner, form);
            }
        }

        public static bool ImportFromPackage(IWin32Window owner, View target)
        {
            Log.Logger.DebugFormat("Importing from package: {0}", target.Name);
            using (Form form = new ImportEncryptedDataPackageDialog(target))
            {
                return ShowDialog(owner, form);
            }
        }

        public static bool ImportFromMobile(IWin32Window owner, View target)
        {
            Log.Logger.DebugFormat("Importing from mobile: {0}", target.Name);
            using (Form form = new ImportPhoneDataForm(target))
            {
                return ShowDialog(owner, form);
            }
        }

        public static bool ExportToPackage(IWin32Window owner, View source)
        {
            Log.Logger.DebugFormat("Exporting to package: {0}", source.Name);
            using (Form form = new PackageForTransportDialog(source.Project.FilePath, source))
            {
                return ShowDialog(owner, form);
            }
        }
    }
}
