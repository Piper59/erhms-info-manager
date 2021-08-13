using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Domain;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class StartViewModel : ObservableObject
    {
        public class CoreProjectViewModel : ObservableObject
        {
            public CoreProject Value { get; }
            public CoreView MainCoreView => CoreView.GetInstances(Value).First();

            private bool hasPath;
            public bool HasPath
            {
                get { return hasPath; }
                set { SetProperty(ref hasPath, value); }
            }

            public CoreProjectViewModel(CoreProject value)
            {
                Value = value;
                Initialize();
                Settings.Default.SettingsSaving += Default_SettingsSaving;
            }

            private void Default_SettingsSaving(object sender, CancelEventArgs e)
            {
                Initialize();
            }

            private void Initialize()
            {
                HasPath = Settings.Default.HasProjectPath(Value);
            }
        }

        public IEnumerable<CoreProjectViewModel> CoreProjects { get; }

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
            CoreProjects = new CoreProjectViewModel[]
            {
                new CoreProjectViewModel(CoreProject.Worker),
                new CoreProjectViewModel(CoreProject.Incident)
            };
            Closed = CoreProjects.All(coreProject => coreProject.HasPath);
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
