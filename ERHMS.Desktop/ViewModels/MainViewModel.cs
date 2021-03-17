using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Module = ERHMS.EpiInfo.Module;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public static MainViewModel Instance { get; } = new MainViewModel();

        private ViewModel content;
        public ViewModel Content
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
        public ICommand GoToCoreProjectCommand { get; }
        public ICommand GoToCoreViewCommand { get; }
        public ICommand OpenLogFileCommand { get; }
        public ICommand OpenLogDirectoryCommand { get; }
        public ICommand ExportLogDirectoryCommand { get; }
        public ICommand StartEpiInfoMenuCommand { get; }
        public ICommand StartFileExplorerCommand { get; }
        public ICommand StartCommandPromptCommand { get; }
        public ICommand ExitCommand { get; }

        private MainViewModel()
        {
            GoToHomeCommand = new SyncCommand(GoToHome);
            GoToCoreProjectCommand = new AsyncCommand<CoreProject>(GoToCoreProjectAsync);
            GoToCoreViewCommand = new AsyncCommand<CoreView>(GoToCoreViewAsync);
            OpenLogFileCommand = new SyncCommand(OpenLogFile);
            OpenLogDirectoryCommand = new SyncCommand(OpenLogDirectory);
            ExportLogDirectoryCommand = new AsyncCommand(ExportLogDirectoryAsync);
            StartEpiInfoMenuCommand = new SyncCommand(StartEpiInfoMenu);
            StartFileExplorerCommand = new SyncCommand(StartFileExplorer);
            StartCommandPromptCommand = new SyncCommand(StartCommandPrompt);
            ExitCommand = new SyncCommand(Exit);
        }

        public event EventHandler ExitRequested;
        private void OnExitRequested(EventArgs e) => ExitRequested?.Invoke(this, e);
        private void OnExitRequested() => OnExitRequested(EventArgs.Empty);

        public void GoToHome()
        {
            Content = new HomeViewModel();
        }

        public async Task GoToCoreProjectAsync(CoreProject coreProject)
        {
            await ServiceLocator.Resolve<IProgressService>().RunAsync(
                ResXResources.Lead_OpeningProject,
                async () =>
                {
                    ProjectViewModel project = new ProjectViewModel(await Task.Run(() =>
                    {
                        return Settings.Default.GetProject(coreProject);
                    }));
                    await project.InitializeAsync();
                    Content = project;
                });
        }

        public async Task GoToCoreViewAsync(CoreView coreView)
        {
            await ServiceLocator.Resolve<IProgressService>().RunAsync(
                ResXResources.Lead_OpeningView,
                async () =>
                {
                    ViewViewModel view = new ViewViewModel(await Task.Run(() =>
                    {
                        return Settings.Default.GetView(coreView);
                    }));
                    await view.InitializeAsync();
                    Content = view;
                });
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
            bool? result = ServiceLocator.Resolve<IFileDialogService>().Save(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                string.Format(ResXResources.FileName_LogDirectoryArchive, DateTime.Now),
                ResXResources.FileDialog_Filter_ZipFiles,
                out string path);
            if (result != true)
            {
                return;
            }
            await ServiceLocator.Resolve<IProgressService>().RunAsync(
                ResXResources.Lead_ExportingLogDirectory,
                () =>
                {
                    ZipExtensions.CreateFromDirectory(Log.DirectoryPath, path);
                });
            ServiceLocator.Resolve<INotificationService>().Notify(ResXResources.Body_ExportedLogDirectory);
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

        public void Exit()
        {
            OnExitRequested();
        }
    }
}
