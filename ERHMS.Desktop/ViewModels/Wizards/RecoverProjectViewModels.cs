using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Domain;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static class RecoverProjectViewModels
    {
        public class State : CreateProjectViewModels.State
        {
            public State(CoreProject coreProject)
                : base(coreProject) { }
        }

        public class SetStrategyViewModel : StepViewModel<State>
        {
            public override string Title => Strings.RecoverProject_Lead_SetStrategy;
            public string ProjectPath { get; }

            public ICommand OpenCommand { get; }
            public ICommand CreateCommand { get; }
            public ICommand RemoveCommand { get; }

            public SetStrategyViewModel(State state)
                : base(state)
            {
                ProjectPath = Configuration.Instance.GetProjectPath(state.CoreProject);
                OpenCommand = new SyncCommand(Open);
                CreateCommand = new SyncCommand(Create);
                RemoveCommand = new SyncCommand(Remove);
            }

            public void Open()
            {
                if (!State.CoreProject.Open())
                {
                    return;
                }
                Wizard.Result = true;
                Wizard.Close();
            }

            public void Create()
            {
                Wizard.GoForward(new CreateProjectViewModels.SetProjectInfoViewModel(State));
            }

            public void Remove()
            {
                Configuration.Instance.UnsetProjectPath(State.CoreProject);
                Configuration.Instance.Save();
                Wizard.Result = false;
                Wizard.Close();
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
