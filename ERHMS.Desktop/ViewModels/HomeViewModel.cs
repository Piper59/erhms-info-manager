using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using System;
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
                CreateCommand = new SyncCommand(Create);
                OpenCommand = new SyncCommand(Open);
                GoToCurrentCommand = new AsyncCommand(GoToCurrentAsync);
            }

            public void Create()
            {
                MainViewModel.Instance.CreateProject(Value);
                Refresh();
            }

            public void Open()
            {
                MainViewModel.Instance.OpenProject(Value);
                Refresh();
            }

            public async Task GoToCurrentAsync()
            {
                await MainViewModel.Instance.GoToProjectAsync(Value);
            }

            protected abstract void Refresh();
        }

        public class WorkerProjectCollectionViewModel : CoreProjectCollectionViewModel
        {
            public override CoreProject Value => CoreProject.Worker;
            public override bool CanHaveRecents => false;

            public override ICommand MakeCurrentCommand => Command.Null;
            public override ICommand RemoveRecentCommand => Command.Null;

            public WorkerProjectCollectionViewModel()
            {
                Initialize();
            }

            private void Initialize()
            {
                if (Settings.Default.HasWorkerProjectPath)
                {
                    Current = new ProjectInfo(Settings.Default.WorkerProjectPath);
                }
            }

            protected override void Refresh()
            {
                Initialize();
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
                MakeCurrentCommand = new SyncCommand<ProjectInfo>(MakeCurrent, IsNotEmpty);
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

            public void MakeCurrent(ProjectInfo projectInfo)
            {
                Settings.Default.IncidentProjectPath = projectInfo.FilePath;
                Settings.Default.Save();
                Initialize();
            }

            public void RemoveRecent(ProjectInfo projectInfo)
            {
                Settings.Default.IncidentProjectPaths.Remove(projectInfo.FilePath);
                Settings.Default.Save();
                Initialize();
            }

            protected override void Refresh()
            {
                Initialize();
            }
        }

        public class PhaseViewModel : ObservableObject
        {
            private static readonly CoreProjectCollectionViewModel workerProjects =
                new WorkerProjectCollectionViewModel();
            private static readonly CoreProjectCollectionViewModel incidentProjects =
                new IncidentProjectCollectionViewModel();

            public static PhaseViewModel PreDeployment { get; } = new PhaseViewModel(Phase.PreDeployment);
            public static PhaseViewModel Deployment { get; } = new PhaseViewModel(Phase.Deployment);
            public static PhaseViewModel PostDeployment { get; } = new PhaseViewModel(Phase.PostDeployment);

            public static IEnumerable<PhaseViewModel> Instances { get; } = new PhaseViewModel[]
            {
                PreDeployment,
                Deployment,
                PostDeployment
            };

            private static CoreProjectCollectionViewModel GetProjects(CoreProject coreProject)
            {
                switch (coreProject)
                {
                    case CoreProject.Worker:
                        return workerProjects;
                    case CoreProject.Incident:
                        return incidentProjects;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(coreProject));
                }
            }

            public Phase Value { get; }
            public CoreProjectCollectionViewModel Projects { get; }
            public IEnumerable<CoreViewViewModel> Views { get; }

            public PhaseViewModel(Phase value)
            {
                Value = value;
                Projects = GetProjects(value.ToCoreProject());
                Views = CoreView.GetInstances(value)
                    .Select(coreView => new CoreViewViewModel(coreView))
                    .ToList();
            }
        }

        public IEnumerable<PhaseViewModel> Phases => PhaseViewModel.Instances;
    }
}
