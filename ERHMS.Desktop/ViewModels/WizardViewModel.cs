using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
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
            public virtual string ContinueAction => Strings.AccessText_Next;

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

            protected void GoToStep(IStep step)
            {
                Wizard.Step = step;
            }

            protected void SetResult(bool? result)
            {
                Wizard.Result = result;
            }

            protected void Commit()
            {
                Wizard.Committed = true;
            }

            protected void Commit(bool? result)
            {
                SetResult(result);
                Commit();
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

            public virtual bool CanContinue()
            {
                return false;
            }

            public virtual Task ContinueAsync()
            {
                throw new NotSupportedException("Cannot continue from this step.");
            }

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
            protected set { SetProperty(ref step, value); }
        }

        private bool? result;
        public bool? Result
        {
            get
            {
                return result;
            }
            private set
            {
                if (SetProperty(ref result, value))
                {
                    OnPropertyChanged(nameof(Succeeded));
                }
            }
        }

        private bool committed;
        public bool Committed
        {
            get
            {
                return committed;
            }
            private set
            {
                if (SetProperty(ref committed, value))
                {
                    OnPropertyChanged(nameof(Succeeded));
                }
            }
        }

        public bool Succeeded => Result.GetValueOrDefault() && Committed;

        public event EventHandler CloseRequested;
        protected virtual void OnCloseRequested(EventArgs e) => CloseRequested?.Invoke(this, e);
        protected void OnCloseRequested() => OnCloseRequested(EventArgs.Empty);
    }
}
