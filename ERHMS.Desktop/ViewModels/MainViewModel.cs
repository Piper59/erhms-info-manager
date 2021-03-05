using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.EpiInfo;
using log4net.Appender;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Input;
using Module = ERHMS.EpiInfo.Module;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public static MainViewModel Instance { get; } = new MainViewModel();

        private static string GetLogFilePath()
        {
            return Log.Default.Logger.Repository.GetAppenders()
                .OfType<FileAppender>()
                .Single()
                .File;
        }

        private static string GetLogDirectoryPath()
        {
            return Path.GetDirectoryName(GetLogFilePath());
        }

        private object content;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Default.Debug($"Displaying content: {value}");
                SetProperty(ref content, value);
            }
        }

        public ICommand ExitCommand { get; }
        public ICommand ViewHomeCommand { get; }
        public ICommand ViewLogCommand { get; }
        public ICommand ViewLogsCommand { get; }
        public ICommand ExportLogsCommand { get; }
        public ICommand StartEpiInfoCommand { get; }
        public ICommand StartFileExplorerCommand { get; }
        public ICommand StartCommandPromptCommand { get; }

        private MainViewModel()
        {
            ExitCommand = new SyncCommand(Exit);
            ViewHomeCommand = new SyncCommand(ViewHome);
            ViewLogCommand = new SyncCommand(ViewLog);
            ViewLogsCommand = new SyncCommand(ViewLogs);
            ExportLogsCommand = new SyncCommand(ExportLogs);
            StartEpiInfoCommand = new SyncCommand(StartEpiInfo);
            StartFileExplorerCommand = new SyncCommand(StartFileExplorer);
            StartCommandPromptCommand = new SyncCommand(StartCommandPrompt);
        }

        public event EventHandler ExitRequested;
        private void OnExitRequested(EventArgs e) => ExitRequested?.Invoke(this, e);
        private void OnExitRequested() => OnExitRequested(EventArgs.Empty);

        public void Exit()
        {
            OnExitRequested();
        }

        public void ViewHome()
        {
            Content = new HomeViewModel();
        }

        public void ViewLog()
        {
            Process.Start(GetLogFilePath());
        }

        public void ViewLogs()
        {
            Process.Start(GetLogDirectoryPath());
        }

        public void ExportLogs()
        {
            string archivePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Logs.zip");
            using (Stream archiveStream = File.Open(archivePath, FileMode.Create, FileAccess.Write))
            using (ZipArchive archive = new ZipArchive(archiveStream, ZipArchiveMode.Create))
            {
                DirectoryInfo logDirectory = new DirectoryInfo(GetLogDirectoryPath());
                Uri logDirectoryUri = new Uri(logDirectory.FullName);
                foreach (FileInfo logFile in logDirectory.EnumerateFiles("*.txt", SearchOption.AllDirectories))
                {
                    Uri logFileUri = new Uri(logFile.FullName);
                    string entryName = logDirectoryUri.MakeRelativeUri(logFileUri).ToString();
                    using (Stream logFileStream = logFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (Stream entryStream = archive.CreateEntry(entryName).Open())
                    {
                        logFileStream.CopyTo(entryStream);
                    }
                }
            }
        }

        public void StartEpiInfo()
        {
            Module.Menu.Start();
        }

        public void StartFileExplorer()
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }

        public void StartCommandPrompt()
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                FileName = Environment.GetEnvironmentVariable("ComSpec")
            });
        }
    }
}
