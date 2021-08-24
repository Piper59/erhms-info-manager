using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Naming;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static partial class CreateViewViewModels
    {
        public partial class State
        {
            public Project Project { get; }
            public string ViewName { get; set; }
            public View View { get; set; }

            public State(Project project)
            {
                Project = project;
            }
        }

        public class SetStrategyViewModel : StepViewModel<State>
        {
            public override string Title => Strings.CreateView_Lead_SetStrategy;

            public ICommand CreateBlankCommand { get; }
            public ICommand CreateFromTemplateCommand { get; }
            public ICommand CreateFromExistingCommand { get; }

            public SetStrategyViewModel(State state)
                : base(state)
            {
                CreateBlankCommand = new SyncCommand(CreateBlank);
                CreateFromTemplateCommand = new SyncCommand(CreateFromTemplate);
                CreateFromExistingCommand = new AsyncCommand(CreateFromExistingAsync);
            }

            public void CreateBlank()
            {
                Wizard.GoForward(new Blank.SetViewNameViewModel(State));
            }

            public void CreateFromTemplate()
            {
                Wizard.GoForward(new FromTemplate.SetXTemplateViewModel(State));
            }

            public async Task CreateFromExistingAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                Wizard.GoForward(await progress.Run(() =>
                {
                    return FromExisting.SetSourceViewViewModel.CreateAsync(State);
                }));
            }
        }

        public abstract class SetViewNameViewModel : StepViewModel<State>
        {
            public override string Title => Strings.CreateView_Lead_SetViewName;

            private string viewName;
            public string ViewName
            {
                get { return viewName; }
                set { SetProperty(ref viewName, value); }
            }

            protected SetViewNameViewModel(State state)
                : base(state) { }

            protected abstract StepViewModel GetSubsequent();

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_ValidatingName;
                InvalidNameReason reason = InvalidNameReason.None;
                bool valid = await progress.Run(() =>
                {
                    ViewNameValidator validator = new ViewNameValidator(State.Project);
                    return validator.IsValid(ViewName, out reason);
                });
                if (reason != InvalidNameReason.None)
                {
                    IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                    dialog.Severity = DialogSeverity.Warning;
                    dialog.Lead = reason.GetLead();
                    dialog.Body = reason.GetViewBody();
                    dialog.Buttons = DialogButtonCollection.Close;
                    dialog.Show();
                }
                if (!valid)
                {
                    return;
                }
                State.ViewName = ViewName;
                Wizard.GoForward(GetSubsequent());
            }
        }

        public abstract class CommitViewModel : StepViewModel<State>
        {
            public override string Title => Strings.Lead_Commit;
            public override string ContinueAction => Strings.AccessText_Finish;
            public DetailsViewModel Details { get; protected set; }

            public CommitViewModel(State state)
                : base(state) { }

            protected abstract View ContinueCore();

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_CreatingView;
                State.View = await progress.Run(ContinueCore);
                Wizard.Result = true;
                Wizard.Committed = true;
                Wizard.GoForward(new CloseViewModel(State));
            }
        }

        public class CloseViewModel : StepViewModel<State>
        {
            public override string Title => Strings.CreateView_Lead_Close;
            public override string ContinueAction => Strings.AccessText_Close;

            private bool openInEpiInfo = true;
            public bool OpenInEpiInfo
            {
                get { return openInEpiInfo; }
                set { SetProperty(ref openInEpiInfo, value); }
            }

            public CloseViewModel(State state)
                : base(state) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                if (OpenInEpiInfo)
                {
                    await Integration.StartAsync(
                        Module.MakeView,
                        $"/project:{State.Project.FilePath}",
                        $"/view:{State.View.Name}");
                }
                Wizard.Close();
            }
        }

        public static WizardViewModel GetWizard(Project project)
        {
            State state = new State(project);
            StepViewModel step = new SetStrategyViewModel(state);
            return new WizardViewModel(step);
        }
    }
}
