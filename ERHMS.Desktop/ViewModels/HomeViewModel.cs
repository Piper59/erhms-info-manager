using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class HomeViewModel
    {
        public class EmptyProjectInfo : ProjectInfo
        {
            public static EmptyProjectInfo Instance { get; } = new EmptyProjectInfo();

            private EmptyProjectInfo() { }
        }

        public class CoreViewViewModel
        {
            public CoreView Value { get; }

            public ICommand GoToViewCommand { get; }

            public CoreViewViewModel(CoreView value)
            {
                Value = value;
                GoToViewCommand = new AsyncCommand(GoToViewAsync);
            }

            public async Task GoToViewAsync()
            {
                await MainViewModel.Instance.GoToViewAsync(Value);
            }
        }

        public abstract class CoreProjectCollectionViewModel : ObservableObject
        {
            public abstract CoreProject Value { get; }

            private ProjectInfo current;
            public ProjectInfo Current
            {
                get { return current; }
                protected set { SetProperty(ref current, value); }
            }

            public abstract bool CanHaveRecents { get; }

            private IEnumerable<ProjectInfo> recents;
            public IEnumerable<ProjectInfo> Recents
            {
                get { return recents; }
                protected set { SetProperty(ref recents, value); }
            }

            public ICommand CreateCommand { get; }
            public ICommand OpenCommand { get; }
            public ICommand GoToCurrentCommand { get; }
            public abstract ICommand MakeCurrentCommand { get; }
            public abstract ICommand RemoveRecentCommand { get; }

            protected CoreProjectCollectionViewModel()
            {
                CreateCommand = new AsyncCommand(CreateAsync);
                OpenCommand = new AsyncCommand(OpenAsync);
                GoToCurrentCommand = new AsyncCommand(GoToCurrentAsync);
            }

            public async Task CreateAsync()
            {
                // TODO: Stay in home view
                await MainViewModel.Instance.CreateProjectAsync(Value);
            }

            public async Task OpenAsync()
            {
                // TODO: Stay in home view
                await MainViewModel.Instance.OpenProjectAsync(Value);
            }

            public async Task GoToCurrentAsync()
            {
                await MainViewModel.Instance.GoToProjectAsync(Value);
            }
        }

        public class WorkerProjectCollectionViewModel : CoreProjectCollectionViewModel
        {
            public override CoreProject Value => CoreProject.Worker;
            public override bool CanHaveRecents => false;

            public override ICommand MakeCurrentCommand => Command.Null;
            public override ICommand RemoveRecentCommand => Command.Null;

            public WorkerProjectCollectionViewModel()
            {
                if (Settings.Default.HasWorkerProjectPath)
                {
                    Current = new ProjectInfo(Settings.Default.WorkerProjectPath);
                }
            }
        }

        public class IncidentProjectCollectionViewModel : CoreProjectCollectionViewModel
        {
            public override CoreProject Value => CoreProject.Incident;
            public override bool CanHaveRecents => true;

            public override ICommand MakeCurrentCommand { get; }
            public override ICommand RemoveRecentCommand { get; }

            public IncidentProjectCollectionViewModel()
            {
                Initialize();
                MakeCurrentCommand = new AsyncCommand<ProjectInfo>(MakeCurrentAsync, IsNotEmpty);
                RemoveRecentCommand = new SyncCommand<ProjectInfo>(RemoveRecent, IsNotEmpty);
            }

            private void Initialize()
            {
                if (Settings.Default.HasIncidentProjectPath)
                {
                    Current = new ProjectInfo(Settings.Default.IncidentProjectPath);
                }
                Recents = Settings.Default.IncidentProjectPaths.Cast<string>()
                    .Skip(1)
                    .Select(path => new ProjectInfo(path))
                    .DefaultIfEmpty(EmptyProjectInfo.Instance)
                    .ToList();
            }

            public bool IsNotEmpty(ProjectInfo projectInfo)
            {
                return projectInfo != EmptyProjectInfo.Instance;
            }

            public async Task MakeCurrentAsync(ProjectInfo projectInfo)
            {
                // TODO: Stay in home view
                Settings.Default.IncidentProjectPath = projectInfo.FilePath;
                Settings.Default.Save();
                Initialize();
                await MainViewModel.Instance.GoToProjectAsync(() => Task.Run(() =>
                {
                    return ProjectExtensions.Open(projectInfo.FilePath);
                }));
            }

            public void RemoveRecent(ProjectInfo projectInfo)
            {
                // TODO: Confirm
                Settings.Default.IncidentProjectPaths.Remove(projectInfo.FilePath);
                Settings.Default.Save();
                Initialize();
            }
        }

        public class PhaseViewModel : ObservableObject
        {
            public Phase Value { get; }
            public CoreProjectCollectionViewModel Projects { get; }
            public IEnumerable<CoreViewViewModel> Views { get; }

            public PhaseViewModel(Phase value, CoreProjectCollectionViewModel projects)
            {
                Value = value;
                Projects = projects;
                Views = CoreView.GetInstances(value)
                    .Select(coreView => new CoreViewViewModel(coreView))
                    .ToList();
            }
        }

        private readonly CoreProjectCollectionViewModel workerProjects = new WorkerProjectCollectionViewModel();
        private readonly CoreProjectCollectionViewModel incidentProjects = new IncidentProjectCollectionViewModel();

        public IEnumerable<PhaseViewModel> Phases { get; }

        public HomeViewModel()
        {
            Phases = new PhaseViewModel[]
            {
                new PhaseViewModel(Phase.PreDeployment, workerProjects),
                new PhaseViewModel(Phase.Deployment, incidentProjects),
                new PhaseViewModel(Phase.PostDeployment, incidentProjects)
            };
        }
    }
}
