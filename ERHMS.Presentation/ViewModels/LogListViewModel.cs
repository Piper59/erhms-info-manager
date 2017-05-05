using Epi;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ERHMS.Presentation.ViewModels
{
    public class LogListViewModel : ListViewModelBase<FileInfo>
    {
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand PackageCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public LogListViewModel()
        {
            Title = "Logs";
            Refresh();
            OpenCommand = new RelayCommand(Open, HasAnySelectedItems);
            DeleteCommand = new RelayCommand(Delete, HasAnySelectedItems);
            PackageCommand = new RelayCommand(Package, HasAnySelectedItems);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                PackageCommand.RaiseCanExecuteChanged();
            };
        }

        private IEnumerable<FileInfo> GetLogs(DirectoryInfo directory)
        {
            return directory.SearchByExtension(".txt");
        }

        protected override IEnumerable<FileInfo> GetItems()
        {
            Configuration configuration = Configuration.GetNewInstance();
            DirectoryInfo directory = new DirectoryInfo(configuration.Directories.LogDir);
            return directory.SearchByExtension(Path.GetExtension(Log.FilePath)).OrderBy(item => item.FullName);
        }

        protected override IEnumerable<string> GetFilteredValues(FileInfo item)
        {
            yield return item.FullName;
        }

        public void Open()
        {
            foreach (FileInfo log in SelectedItems)
            {
                log.Refresh();
                if (log.Exists)
                {
                    Process.Start(log.FullName);
                }
            }
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected logs?"
            };
            msg.Confirmed += (sender, e) =>
            {
                foreach (FileInfo log in SelectedItems)
                {
                    log.Refresh();
                    if (log.Exists)
                    {
                        try
                        {
                            IOExtensions.RecycleFile(log.FullName);
                        }
                        catch (OperationCanceledException) { }
                    }
                }
                Refresh();
            };
            Messenger.Default.Send(msg);
        }

        public void Package()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Package Logs";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dialog.Filter = FileDialogExtensions.GetFilter("ZIP Files", ".zip");
                dialog.FileName = string.Format("Logs-{0:yyyyMMdd}-{0:HHmmss}.zip", DateTime.Now);
                if (dialog.ShowDialog(App.Current.MainWin32Window) == DialogResult.OK)
                {
                    BlockMessage msg = new BlockMessage
                    {
                        Message = "Packaging logs \u2026"
                    };
                    msg.Executing += (sender, e) =>
                    {
                        using (ZipFile package = new ZipFile())
                        {
                            foreach (FileInfo log in SelectedItems)
                            {
                                log.Refresh();
                                if (log.Exists)
                                {
                                    package.AddFile(log.FullName);
                                }
                            }
                            using (Stream stream = dialog.OpenFile())
                            {
                                package.Save(stream);
                            }
                        }
                    };
                    msg.Executed += (sender, e) =>
                    {
                        Messenger.Default.Send(new ToastMessage
                        {
                            Message = "Logs have been packaged."
                        });
                    };
                    Messenger.Default.Send(msg);
                }
            }
        }
    }
}
