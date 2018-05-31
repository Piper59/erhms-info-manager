using Epi;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    [ContextSafe]
    public class LogListViewModel : DocumentViewModel
    {
        public class LogListChildViewModel : ListViewModel<FileInfo>
        {
            public LogListChildViewModel()
            {
                Refresh();
            }

            protected override IEnumerable<FileInfo> GetItems()
            {
                DirectoryInfo directory = new DirectoryInfo(Configuration.GetNewInstance().Directories.LogDir);
                return directory.SearchByExtension(Log.FileExtension).OrderBy(log => log.FullName, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(FileInfo item)
            {
                yield return item.FullName;
            }
        }

        public LogListChildViewModel Logs { get; private set; }

        public ICommand OpenCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand PackageCommand { get; private set; }

        public LogListViewModel()
        {
            Title = "Logs";
            Logs = new LogListChildViewModel();
            OpenCommand = new Command(Open, Logs.HasAnySelectedItems);
            DeleteCommand = new AsyncCommand(DeleteAsync, Logs.HasAnySelectedItems);
            PackageCommand = new AsyncCommand(PackageAsync, Logs.HasAnySelectedItems);
        }

        public void Open()
        {
            foreach (FileInfo file in Logs.SelectedItems)
            {
                file.Refresh();
                if (file.Exists)
                {
                    ServiceLocator.Process.Start(new ProcessStartInfo
                    {
                        FileName = file.FullName
                    });
                }
            }
        }

        public async Task DeleteAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.LogConfirmDelete, "Delete"))
            {
                foreach (FileInfo file in Logs.SelectedItems)
                {
                    file.Refresh();
                    if (file.Exists)
                    {
                        try
                        {
                            IOExtensions.RecycleFile(file.FullName);
                        }
                        catch (OperationCanceledException) { }
                    }
                }
                ServiceLocator.Data.Refresh(typeof(FileInfo));
            }
        }

        public async Task PackageAsync()
        {
            string path = ServiceLocator.Dialog.SaveFile(
                "Package Logs",
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                FileDialogExtensions.Filters.ZipFiles,
                string.Format("Logs-{0:yyyyMMdd}-{0:HHmmss}.zip", DateTime.Now));
            if (path != null)
            {
                await ServiceLocator.Dialog.BlockAsync(Resources.LogPackaging, () =>
                {
                    using (ZipFile package = new ZipFile())
                    {
                        foreach (FileInfo file in Logs.SelectedItems)
                        {
                            file.Refresh();
                            if (file.Exists)
                            {
                                package.AddFile(file.FullName);
                            }
                        }
                        using (Stream stream = File.OpenWrite(path))
                        {
                            package.Save(stream);
                        }
                    }
                });
                ServiceLocator.Dialog.Notify(Resources.LogPackaged);
            }
        }
    }
}
