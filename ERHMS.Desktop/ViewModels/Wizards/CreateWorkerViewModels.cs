using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.Domain;
using ERHMS.Domain.Data;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static class CreateWorkerViewModels
    {
        public class State
        {
            public string FirstName { get; }
            public string LastName { get; }
            public string EmailAddress { get; }
            public Worker Worker { get; set; }

            public State(string firstName, string lastName, string emailAddress)
            {
                FirstName = firstName;
                LastName = lastName;
                EmailAddress = emailAddress;
            }
        }

        public class SetWorkerViewModel : StepViewModel<State>
        {
            public override string Title => Strings.CreateWorker_Lead_SetWorker;

            private string firstName;
            public string FirstName
            {
                get { return firstName; }
                set { SetProperty(ref firstName, value); }
            }

            private string lastName;
            public string LastName
            {
                get { return lastName; }
                set { SetProperty(ref lastName, value); }
            }

            private string emailAddress;
            public string EmailAddress
            {
                get { return emailAddress; }
                set { SetProperty(ref emailAddress, value); }
            }

            public SetWorkerViewModel(State state)
                : base(state)
            {
                FirstName = state.FirstName;
                LastName = state.LastName;
                EmailAddress = state.EmailAddress;
            }

            public override bool CanContinue()
            {
                return true;
            }

            public override Task ContinueAsync()
            {
                State.Worker = new Worker
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    EmailAddress = EmailAddress
                };
                Wizard.GoForward(new CommitViewModel(State));
                return Task.CompletedTask;
            }
        }

        public class CommitViewModel : StepViewModel<State>
        {
            public override string Title => Strings.Lead_Commit;
            public override string ContinueAction => Strings.AccessText_Create;
            public DetailsViewModel Details { get; }

            public CommitViewModel(State state)
                : base(state)
            {
                Details = new DetailsViewModel
                {
                    { Strings.Label_FirstName, state.Worker.FirstName },
                    { Strings.Label_LastName, state.Worker.LastName },
                    { Strings.Label_EmailAddress, state.Worker.EmailAddress }
                };
            }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_CreatingWorker;
                await progress.Run(() =>
                {
                    Project project = ProjectExtensions.Open(Configuration.Instance.WorkerProjectPath);
                    View view = project.Views[CoreView.WorkerRosteringForm.Name];
                    using (RecordRepository<Worker> repository = new RecordRepository<Worker>(view))
                    {
                        repository.Insert(State.Worker);
                    }
                });
                Wizard.Result = true;
                Wizard.Committed = true;
                Wizard.GoForward(new CloseViewModel(State));
            }
        }

        public class CloseViewModel : StepViewModel<State>
        {
            public override string Title => Strings.CreateWorker_Lead_Close;
            public override string ContinueAction => Strings.AccessText_Close;

            public CloseViewModel(State state)
                : base(state) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override Task ContinueAsync()
            {
                Wizard.Close();
                return Task.CompletedTask;
            }
        }

        public static WizardViewModel GetWizard(string firstName, string lastName, string emailAddress)
        {
            State state = new State(firstName, lastName, emailAddress);
            StepViewModel step = new SetWorkerViewModel(state);
            return new WizardViewModel(step);
        }
    }
}
