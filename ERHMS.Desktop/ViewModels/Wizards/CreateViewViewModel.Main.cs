using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo.Naming;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public partial class CreateViewViewModel : WizardViewModel
    {
        public class SetStrategyViewModel : StepViewModel<CreateViewViewModel>
        {
            public override string Title => Strings.CreateView_Lead_SetStrategy;

            public ICommand CreateBlankCommand { get; }
            public ICommand CreateFromTemplateCommand { get; }
            public ICommand CreateFromExistingCommand { get; }

            public SetStrategyViewModel(CreateViewViewModel wizard)
                : base(wizard)
            {
                CreateBlankCommand = new SyncCommand(CreateBlank);
                CreateFromTemplateCommand = new SyncCommand(CreateFromTemplate);
                CreateFromExistingCommand = new AsyncCommand(CreateFromExistingAsync);
            }

            public void CreateBlank()
            {
                GoToStep(new Blank.SetViewNameViewModel(Wizard, this));
            }

            public void CreateFromTemplate()
            {
                GoToStep(new FromTemplate.SetXTemplateViewModel(Wizard, this));
            }

            public async Task CreateFromExistingAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                IStep step = await progress.Run(() =>
                {
                    return FromExisting.SetSourceViewViewModel.CreateAsync(Wizard, this);
                });
                GoToStep(step);
            }
        }

        public abstract class SetViewNameViewModel : StepViewModel<CreateViewViewModel>
        {
            public override string Title => Strings.CreateView_Lead_SetViewName;

            private string viewName;
            public string ViewName
            {
                get { return viewName; }
                set { SetProperty(ref viewName, value); }
            }

            protected SetViewNameViewModel(CreateViewViewModel wizard, IStep step)
                : base(wizard, step) { }

            protected abstract void GoToNextStep();

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_ValidatingViewName;
                InvalidViewNameReason reason = InvalidViewNameReason.None;
                bool valid = await progress.Run(() =>
                {
                    ViewNameValidator validator = new ViewNameValidator(Wizard.Project);
                    return validator.IsValid(ViewName, out reason);
                });
                if (reason != InvalidViewNameReason.None)
                {
                    IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                    dialog.Severity = DialogSeverity.Warning;
                    dialog.Lead = reason.GetLead();
                    dialog.Body = reason.GetBody();
                    dialog.Buttons = DialogButtonCollection.Close;
                    dialog.Show();
                }
                if (!valid)
                {
                    return;
                }
                Wizard.ViewName = ViewName;
                GoToNextStep();
            }
        }

        public class CloseViewModel : StepViewModel<CreateViewViewModel>
        {
            public override string Title => Strings.CreateView_Lead_Close;
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

            public override Task ContinueAsync()
            {
                Wizard.OpenInEpiInfo = OpenInEpiInfo;
                Close();
                return Task.CompletedTask;
            }
        }

        public Project Project { get; }
        public string ViewName { get; private set; }
        public View View { get; private set; }
        public bool OpenInEpiInfo { get; private set; }

        public CreateViewViewModel(Project project)
        {
            Project = project;
            Step = new SetStrategyViewModel(this);
        }
    }
}
