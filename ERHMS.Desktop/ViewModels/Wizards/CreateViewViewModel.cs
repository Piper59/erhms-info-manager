using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Naming;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public partial class CreateViewViewModel : WizardViewModel
    {
        public class InitializeViewModel : StepViewModel<CreateViewViewModel>
        {
            public override string Title => Strings.Lead_CreateView_Initialize;

            public ICommand CreateBlankCommand { get; }
            public ICommand CreateFromTemplateCommand { get; }
            public ICommand CreateFromExistingCommand { get; }

            public InitializeViewModel(CreateViewViewModel wizard)
                : base(wizard)
            {
                CreateBlankCommand = new SyncCommand(CreateBlank);
                CreateFromTemplateCommand = new SyncCommand(CreateFromTemplate);
                CreateFromExistingCommand = Command.Null;
            }

            public void CreateBlank()
            {
                ContinueTo(new Blank.SetViewNameViewModel(Wizard, this));
            }

            public void CreateFromTemplate()
            {
                ContinueTo(new FromTemplate.SetXTemplateViewModel(Wizard, this));
            }
        }

        public class CloseViewModel : StepViewModel<CreateViewViewModel>
        {
            public override string Title => Strings.Lead_CreateView_Close;
            public override string ContinueAction => Strings.AccessText_Close;

            private bool openInEpiInfo = true;
            public bool OpenInEpiInfo
            {
                get { return openInEpiInfo; }
                set { SetProperty(ref openInEpiInfo, value); }
            }

            public CloseViewModel(CreateViewViewModel wizard, IStep antecedent)
                : base(wizard, antecedent) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                if (openInEpiInfo)
                {
                    await MainViewModel.Instance.StartEpiInfoAsync(
                        Module.MakeView,
                        $"/project:{Wizard.Project.FilePath}",
                        $"/view:{Wizard.View.Name}");
                }
                Close();
            }
        }

        public Project Project { get; }
        public View View { get; private set; }

        public CreateViewViewModel(Project project)
        {
            Project = project;
            Initialize(new InitializeViewModel(this));
        }

        private async Task<bool> ValidateAsync(string viewName)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_ValidatingViewName;
            bool result = false;
            InvalidViewNameReason reason = InvalidViewNameReason.None;
            await progress.Run(() =>
            {
                ViewNameValidator validator = new ViewNameValidator(Project);
                result = validator.IsValid(viewName, out reason);
            });
            if (reason != InvalidViewNameReason.None)
            {
                IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                dialog.Severity = DialogSeverity.Warning;
                dialog.Lead = reason.GetLead();
                dialog.Body = reason.GetBody();
                dialog.Buttons = DialogButtonCollection.Ok;
                dialog.Show();
            }
            return result;
        }
    }
}
