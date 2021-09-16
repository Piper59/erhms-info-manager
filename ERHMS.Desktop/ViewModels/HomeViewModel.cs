using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using System;
using System.Collections.Generic;
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

        public abstract class ProjectCollectionViewModel : ObservableObject
        {
            public abstract CoreProject CoreProject { get; }

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

            public abstract ICommand SetCurrentCommand { get; }
            public abstract ICommand RemoveRecentCommand { get; }

            protected ProjectCollectionViewModel()
            {
                Configuration.Instance.Saved += Configuration_Saved;
            }

            private void Configuration_Saved(object sender, EventArgs e)
            {
                Refresh();
            }

            protected abstract void Refresh();
        }

        public class WorkerProjectCollectionViewModel : ProjectCollectionViewModel
        {
            public override CoreProject CoreProject => CoreProject.Worker;
            public override bool CanHaveRecents => false;

            public override ICommand SetCurrentCommand => Command.Null;
            public override ICommand RemoveRecentCommand => Command.Null;

            public WorkerProjectCollectionViewModel()
            {
                Initialize();
            }

            private void Initialize()
            {
                Current =
                    Configuration.Instance.HasWorkerProjectPath
                    ? new ProjectInfo(Configuration.Instance.WorkerProjectPath)
                    : null;
            }

            protected override void Refresh()
            {
                Initialize();
            }
        }

        public class IncidentProjectCollectionViewModel : ProjectCollectionViewModel
        {
            public override CoreProject CoreProject => CoreProject.Incident;
            public override bool CanHaveRecents => true;

            public override ICommand SetCurrentCommand { get; }
            public override ICommand RemoveRecentCommand { get; }

            public IncidentProjectCollectionViewModel()
            {
                Initialize();
                SetCurrentCommand = new SyncCommand<ProjectInfo>(SetCurrent, IsNotEmpty);
                RemoveRecentCommand = new SyncCommand<ProjectInfo>(RemoveRecent, IsNotEmpty);
            }

            private void Initialize()
            {
                Current =
                    Configuration.Instance.HasIncidentProjectPaths
                    ? new ProjectInfo(Configuration.Instance.IncidentProjectPath)
                    : null;
                Recents = Configuration.Instance.IncidentProjectPaths.Cast<string>()
                    .Skip(1)
                    .Select(path => new ProjectInfo(path))
                    .DefaultIfEmpty(EmptyProjectInfo.Instance)
                    .ToList();
            }

            public bool IsNotEmpty(ProjectInfo projectInfo)
            {
                return projectInfo != EmptyProjectInfo.Instance;
            }

            public void SetCurrent(ProjectInfo projectInfo)
            {
                Configuration.Instance.IncidentProjectPath = projectInfo.FilePath;
                Configuration.Instance.Save();
            }

            public void RemoveRecent(ProjectInfo projectInfo)
            {
                IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                dialog.Severity = DialogSeverity.Warning;
                dialog.Lead = Strings.Lead_ConfirmRecentProjectRemoval;
                dialog.Body = string.Format(Strings.Body_ConfirmRecentProjectRemoval, projectInfo.FilePath);
                dialog.Buttons = DialogButtonCollection.ActionOrCancel(Strings.AccessText_Remove);
                if (!dialog.Show().GetValueOrDefault())
                {
                    return;
                }
                Configuration.Instance.IncidentProjectPaths.Remove(projectInfo.FilePath);
                Configuration.Instance.Save();
            }

            protected override void Refresh()
            {
                Initialize();
            }
        }

        public class PhaseViewModel : ObservableObject
        {
            public Phase Value { get; }
            public ProjectCollectionViewModel Projects { get; }
            public IEnumerable<CoreView> Views { get; }

            public PhaseViewModel(Phase value, ProjectCollectionViewModel projects)
            {
                Value = value;
                Projects = projects;
                Views = CoreView.GetInstances(value).ToList();
            }
        }

        private readonly ProjectCollectionViewModel workerProjects = new WorkerProjectCollectionViewModel();
        private readonly ProjectCollectionViewModel incidentProjects = new IncidentProjectCollectionViewModel();

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
