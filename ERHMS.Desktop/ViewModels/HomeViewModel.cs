using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

            public abstract ICommand MakeCurrentCommand { get; }
            public abstract ICommand RemoveRecentCommand { get; }

            protected CoreProjectCollectionViewModel()
            {
                Settings.Default.SettingsSaving += Default_SettingsSaving;
            }

            private void Default_SettingsSaving(object sender, CancelEventArgs e)
            {
                Refresh();
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
            }

            public void RemoveRecent(ProjectInfo projectInfo)
            {
                Settings.Default.IncidentProjectPaths.Remove(projectInfo.FilePath);
                Settings.Default.Save();
            }

            protected override void Refresh()
            {
                Initialize();
            }
        }

        public class PhaseViewModel : ObservableObject
        {
            public Phase Value { get; }
            public CoreProjectCollectionViewModel Projects { get; }
            public IEnumerable<CoreView> Views { get; }

            public PhaseViewModel(Phase value, CoreProjectCollectionViewModel projects)
            {
                Value = value;
                Projects = projects;
                Views = CoreView.GetInstances(value).ToList();
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
