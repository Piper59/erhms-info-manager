using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Projects;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Module = ERHMS.EpiInfo.Module;
using View = Epi.View;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public static MainViewModel Current { get; } = new MainViewModel();

        private object content;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Default.Debug($"Displaying: {value}");
                SetProperty(ref content, value);
            }
        }

        public Command ExitCommand { get; }
        public Command GoToHomeCommand { get; }
        public Command OpenWorkerProjectCommand { get; }
        public Command OpenIncidentProjectCommand { get; }
        public Command OpenViewCommand { get; }
        public Command StartEpiInfoCommand { get; }
        public Command StartFileExplorerCommand { get; }

        private MainViewModel()
        {
            ExitCommand = new SimpleSyncCommand(Exit);
            GoToHomeCommand = new SimpleSyncCommand(GoToHome);
            OpenWorkerProjectCommand = new SimpleAsyncCommand(OpenWorkerProjectAsync);
            OpenIncidentProjectCommand = new SimpleAsyncCommand(OpenIncidentProjectAsync);
            OpenViewCommand = new SimpleAsyncCommand<View>(OpenViewAsync);
            StartEpiInfoCommand = new SimpleSyncCommand(StartEpiInfo);
            StartFileExplorerCommand = new SimpleSyncCommand(StartFileExplorer);
        }

        public event EventHandler ExitRequested;

        private void OnExitRequested()
        {
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Exit()
        {
            OnExitRequested();
        }

        private void GoToHome()
        {
            Content = new HomeViewModel();
        }

        private async Task OpenProjectAsync(ProjectType projectType, string path)
        {
            // TODO: Handle errors
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningProjectTaskName);
            await progress.RunAsync(() =>
            {
                content = new ProjectViewModel(ProjectFactory.GetProject(projectType, path));
            });
            OnPropertyChanged(nameof(Content));
        }

        private async Task OpenWorkerProjectAsync()
        {
            await OpenProjectAsync(ProjectType.Worker, Settings.Default.WorkerProjectPath);
        }

        private async Task OpenIncidentProjectAsync()
        {
            await OpenProjectAsync(ProjectType.Incident, Settings.Default.IncidentProjectPath);
        }

        private async Task OpenViewAsync(View view)
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningViewTaskName);
            await progress.RunAsync(() =>
            {
                content = new ViewViewModel(view);
            });
            OnPropertyChanged(nameof(Content));
        }

        private void StartEpiInfo()
        {
            Module.Menu.Start();
        }

        private void StartFileExplorer()
        {
            string entryDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Process.Start(entryDirectory);
        }
    }
}
