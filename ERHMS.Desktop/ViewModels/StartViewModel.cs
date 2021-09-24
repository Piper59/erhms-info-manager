using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Domain;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class StartViewModel : ObservableObject
    {
        public class CoreProjectViewModel
        {
            public CoreProject CoreProject { get; }
            public CoreView MainCoreView { get; }

            public CoreProjectViewModel(CoreProject coreProject)
            {
                CoreProject = coreProject;
                MainCoreView = CoreView.GetInstances(CoreProject).First();
            }
        }

        public CoreProjectViewModel WorkerProject { get; } = new CoreProjectViewModel(CoreProject.Worker);
        public CoreProjectViewModel IncidentProject { get; } = new CoreProjectViewModel(CoreProject.Incident);

        private bool minimized;
        public bool Minimized
        {
            get { return minimized; }
            set { SetProperty(ref minimized, value); }
        }

        private bool closed;
        public bool Closed
        {
            get { return closed; }
            set { SetProperty(ref closed, value); }
        }

        public ICommand ToggleCommand { get; }
        public ICommand CloseCommand { get; }

        public StartViewModel()
        {
            Closed = Configuration.Instance.HasWorkerProjectPath && Configuration.Instance.HasIncidentProjectPaths;
            ToggleCommand = new SyncCommand(Toggle);
            CloseCommand = new SyncCommand(Close);
        }

        public void Toggle()
        {
            Minimized = !Minimized;
        }

        public void Close()
        {
            Closed = true;
        }
    }
}
