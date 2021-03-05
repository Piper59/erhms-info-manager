using ERHMS.Desktop.Services;
using Microsoft.Win32;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class FileDialogService : IFileDialogService
    {
        public Application Application { get; }

        public FileDialogService(Application application)
        {
            Application = application;
        }

        private bool? Show(FileDialog dialog, out string fileName)
        {
            bool? result = dialog.ShowDialog(Application.GetActiveOrMainWindow());
            fileName = dialog.FileName;
            return result;
        }

        public bool? Open(string initialDirectory, string filter, out string fileName)
        {
            FileDialog dialog = new OpenFileDialog
            {
                Title = "Open",
                InitialDirectory = initialDirectory,
                Filter = filter
            };
            return Show(dialog, out fileName);
        }

        public bool? Save(string initialDirectory, string initialFileName, string filter, out string fileName)
        {
            FileDialog dialog = new SaveFileDialog
            {
                Title = "Save As",
                InitialDirectory = initialDirectory,
                FileName = initialFileName,
                Filter = filter
            };
            return Show(dialog, out fileName);
        }
    }
}
