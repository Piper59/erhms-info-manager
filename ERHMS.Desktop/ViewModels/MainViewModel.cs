using Epi;
using ERHMS.Common.ComponentModel;
using ERHMS.Common.Compression;
using ERHMS.Common.Logging;
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

        private object content = new HomeViewModel();
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Instance.Debug($"Navigating: {value}");
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

        public async Task GoToProjectAsync(Func<Task<Project>> action)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = Strings.Lead_LoadingProject;
            Content = await progress.Run(async () =>
            {
                ProjectViewModel content = new ProjectViewModel(await action());
                await content.InitializeAsync();
                return content;
            });
        }

        public async Task GoToCoreProjectAsync(CoreProject coreProject)
        {
            await GoToProjectAsync(() => Task.Run(() =>
            {
                string projectPath = Settings.Default.GetProjectPath(coreProject);
                return ProjectExtensions.Open(projectPath);
            }));
        }

        public async Task GoToViewAsync(Func<Task<View>> action)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = Strings.Lead_LoadingView;
            Content = await progress.Run(async () =>
            {
                ViewViewModel content = new ViewViewModel(await action());
                await content.InitializeAsync();
                return content;
            });
        }

        public async Task GoToCoreViewAsync(CoreView coreView)
        {
            await GoToViewAsync(() => Task.Run(() =>
            {
                string projectPath = Settings.Default.GetProjectPath(coreView.CoreProject);
                Project project = ProjectExtensions.Open(projectPath);
                return project.Views[coreView.Name];
            }));
        }

        public void OpenLogFile()
        {
            Process.Start(Log.Instance.GetFile())?.Dispose();
        }

        public void OpenLogDirectory()
        {
            Process.Start(FileAppender.Directory)?.Dispose();
        }

        public async Task ExportLogDirectoryAsync()
        {
            IFileDialogService fileDialog = ServiceLocator.Resolve<IFileDialogService>();
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            fileDialog.FileName = $"Logs_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            fileDialog.Filter = Strings.FileDialog_Filter_Archives;
            if (fileDialog.Save() != true)
            {
                return;
            }
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Title = Strings.Lead_ExportingLogDirectory;
            await progress.Run(() =>
            {
                ZipFileExtensions.CreateFromDirectory(
                    FileAppender.Directory,
                    fileDialog.FileName,
                    $"*{FileAppender.Extension}",
                    FileMode.Create,
                    FileShare.ReadWrite);
            });
        }

        public async Task StartEpiInfoAsync(Module module, params string[] args)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Delay = TimeSpan.Zero;
            progress.Title = Strings.Lead_StartingEpiInfo;
            await progress.Run(() =>
            {
                using (Process process = module.Start(args))
                {
                    process.WaitForExit(3000);
                }
            });
        }

        public void StartEpiInfoMenu()
        {
            Module.Menu.Start()?.Dispose();
        }

        public void StartFileExplorer()
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory)?.Dispose();
        }

        public void StartCommandPrompt()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                FileName = Environment.GetEnvironmentVariable("ComSpec")
            };
            Process.Start(startInfo)?.Dispose();
        }
    }
}
