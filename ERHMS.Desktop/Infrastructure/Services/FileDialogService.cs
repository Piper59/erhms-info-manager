using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using Microsoft.Win32;
using System.Collections.Generic;
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
                InitialDirectory = Path.GetDirectoryName(value);
                InitialFileName = Path.GetFileName(value);
                fileName = value;
            }
        }

        public string Filter { get; set; }

        public IEnumerable<string> Filters
        {
            set { Filter = string.Join("|", value); }
        }

        private bool? Show(FileDialog dialog)
        {
            dialog.InitialDirectory = InitialDirectory;
            dialog.FileName = InitialFileName;
            dialog.Filter = Filter;
            Window owner = Application.Current.GetActiveWindow();
            owner.EnsureHandle();
            if (dialog.ShowDialog(owner) == true)
            {
                FileName = dialog.FileName;
                return true;
            }
            else
            {
                return false;
            }
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
