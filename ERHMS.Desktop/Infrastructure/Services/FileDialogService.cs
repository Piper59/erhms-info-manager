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

        public bool? Open(string initialDirectory, string filter, out string fileName)
        {
            Window owner = Application.GetActiveOrMainWindow();
            FileDialog dialog = new OpenFileDialog
            {
                Title = "Open",
                InitialDirectory = initialDirectory,
                Filter = filter
            };
            bool? result = dialog.ShowDialog(owner);
            fileName = dialog.FileName;
            return result;
        }

        public bool? Save(string initialDirectory, string initialFileName, string filter, out string fileName)
        {
            Window owner = Application.GetActiveOrMainWindow();
            FileDialog dialog = new SaveFileDialog
            {
                Title = "Save As",
                InitialDirectory = initialDirectory,
                FileName = initialFileName,
                Filter = filter
            };
            bool? result = dialog.ShowDialog(owner);
            fileName = dialog.FileName;
            return result;
        }
    }
}
