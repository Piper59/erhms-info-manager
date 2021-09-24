using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Shared
{
    public abstract class ProjectInfoViewModel
    {
        public class Empty : ProjectInfoViewModel
        {
            public Empty(CoreProject coreProject)
                : base(coreProject) { }
        }

        public class Worker : ProjectInfoViewModel
        {
            public Worker(string path)
                : base(CoreProject.Worker, path) { }
        }

        public class Incident : ProjectInfoViewModel
        {
            public override ICommand SetCurrentCommand { get; }
            public override ICommand RemoveRecentCommand { get; }

            public Incident(string path)
                : base(CoreProject.Incident, path)
            {
                SetCurrentCommand = new SyncCommand(SetCurrent);
                RemoveRecentCommand = new SyncCommand(RemoveRecent);
            }

            public void SetCurrent()
            {
                Configuration.Instance.IncidentProjectPath = ProjectInfo.FilePath;
                Configuration.Instance.Save();
            }

            public void RemoveRecent()
            {
                IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                dialog.Severity = DialogSeverity.Warning;
                dialog.Lead = Strings.Lead_ConfirmRecentProjectRemoval;
                dialog.Body = string.Format(Strings.Body_ConfirmRecentProjectRemoval, ProjectInfo.FilePath);
                dialog.Buttons = DialogButtonCollection.ActionOrCancel(Strings.AccessText_Remove);
                if (!dialog.Show().GetValueOrDefault())
                {
                    return;
                }
                Configuration.Instance.IncidentProjectPaths.Remove(ProjectInfo.FilePath);
                Configuration.Instance.Save();
            }
        }

        public CoreProject CoreProject { get; }
        public ProjectInfo ProjectInfo { get; }
        public bool IsEmpty => ProjectInfo == null;

        public virtual ICommand SetCurrentCommand => Command.Null;
        public virtual ICommand RemoveRecentCommand => Command.Null;

        protected ProjectInfoViewModel(CoreProject coreProject)
        {
            CoreProject = coreProject;
        }

        protected ProjectInfoViewModel(CoreProject coreProject, string path)
            : this(coreProject)
        {
            ProjectInfo = new ProjectInfo(path);
        }
    }
}
