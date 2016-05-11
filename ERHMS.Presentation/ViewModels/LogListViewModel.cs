using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Data;

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
                        log.Recycle();
                    }
                }
                Refresh();
            };
            Messenger.Default.Send(msg);
        }

        public void Package()
        {
            // TODO: Implement
        }
    }
}
