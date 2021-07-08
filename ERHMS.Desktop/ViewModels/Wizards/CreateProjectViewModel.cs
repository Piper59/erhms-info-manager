using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.Domain;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public partial class CreateProjectViewModel : WizardViewModel
    {
        public class InitializeViewModel : StepViewModel<CreateProjectViewModel>
        {
            public override string Title => Strings.CreateProject_Lead_Initialize;

            public ICommand CreateBlankCommand { get; }
            public ICommand CreateStandardCommand { get; }
            public ICommand CreateFromTemplateCommand { get; }
            public ICommand CreateFromExistingCommand { get; }

            public InitializeViewModel(CreateProjectViewModel wizard)
                : base(wizard) { }
        }

        public CoreProject CoreProject { get; }
        public Project Project { get; private set; }

        public CreateProjectViewModel(CoreProject coreProject)
        {
            CoreProject = coreProject;
            Step = new InitializeViewModel(this);
        }
    }
}
