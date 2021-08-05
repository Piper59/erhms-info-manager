using Epi;
using ERHMS.Common.ComponentModel;
using ERHMS.Common.Compression;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.Desktop.Wizards;
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
        public ICommand CreateProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand GoToProjectCommand { get; }
        public ICommand GoToViewCommand { get; }
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
            CreateProjectCommand = new SyncCommand<CoreProject>(CreateProject);
            OpenProjectCommand = new SyncCommand<CoreProject>(OpenProject);
            GoToProjectCommand = new AsyncCommand<CoreProject>(GoToProjectAsync);
            GoToViewCommand = new AsyncCommand<CoreView>(GoToViewAsync);
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

        public void CreateProject(CoreProject coreProject)
        {
            CreateProjectViewModel wizard = new CreateProjectViewModel(coreProject);
            if (wizard.Show() != true)
            {
                return;
            }
            Settings.Default.SetProjectPath(coreProject, wizard.Project.FilePath);
            Settings.Default.Save();
        }

        public void OpenProject(CoreProject coreProject)
        {
            // TODO: Implement
        }

        public async Task GoToProjectAsync(Func<Task<Project>> action)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_LoadingProject;
            await progress.Run(async () =>
            {
                Content = await ProjectViewModel.CreateAsync(await action());
            });
        }

        public async Task GoToProjectAsync(CoreProject coreProject)
        {
            // TODO: Handle errors
            await GoToProjectAsync(() => Task.Run(() =>
            {
                string projectPath = Settings.Default.GetProjectPath(coreProject);
                return ProjectExtensions.Open(projectPath);
            }));
        }

        public async Task GoToViewAsync(Func<Task<View>> action)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_LoadingView;
            await progress.Run(async () =>
            {
                Content = await ViewViewModel.CreateAsync(await action());
            });
        }

        public async Task GoToViewAsync(CoreView coreView)
        {
            // TODO: Handle errors
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
            fileDialog.InitialFileName = $"Logs_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            fileDialog.Filter = Strings.FileDialog_Filter_ZipFiles;
            if (fileDialog.Save() != true)
            {
                return;
            }
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_ExportingLogs;
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
