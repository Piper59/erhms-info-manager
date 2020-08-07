using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
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
            StartEpiInfoCommand = new SimpleAsyncCommand(StartEpiInfoAsync);
            StartFileExplorerCommand = new SimpleSyncCommand(StartFileExplorer);
        }

        public event EventHandler<ProcessStartedEventArgs> ProcessStarted;
        private void OnProcessStarted(ProcessStartedEventArgs e) => ProcessStarted?.Invoke(this, e);
        private void OnProcessStarted(Process process) => OnProcessStarted(new ProcessStartedEventArgs(process));

        public event EventHandler ExitRequested;
        private void OnExitRequested() => ExitRequested?.Invoke(this, EventArgs.Empty);

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
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningProjectTaskName, false);
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
            IProgressService progress = ServiceProvider.GetProgressService(Resources.OpeningProjectTaskName, false);
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
                IProgressService progress = ServiceProvider.GetProgressService(Resources.StartingEpiInfoTaskName, true);
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
                        info.Lead = Resources.EpiInfoNotRespondingLead;
                        info.Body = Resources.EpiInfoNotRespondingBody;
                    }
                    else
                    {
                        info.Lead = Resources.StartingEpiInfoErrorLead;
                        info.Body = ex.Message;
                        info.Details = ex.ToString();
                    }
                    IDialogService dialog = ServiceProvider.GetDialogService(info);
                    dialog.Show();
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

        public void Exit()
        {
            OnExitRequested();
        }
    }
}
