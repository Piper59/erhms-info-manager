using System.IO;
using System.Windows.Forms;

namespace ERHMS.Presentation.Dialogs
{
    public static class RootDirectoryDialog
    {
        public static FolderBrowserDialog GetDialog()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = string.Format("Choose a location for your application files. We'll create a folder named {0} in that location.", App.BareTitle);
            return dialog;
        }

        public static DialogResult ShowDialog(this FolderBrowserDialog @this, bool verify)
        {
            DialogResult result;
            while (true)
            {
                result = @this.ShowDialog();
                if (result == DialogResult.OK && verify && Directory.Exists(@this.GetRootDirectory()))
                {
                    string message = string.Format(
                        "A folder named {0} already exists in the location you have selected. Are you sure you want to use this location?",
                        App.BareTitle);
                    if (MessageBox.Show(message, App.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        public static string GetRootDirectory(this FolderBrowserDialog @this)
        {
            return Path.Combine(@this.SelectedPath, App.BareTitle);
        }
    }
}
