using ERHMS.Common;
using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public abstract class StepViewModel : ObservableObject, IDisposable
    {
        private readonly Immutable<WizardViewModel> wizard = new Immutable<WizardViewModel>();
        public WizardViewModel Wizard
        {
            get { return wizard.Value; }
            set { wizard.Value = value; }
        }

        private readonly Immutable<StepViewModel> antecedent = new Immutable<StepViewModel>();
        public StepViewModel Antecedent
        {
            get { return antecedent.Value; }
            set { antecedent.Value = value; }
        }

        public abstract string Title { get; }
        public virtual string ContinueAction => Strings.AccessText_Next;

        public ICommand ReturnCommand { get; }
        public ICommand ContinueCommand { get; }
        public ICommand CancelCommand { get; }

        protected StepViewModel()
        {
            ReturnCommand = new SyncCommand(Return, CanReturn);
            ContinueCommand = new AsyncCommand(ContinueAsync, CanContinue);
            CancelCommand = new SyncCommand(Cancel, CanCancel);
        }

        public bool CanReturn()
        {
            return Antecedent != null && !Wizard.Committed;
        }

        public void Return()
        {
            Wizard.GoBack();
        }

        public virtual bool CanContinue()
        {
            return false;
        }

        public virtual Task ContinueAsync()
        {
            throw new NotSupportedException("Cannot continue from this step.");
        }

        public bool CanCancel()
        {
            return !Wizard.Committed;
        }

        public void Cancel()
        {
            Wizard.Close();
        }

        public virtual void Dispose() { }
    }

    public abstract class StepViewModel<TState> : StepViewModel
    {
        public TState State { get; }

        protected StepViewModel(TState state)
        {
            State = state;
        }
    }
}
