﻿using System.IO;
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

        public static string GetRootDirectory(this FolderBrowserDialog @this)
        {
            return Path.Combine(@this.SelectedPath, App.BareTitle);
        }
    }
}
