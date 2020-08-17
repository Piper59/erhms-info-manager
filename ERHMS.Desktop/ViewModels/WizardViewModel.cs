using ERHMS.Common;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Wizards;
using System;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public abstract class WizardViewModel : ObservableObject, IWizard
    {
        public abstract class StepViewModel<TWizard> : ObservableObject, IStep
            where TWizard : WizardViewModel
        {
            public TWizard Wizard { get; }
            public IStep Previous { get; private set; }
            public bool CanReturn => Previous != null && !Wizard.Completed;
            public virtual string ContinueText => "_Next";

            public ICommand ReturnCommand { get; protected set; }
            public abstract ICommand ContinueCommand { get; }

            protected StepViewModel(TWizard wizard)
            {
                Wizard = wizard;
                ReturnCommand = new SyncCommand(Return, () => CanReturn);
            }

            public virtual void Return()
            {
                Wizard.CurrentStep = Previous;
            }

            protected void GoToStep(StepViewModel<TWizard> step)
            {
                step.Previous = this;
                Wizard.CurrentStep = step;
            }
        }

        protected IStep currentStep;
        public IStep CurrentStep
        {
            get { return currentStep; }
            protected set { SetProperty(ref currentStep, value); }
        }

        public bool? Result { get; protected set; }

        protected bool completed;
        public bool Completed
        {
            get { return completed; }
            protected set { SetProperty(ref completed, value); }
        }

        public ICommand ExitCommand { get; protected set; }

        protected WizardViewModel()
        {
            ExitCommand = new SyncCommand(Exit, CanExit);
        }

        public event EventHandler ExitRequested;
        protected void OnExitRequested() => ExitRequested?.Invoke(this, EventArgs.Empty);

        public virtual bool CanExit()
        {
            return true;
        }

        public virtual void Exit()
        {
            OnExitRequested();
        }
    }
}
