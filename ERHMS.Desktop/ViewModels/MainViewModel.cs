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
        public Command GoHomeCommand { get; }
        public Command ViewWorkerProjectCommand { get; }
        public Command ViewIncidentProjectCommand { get; }
        public Command StartEpiInfoCommand { get; }
        public Command StartFileExplorerCommand { get; }

        private MainViewModel()
        {
            ExitCommand = new SimpleSyncCommand(Exit);
            GoHomeCommand = new SimpleSyncCommand(GoHome);
            ViewWorkerProjectCommand = new SimpleAsyncCommand<string>(ViewWorkerProjectAsync);
            ViewIncidentProjectCommand = new SimpleAsyncCommand<string>(ViewIncidentProjectAsync);
            StartEpiInfoCommand = new SimpleSyncCommand(StartEpiInfo);
            StartFileExplorerCommand = new SimpleSyncCommand(StartFileExplorer);
        }

        public event EventHandler ExitRequested;

        private void OnExitRequested()
        {
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Exit()
        {
            OnExitRequested();
        }

        public void GoHome()
        {
            Content = new HomeViewModel();
        }

        // TODO: Handle errors
        // Path and setting are null: offer to create/open
        // Project does not exist: remove from settings, offer to create/open

        public async Task ViewWorkerProjectAsync(string path = null)
        {
            path = path ?? Settings.Default.WorkerProjectPath;
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningProjectTaskName);
            await progress.RunAsync(() =>
            {
                content = new ProjectViewModel(new WorkerProject(path));
                if (Settings.Default.WorkerProjectPath != path)
                {
                    Settings.Default.WorkerProjectPath = path;
                    Settings.Default.Save();
                }
            });
            OnPropertyChanged(nameof(Content));
        }

        public async Task ViewIncidentProjectAsync(string path = null)
        {
            path = path ?? Settings.Default.IncidentProjectPath;
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningProjectTaskName);
            await progress.RunAsync(() =>
            {
                content = new ProjectViewModel(new IncidentProject(path));
                if (Settings.Default.IncidentProjectPath != path)
                {
                    Settings.Default.IncidentProjectPath = path;
                    Settings.Default.IncidentProjectPaths.Remove(path);
                    Settings.Default.IncidentProjectPaths.Insert(0, path);
                    Settings.Default.Save();
                }
            });
            OnPropertyChanged(nameof(Content));
        }

        public void StartEpiInfo()
        {
            Module.Menu.Start();
        }

        public void StartFileExplorer()
        {
            string entryDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Process.Start(entryDirectory);
        }
    }
}
