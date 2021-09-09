using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Domain;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static class SetUpProjectViewModels
    {
        public class State : CreateProjectViewModels.State
        {
            public State(CoreProject coreProject)
                : base(coreProject) { }
        }

        public class SetStrategyViewModel : StepViewModel<State>
        {
            public override string Title => Strings.SetUpProject_Lead_SetStrategy;

            public ICommand CreateCommand { get; }
            public ICommand OpenCommand { get; }

            public SetStrategyViewModel(State state)
                : base(state)
            {
                CreateCommand = new SyncCommand(Create);
                OpenCommand = new SyncCommand(Open);
            }

            public void Create()
            {
                Wizard.GoForward(new CreateProjectViewModels.SetProjectCreationInfoViewModel(State));
            }

            public void Open()
            {
                Wizard.Close();
                Wizard.Result = SetUpProjectViewModels.Open(State.CoreProject);
            }
        }

        public static WizardViewModel GetWizard(CoreProject coreProject)
        {
            State state = new State(coreProject);
            StepViewModel step = new SetStrategyViewModel(state);
            return new WizardViewModel(step);
        }

        public static bool Open(CoreProject coreProject)
        {
            IFileDialogService fileDialog = ServiceLocator.Resolve<IFileDialogService>();
            fileDialog.InitialDirectory = EpiInfo.Configuration.Instance.Directories.Projects;
            fileDialog.Filter = Strings.FileDialog_Filter_Projects;
            if (fileDialog.Open() != true)
            {
                return false;
            }
            Configuration.Instance.SetProjectPath(coreProject, fileDialog.FileName);
            Configuration.Instance.Save();
            return true;
        }
    }
}
