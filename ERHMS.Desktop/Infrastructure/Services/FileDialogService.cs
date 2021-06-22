using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string InitialDirectory { get; set; }
        public string InitialFileName { get; set; }
        public string FileName { get; set; }
        public string Filter { get; set; }

        public event CancelEventHandler FileOk;

        private bool? Show(FileDialog dialog)
        {
            dialog.FileOk += (sender, e) =>
            {
                FileName = dialog.FileName;
                FileOk?.Invoke(this, e);
            };
            Window owner = Application.Current.GetActiveWindow();
            owner.EnsureHandle();
            return dialog.ShowDialog(owner);
        }

        public bool? Open()
        {
            return Show(new OpenFileDialog
            {
                Title = Strings.FileDialog_Title_Open,
                InitialDirectory = InitialDirectory,
                Filter = Filter
            });
        }

        public bool? Save()
        {
            return Show(new SaveFileDialog
            {
                Title = Strings.FileDialog_Title_Save,
                InitialDirectory = InitialDirectory,
                FileName = InitialFileName,
                Filter = Filter
            });
        }
    }
}
