using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Domain;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static class SetUpProjectViewModels
    {
        public class State
        {
            public CoreProject CoreProject { get; }

            public State(CoreProject coreProject)
            {
                CoreProject = coreProject;
            }
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
                OpenCommand = new SyncCommand(Open, CanOpen);
            }

            public void Create()
            {
                CreateProjectViewModels.State state = new CreateProjectViewModels.State(State.CoreProject);
                Wizard.GoForward(new CreateProjectViewModels.SetStrategyViewModel(state));
            }

            public bool CanOpen()
            {
                return AppCommands.OpenCoreProjectCommand.CanExecute(State.CoreProject);
            }

            public void Open()
            {
                Wizard.Close();
                AppCommands.OpenCoreProjectCommand.Execute(State.CoreProject);
            }
        }

        public static WizardViewModel GetWizard(CoreProject coreProject)
        {
            State state = new State(coreProject);
            StepViewModel step = new SetStrategyViewModel(state);
            return new WizardViewModel(step);
        }
    }
}
