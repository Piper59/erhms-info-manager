using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
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
                Wizard.GoForward(new CreateProjectViewModels.SetProjectInfoViewModel(State));
            }

            public void Open()
            {
                Wizard.Close();
                Wizard.Result = State.CoreProject.Open();
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
