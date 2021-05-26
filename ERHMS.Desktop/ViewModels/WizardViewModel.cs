using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public abstract class WizardViewModel : ObservableObject
    {
        public abstract class StepViewModel : ObservableObject
        {
            public WizardViewModel Wizard { get; protected set; }
            public StepViewModel Antecedent { get; protected set; }
            public bool First => Antecedent == null;
            public virtual bool Committing => false;
            public bool Committed => !First && Antecedent.Committing;
            public abstract string Lead { get; }

            public string ContinueAccessText
            {
                get
                {
                    if (Committed)
                    {
                        return ResXResources.AccessText_Close;
                    }
                    else if (Committing)
                    {
                        return ResXResources.AccessText_Finish;
                    }
                    else
                    {
                        return ResXResources.AccessText_Next;
                    }
                }
            }

            public ICommand ReturnCommand { get; }
            public ICommand ContinueCommand { get; }
            public ICommand CancelCommand { get; }

            protected StepViewModel()
            {
                ReturnCommand = new SyncCommand(Return, CanReturn);
                ContinueCommand = new AsyncCommand(ContinueAsync, CanContinue);
            }

            protected void OnCloseRequested() => Wizard.OnCloseRequested();

            protected void ContinueTo(StepViewModel step)
            {
                step.Wizard = Wizard;
                step.Antecedent = this;
                Wizard.Step = step;
            }

            public virtual bool CanReturn()
            {
                return !First && !Committed;
            }

            public virtual void Return()
            {
                Wizard.Step = Antecedent;
            }

            public abstract bool CanContinue();
            public abstract Task ContinueAsync();

            public virtual bool CanCancel()
            {
                return !Committed;
            }

            public virtual void Cancel()
            {
                OnCloseRequested();
            }
        }

        protected StepViewModel step;
        public StepViewModel Step
        {
            get { return step; }
            protected set { SetProperty(ref step, value); }
        }

        public event EventHandler CloseRequested;
        protected virtual void OnCloseRequested(EventArgs e) => CloseRequested?.Invoke(this, e);
        protected void OnCloseRequested() => OnCloseRequested(EventArgs.Empty);
    }
}
