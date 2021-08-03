using ERHMS.Desktop.Services;
using System;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class DirectoryDialogService : IDirectoryDialogService
    {
        public string Directory { get; set; }

        public bool? Open()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = Directory;
                Window owner = Application.Current.GetActiveWindow();
                IntPtr handle = owner.EnsureHandle();
                if (dialog.ShowDialog(NativeWindow.FromHandle(handle)) == DialogResult.OK)
                {
                    Directory = dialog.SelectedPath;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
