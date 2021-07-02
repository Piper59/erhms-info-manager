using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string InitialDirectory { get; set; }
        public string InitialFileName { get; set; }

        private string fileName;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                SetInitialPath(value);
                fileName = value;
            }
        }

        public string Filter { get; set; }

        public event CancelEventHandler FileOk;

        private void SetInitialPath(string path)
        {
            InitialDirectory = Path.GetDirectoryName(path);
            InitialFileName = Path.GetFileName(path);
        }

        private bool? Show(FileDialog dialog)
        {
            dialog.InitialDirectory = InitialDirectory;
            dialog.FileName = InitialFileName;
            dialog.Filter = Filter;
            dialog.FileOk += (sender, e) =>
            {
                string oldFileName = FileName;
                fileName = dialog.FileName;
                FileOk?.Invoke(this, e);
                if (e.Cancel)
                {
                    fileName = oldFileName;
                }
                else
                {
                    SetInitialPath(fileName);
                }
            };
            Window owner = Application.Current.GetActiveWindow();
            owner.EnsureHandle();
            return dialog.ShowDialog(owner);
        }

        public bool? Open()
        {
            return Show(new OpenFileDialog
            {
                Title = Strings.FileDialog_Title_Open
            });
        }

        public bool? Save()
        {
            return Show(new SaveFileDialog
            {
                Title = Strings.FileDialog_Title_Save
            });
        }
    }
}
