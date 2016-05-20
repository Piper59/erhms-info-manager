using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Data;
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
            Selecting += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                PackageCommand.RaiseCanExecuteChanged();
            };
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            PackageCommand = new RelayCommand(Package, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
        }

        private IEnumerable<FileInfo> GetLogs(DirectoryInfo directory)
        {
            return directory.GetSubdirectory("Logs").SearchByExtension(".txt");
        }

        protected override ICollectionView GetItems()
        {
            ICollection<FileInfo> items = new List<FileInfo>();
            foreach (FileInfo log in GetLogs(ConfigurationExtensions.GetApplicationRoot()))
            {
                items.Add(log);
            }
            foreach (FileInfo log in GetLogs(ConfigurationExtensions.GetConfigurationRoot()))
            {
                items.Add(log);
            }
            return CollectionViewSource.GetDefaultView(items.OrderBy(log => log.FullName));
        }

        protected override IEnumerable<string> GetFilteredValues(FileInfo item)
        {
            yield return item.FullName;
        }

        public void Open()
        {
            foreach (FileInfo log in SelectedItems)
            {
                if (log.Exists)
                {
                    Process.Start(log.FullName);
                }
            }
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Delete?",
                string.Format("Are you sure you want to delete {0}?", SelectedItems.Count > 1 ? "these logs" : "this log"),
                "Delete",
                "Don't Delete");
            msg.Confirmed += (sender, e) =>
            {
                foreach (FileInfo log in SelectedItems)
                {
                    if (log.Exists)
                    {
                        try
                        {
                            log.Recycle();
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
                dialog.Filter = "ZIP Files (*.zip)|*.zip";
                dialog.FileName = string.Format("Logs-{0:yyyyMMdd}-{0:HHmmss}.zip", DateTime.Now);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    BlockMessage msg = new BlockMessage("Packaging logs \u2026");
                    msg.Executing += (sender, e) =>
                    {
                        using (Stream stream = dialog.OpenFile())
                        using (ZipFile zip = new ZipFile())
                        {
                            foreach (FileInfo log in SelectedItems)
                            {
                                zip.AddFile(log.FullName);
                            }
                            zip.Save(stream);
                        }
                    };
                    msg.Executed += (sender, e) =>
                    {
                        Messenger.Default.Send(new ToastMessage("Logs have been packaged."));
                    };
                    Messenger.Default.Send(msg);
                }
            }
        }
    }
}
