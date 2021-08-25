using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.Domain;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class StartViewModel : ObservableObject
    {
        public class ProjectViewModel : ObservableObject
        {
            public CoreProject CoreProject { get; }
            public CoreView MainCoreView => CoreView.GetInstances(CoreProject).First();

            private bool hasPath;
            public bool HasPath
            {
                get { return hasPath; }
                set { SetProperty(ref hasPath, value); }
            }

            public ICommand SetUpProjectCommand { get; }
            public ICommand GoToMainViewCommand { get; }

            public ProjectViewModel(CoreProject coreProject)
            {
                CoreProject = coreProject;
                Initialize();
                Settings.Default.SettingsSaving += Default_SettingsSaving;
                SetUpProjectCommand = new SyncCommand(SetUpProject);
                GoToMainViewCommand = new SyncCommand(GoToMainView, CanGoToMainView);
            }

            private void Default_SettingsSaving(object sender, CancelEventArgs e)
            {
                Initialize();
            }

            private void Initialize()
            {
                HasPath = Settings.Default.HasProjectPath(CoreProject);
            }

            public void SetUpProject()
            {
                WizardViewModel wizard = SetUpProjectViewModels.GetWizard(CoreProject);
                wizard.Run();
            }

            public bool CanGoToMainView()
            {
                return AppCommands.GoToCoreViewCommand.CanExecute(MainCoreView);
            }

            public void GoToMainView()
            {
                AppCommands.GoToCoreViewCommand.Execute(MainCoreView);
            }
        }

        public ProjectViewModel WorkerProject { get; } = new ProjectViewModel(CoreProject.Worker);
        public ProjectViewModel IncidentProject { get; } = new ProjectViewModel(CoreProject.Incident);

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
            Closed = WorkerProject.HasPath && IncidentProject.HasPath;
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
