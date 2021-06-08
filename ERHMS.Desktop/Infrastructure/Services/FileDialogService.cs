using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using Microsoft.Win32;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public string Filter { get; set; }

        private bool? Show(FileDialog dialog)
        {
            Window owner = Application.Current.GetActiveWindow();
            owner.EnsureHandle();
            bool? result = dialog.ShowDialog(owner);
            FileName = dialog.FileName;
            return result;
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
                FileName = FileName,
                Filter = Filter
            });
        }
    }
}
