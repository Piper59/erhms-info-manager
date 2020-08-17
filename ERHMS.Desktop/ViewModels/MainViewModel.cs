using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Events;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Projects;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Module = ERHMS.EpiInfo.Module;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private static readonly TimeSpan StartEpiInfoWait = TimeSpan.FromSeconds(10.0);
        private static readonly TimeSpan StartEpiInfoPollInterval = TimeSpan.FromSeconds(0.1);

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

        public ICommand ExitCommand { get; }
        public ICommand GoHomeCommand { get; }
        public ICommand ViewWorkerProjectCommand { get; }
        public ICommand ViewIncidentProjectCommand { get; }
        public ICommand StartEpiInfoCommand { get; }
        public ICommand StartFileExplorerCommand { get; }
        public ICommand ViewCoreViewCommand { get; }

        private MainViewModel()
        {
            ExitCommand = new SyncCommand(Exit);
            GoHomeCommand = new SyncCommand(GoHome);
            ViewWorkerProjectCommand = new AsyncCommand<string>(ViewWorkerProjectAsync);
            ViewIncidentProjectCommand = new AsyncCommand<string>(ViewIncidentProjectAsync);
            StartEpiInfoCommand = new AsyncCommand(StartEpiInfoAsync);
            StartFileExplorerCommand = new SyncCommand(StartFileExplorer);
            ViewCoreViewCommand = new AsyncCommand<CoreView>(ViewCoreViewAsync);
        }

        public event EventHandler<ProcessStartedEventArgs> ProcessStarted;
        private void OnProcessStarted(ProcessStartedEventArgs e) => ProcessStarted?.Invoke(this, e);
        private void OnProcessStarted(Process process) => OnProcessStarted(new ProcessStartedEventArgs(process));

        public event EventHandler ExitRequested;
        private void OnExitRequested() => ExitRequested?.Invoke(this, EventArgs.Empty);

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
            IProgressService progress = ServiceProvider.Resolve<IProgressService>();
            progress.Title = ResX.OpeningProjectTitle;
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

        // TODO: Handle errors
        public async Task ViewIncidentProjectAsync(string path = null)
        {
            path = path ?? Settings.Default.IncidentProjectPath;
            IProgressService progress = ServiceProvider.Resolve<IProgressService>();
            progress.Title = ResX.OpeningProjectTitle;
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

        public async Task StartEpiInfoAsync(Module module, params string[] arguments)
        {
            using (CancellationTokenSource starting = new CancellationTokenSource())
            {
                IProgressService progress = ServiceProvider.Resolve<IProgressService>();
                progress.Title = ResX.StartingEpiInfoTitle;
                progress.CanUserCancel = true;
                try
                {
                    await progress.RunAsync(() =>
                    {
                        starting.CancelAfter(StartEpiInfoWait);
                        Process process = module.Start(arguments);
                        while (!process.WaitForInputIdle((int)StartEpiInfoPollInterval.TotalMilliseconds))
                        {
                            if (progress.IsUserCancellationRequested)
                            {
                                process.CloseMainWindow();
                                process.Close();
                                return;
                            }
                            starting.Token.ThrowIfCancellationRequested();
                        }
                        OnProcessStarted(process);
                    }, starting.Token);
                }
                catch (Exception ex)
                {
                    DialogInfo info = new DialogInfo(DialogInfoPreset.Error);
                    if (ex is OperationCanceledException)
                    {
                        info.Lead = ResX.EpiInfoNotRespondingLead;
                        info.Body = ResX.EpiInfoNotRespondingBody;
                    }
                    else
                    {
                        info.Lead = ResX.StartingEpiInfoErrorLead;
                        info.Body = ex.Message;
                        info.Details = ex.ToString();
                    }
                    ServiceProvider.Resolve<IDialogService>().Show(info);
                }
            }
        }

        public async Task StartEpiInfoAsync()
        {
            await StartEpiInfoAsync(Module.Menu);
        }

        public void StartFileExplorer()
        {
            string entryDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Process.Start(entryDirectory);
        }

        // TODO: Handle errors
        public async Task ViewCoreViewAsync(CoreView coreView)
        {
            IProgressService progress = ServiceProvider.Resolve<IProgressService>();
            progress.Title = ResX.OpeningProjectTitle;
            await progress.RunAsync(() =>
            {
                Project project = ProjectFactory.GetProject(coreView.ProjectType, Settings.Default.GetProjectPath(coreView.ProjectType));
                Epi.View view = project.Views[coreView.Name];
                content = new ViewViewModel(project, view);
            });
            OnPropertyChanged(nameof(Content));
        }
    }
}
