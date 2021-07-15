using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.Domain;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public partial class CreateProjectViewModel : WizardViewModel
    {
        public class SetStrategyViewModel : StepViewModel<CreateProjectViewModel>
        {
            public override string Title => Strings.CreateProject_Lead_SetStrategy;
            public CoreProject CoreProject => Wizard.CoreProject;

            public ICommand CreateBlankCommand { get; }
            public ICommand CreateStandardCommand { get; }
            public ICommand CreateFromTemplateCommand { get; }
            public ICommand CreateFromExistingCommand { get; }

            public SetStrategyViewModel(CreateProjectViewModel wizard)
                : base(wizard) { }
        }

        public CoreProject CoreProject { get; }
        public Project Project { get; private set; }

        public CreateProjectViewModel(CoreProject coreProject)
        {
            CoreProject = coreProject;
            Step = new SetStrategyViewModel(this);
        }
    }
}
