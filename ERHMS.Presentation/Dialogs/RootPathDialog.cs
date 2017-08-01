using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ERHMS.Presentation.Dialogs
{
    public static class RootPathDialog
    {
        private const string DirectoryName = App.BareTitle;

        public static FolderBrowserDialog GetDialog()
        {
            string format = "Choose a location for your application files. We'll create a directory named {0} in that location.";
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                Description = string.Format(format, DirectoryName)
            };
            return dialog;
        }

        public static bool Show(this FolderBrowserDialog @this, IWin32Window owner)
        {
            DialogResult result;
            while (true)
            {
                result = @this.ShowDialog(owner);
                if (result == DialogResult.OK && Directory.Exists(@this.GetRootPath()))
                {
                    if (@this.Confirm())
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return result == DialogResult.OK;
        }

        private static bool Confirm(this FolderBrowserDialog @this)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("The following directory already exists. Are you sure you want to use this location?");
            message.AppendLine();
            message.Append(@this.GetRootPath());
            return MessageBox.Show(message.ToString(), App.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        public static string GetRootPath(this FolderBrowserDialog @this)
        {
            return Path.Combine(@this.SelectedPath, App.BareTitle);
        }
    }
}
