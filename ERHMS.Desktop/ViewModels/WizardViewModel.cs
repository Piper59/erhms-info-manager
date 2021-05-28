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
            public TWizard Wizard { get; private set; }
            IWizard IStep.Wizard => Wizard;
            public StepViewModel<TWizard> Antecedent { get; private set; }
            public abstract string Title { get; }
            public virtual string ContinueAction => ResXResources.AccessText_Next;

            public ICommand ReturnCommand { get; }
            public ICommand ContinueCommand { get; }
            public ICommand CancelCommand { get; }

            protected StepViewModel()
            {
                ReturnCommand = new SyncCommand(Return, CanReturn);
                ContinueCommand = new AsyncCommand(ContinueAsync, CanContinue);
                CancelCommand = new SyncCommand(Cancel, CanCancel);
            }

            protected StepViewModel(TWizard wizard)
                : this()
            {
                Wizard = wizard;
            }

            protected void ContinueTo(StepViewModel<TWizard> step, bool commit = false)
            {
                if (commit)
                {
                    Wizard.Committed = true;
                }
                step.Wizard = Wizard;
                step.Antecedent = this;
                Wizard.Step = step;
            }

            protected void RequestClose(bool? result)
            {
                Wizard.Result = result;
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
                RequestClose(false);
            }
        }

        protected IStep step;
        public IStep Step
        {
            get { return step; }
            protected set { SetProperty(ref step, value); }
        }

        protected bool committed;
        public bool Committed
        {
            get { return committed; }
            protected set { SetProperty(ref committed, value); }
        }

        public bool? Result { get; protected set; }

        public event EventHandler CloseRequested;
        protected virtual void OnCloseRequested(EventArgs e) => CloseRequested?.Invoke(this, e);
        protected void OnCloseRequested() => OnCloseRequested(EventArgs.Empty);

        public bool? Show()
        {
            return ServiceLocator.Resolve<IWizardService>().Show(this);
        }
    }
}
