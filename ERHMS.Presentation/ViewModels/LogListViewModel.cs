﻿using Epi;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class LogListViewModel : ListViewModel<FileInfo>
    {
        private RelayCommand openCommand;
        public ICommand OpenCommand
        {
            get { return openCommand ?? (openCommand = new RelayCommand(Open, HasSelectedItem)); }
        }

        private RelayCommand deleteCommand;
        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new RelayCommand(Delete, HasSelectedItem)); }
        }

        private RelayCommand packageCommand;
        public ICommand PackageCommand
        {
            get { return packageCommand ?? (packageCommand = new RelayCommand(Package, HasSelectedItem)); }
        }

        public LogListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Logs";
            SelectionChanged += (sender, e) =>
            {
                openCommand.RaiseCanExecuteChanged();
                deleteCommand.RaiseCanExecuteChanged();
                packageCommand.RaiseCanExecuteChanged();
            };
            Refresh();
        }

        protected override IEnumerable<FileInfo> GetItems()
        {
            DirectoryInfo directory = new DirectoryInfo(Configuration.GetNewInstance().Directories.LogDir);
            return directory.SearchByExtension(Log.FileExtension).OrderBy(log => log.FullName);
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
                MessengerInstance.Send(new RefreshMessage(typeof(FileInfo)));
            };
            MessengerInstance.Send(msg);
        }

        public void Package()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Package Logs";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dialog.Filter = FileDialogExtensions.GetFilter("ZIP Files", ".zip");
                dialog.FileName = string.Format("Logs-{0:yyyyMMdd}-{0:HHmmss}.zip", DateTime.Now);
                if (dialog.ShowDialog(Dialogs.Win32Window) == DialogResult.OK)
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
                        MessengerInstance.Send(new ToastMessage
                        {
                            Message = "Logs have been packaged."
                        });
                    };
                    MessengerInstance.Send(msg);
                }
            }
        }
    }
}
