using ERHMS.Desktop.Infrastructure;
using Microsoft.Win32;
using System.Windows;

namespace ERHMS.Desktop.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string Title { get; set; }
        public string InitialDirectory { get; set; }
        public string Filter { get; set; }

        private readonly Application application;

        public FileDialogService(Application application)
        {
            this.application = application;
        }

        public bool? Open(out string path)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = Title,
                InitialDirectory = InitialDirectory,
                Filter = Filter
            };
            bool? result = dialog.ShowDialog(application.GetActiveWindow());
            path = result.GetValueOrDefault() ? dialog.FileName : null;
            return result;
        }
    }
}
