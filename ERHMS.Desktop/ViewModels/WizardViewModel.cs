using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public abstract class WizardViewModel : ObservableObject, IWizard
    {
        public abstract class StepViewModel<TWizard> : ObservableObject, IStep
            where TWizard : WizardViewModel
        {
            public TWizard Wizard { get; }
            IWizard IStep.Wizard => Wizard;
            public IStep Antecedent { get; }
            public abstract string Title { get; }
            public virtual string ContinueAction => ResXResources.AccessText_Next;

            public ICommand ReturnCommand { get; }
            public ICommand ContinueCommand { get; }
            public ICommand CancelCommand { get; }

            protected StepViewModel(TWizard wizard, IStep antecedent = null)
            {
                Wizard = wizard;
                Antecedent = antecedent;
                ReturnCommand = new SyncCommand(Return, CanReturn);
                ContinueCommand = new AsyncCommand(ContinueAsync, CanContinue);
                CancelCommand = new SyncCommand(Cancel, CanCancel);
            }

            protected void ContinueTo(StepViewModel<TWizard> step)
            {
                Wizard.Step = step;
            }

            protected async Task ContinueToAsync(Func<Task<StepViewModel<TWizard>>> action)
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                ContinueTo(await progress.Run(action));
            }

            protected void Commit()
            {
                Wizard.Committed = true;
            }

            protected void SetResult(bool? result)
            {
                Wizard.Result = result;
            }

            protected void Close()
            {
                Wizard.OnCloseRequested();
            }

            public virtual bool CanReturn()
            {
                return Antecedent != null && !Wizard.Committed;
            }

            public virtual void Return()
            {
                Wizard.Step = Antecedent;
            }

            public abstract bool CanContinue();
            public abstract Task ContinueAsync();

            public virtual bool CanCancel()
            {
                return !Wizard.Committed;
            }

            public virtual void Cancel()
            {
                Close();
            }
        }

        private IStep step;
        public IStep Step
        {
            get { return step; }
            private set { SetProperty(ref step, value); }
        }

        private bool committed;
        public bool Committed
        {
            get { return committed; }
            private set { SetProperty(ref committed, value); }
        }

        public bool? Result { get; protected set; }

        public event EventHandler CloseRequested;
        private void OnCloseRequested(EventArgs e) => CloseRequested?.Invoke(this, e);
        private void OnCloseRequested() => OnCloseRequested(EventArgs.Empty);

        protected void Initialize(IStep step)
        {
            this.step = step;
        }

        public bool? Show()
        {
            return ServiceLocator.Resolve<IWizardService>().Show(this);
        }
    }
}
