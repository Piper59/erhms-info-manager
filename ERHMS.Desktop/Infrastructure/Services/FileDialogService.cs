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
            bool? result = dialog.ShowDialog(Application.Current.MainWindow);
            FileName = dialog.FileName;
            return result;
        }

        public bool? Open()
        {
            return Show(new OpenFileDialog
            {
                Title = ResXResources.FileDialog_Title_Open,
                InitialDirectory = InitialDirectory,
                Filter = Filter
            });
        }

        public bool? Save()
        {
            return Show(new SaveFileDialog
            {
                Title = ResXResources.FileDialog_Title_Save,
                InitialDirectory = InitialDirectory,
                FileName = FileName,
                Filter = Filter
            });
        }
    }
}
