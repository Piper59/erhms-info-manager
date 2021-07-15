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

            protected CoreProjectCollectionViewModel()
            {
                CreateCommand = new SyncCommand(Create);
                OpenCommand = new SyncCommand(Open);
                GoToCurrentCommand = new AsyncCommand(GoToCurrentAsync);
            }

            public void Create()
            {
                MainViewModel.Instance.CreateProject(Value);
            }

            public void Open()
            {
                MainViewModel.Instance.OpenProject(Value);
            }

            public async Task GoToCurrentAsync()
            {
                await MainViewModel.Instance.GoToProjectAsync(() => Task.Run(() =>
                {
                    return ProjectExtensions.Open(Current.FilePath);
                }));
            }
        }

        public class WorkerProjectCollectionViewModel : CoreProjectCollectionViewModel
        {
            public override CoreProject Value => CoreProject.Worker;

            public override ICommand MakeCurrentCommand { get; } = Command.Null;

            public WorkerProjectCollectionViewModel()
            {
                Current = new ProjectInfo(Settings.Default.WorkerProjectPath);
            }
        }

        public class IncidentProjectCollectionViewModel : CoreProjectCollectionViewModel
        {
            public override CoreProject Value => CoreProject.Incident;

            public override ICommand MakeCurrentCommand { get; }

            public IncidentProjectCollectionViewModel()
            {
                Initialize();
                MakeCurrentCommand = new SyncCommand<ProjectInfo>(MakeCurrent);
            }

            private void Initialize()
            {
                Current = new ProjectInfo(Settings.Default.IncidentProjectPath);
                Recents = Settings.Default.IncidentProjectPaths.Cast<string>()
                    .Select(path => new ProjectInfo(path))
                    .ToList();
            }

            public void MakeCurrent(ProjectInfo projectInfo)
            {
                Settings.Default.IncidentProjectPath = projectInfo.FilePath;
                Settings.Default.Save();
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
                Views = CoreView.Instances.Where(coreView => coreView.Phase == value)
                    .Select(coreView => new CoreViewViewModel(coreView))
                    .ToList();
            }
        }

        public IEnumerable<PhaseViewModel> Phases => PhaseViewModel.Instances;
    }
}
