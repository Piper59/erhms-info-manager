using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Module = ERHMS.EpiInfo.Module;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public static MainViewModel Instance { get; } = new MainViewModel();

        private object content;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Instance.Debug($"Setting main content: {value}");
                SetProperty(ref content, value);
            }
        }

        public ICommand GoToHomeCommand { get; }
        public ICommand GoToHelpCommand { get; }
        public ICommand GoToCoreProjectCommand { get; }
        public ICommand GoToCoreViewCommand { get; }
        public ICommand OpenLogFileCommand { get; }
        public ICommand OpenLogDirectoryCommand { get; }
        public ICommand ExportLogDirectoryCommand { get; }
        public ICommand StartEpiInfoMenuCommand { get; }
        public ICommand StartFileExplorerCommand { get; }
        public ICommand StartCommandPromptCommand { get; }

        private MainViewModel()
        {
            GoToHomeCommand = new SyncCommand(GoToHome);
            GoToHelpCommand = Command.Null;
            GoToCoreProjectCommand = new AsyncCommand<CoreProject>(GoToCoreProjectAsync);
            GoToCoreViewCommand = new AsyncCommand<CoreView>(GoToCoreViewAsync);
            OpenLogFileCommand = new SyncCommand(OpenLogFile);
            OpenLogDirectoryCommand = new SyncCommand(OpenLogDirectory);
            ExportLogDirectoryCommand = new AsyncCommand(ExportLogDirectoryAsync);
            StartEpiInfoMenuCommand = new SyncCommand(StartEpiInfoMenu);
            StartFileExplorerCommand = new SyncCommand(StartFileExplorer);
            StartCommandPromptCommand = new SyncCommand(StartCommandPrompt);
        }

        public void GoToHome()
        {
            Content = new HomeViewModel();
        }

        public async Task GoToProjectAsync(string projectPath)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = ResXResources.Lead_LoadingProject;
            await progress.RunAsync(async () =>
            {
                Project value = await Task.Run(() =>
                {
                    return ProjectExtensions.Open(projectPath);
                });
                Content = await ProjectViewModel.CreateAsync(value);
            });
        }

        public async Task GoToCoreProjectAsync(CoreProject coreProject)
        {
            await GoToProjectAsync(Settings.Default.GetProjectPath(coreProject));
        }

        public async Task GoToViewAsync(string projectPath, string viewName)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = ResXResources.Lead_LoadingView;
            await progress.RunAsync(async () =>
            {
                View value = await Task.Run(() =>
                {
                    return ProjectExtensions.Open(projectPath).Views[viewName];
                });
                Content = await ViewViewModel.CreateAsync(value);
            });
        }

        public async Task GoToCoreViewAsync(CoreView coreView)
        {
            await GoToViewAsync(Settings.Default.GetProjectPath(coreView.CoreProject), coreView.Name);
        }

        public void OpenLogFile()
        {
            Process.Start(Log.FilePath);
        }

        public void OpenLogDirectory()
        {
            Process.Start(Log.DirectoryPath);
        }

        public async Task ExportLogDirectoryAsync()
        {
            IFileDialogService fileDialog = ServiceLocator.Resolve<IFileDialogService>();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            fileDialog.FileName = string.Format(ResXResources.FileName_LogDirectoryArchive, DateTime.Now);
            fileDialog.Filter = ResXResources.FileDialog_Filter_ZipFiles;
            if (fileDialog.Save() == true)
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Title = ResXResources.Lead_ExportingLogDirectory;
                await progress.RunAsync(() =>
                {
                    ZipExtensions.CreateFromDirectory(
                        Log.DirectoryPath,
                        fileDialog.FileName,
                        "*.txt",
                        FileShare.ReadWrite);
                });
            }
        }

        public async Task StartEpiInfoAsync(Module module, params string[] args)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Delay = TimeSpan.Zero;
            progress.Title = ResXResources.Lead_StartingEpiInfo;
            await progress.RunAsync(async () =>
            {
                Process process = module.Start(args);
                await Task.Run(() =>
                {
                    process.WaitForExit(3000);
                });
            });
        }

        public void StartEpiInfoMenu()
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
