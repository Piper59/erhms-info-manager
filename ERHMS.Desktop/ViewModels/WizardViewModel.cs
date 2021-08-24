using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Services;
using System;

namespace ERHMS.Desktop.ViewModels
{
    public class WizardViewModel : ObservableObject
    {
        private StepViewModel step;
        public StepViewModel Step
        {
            get { return step; }
            private set { SetProperty(ref step, value); }
        }

        private bool? result;
        public bool? Result
        {
            get { return result; }
            set { SetProperty(ref result, value); }
        }

        private bool committed;
        public bool Committed
        {
            get { return committed; }
            set { SetProperty(ref committed, value); }
        }

        public event EventHandler CloseRequested;
        private void OnCloseRequested(EventArgs e) => CloseRequested?.Invoke(this, e);
        private void OnCloseRequested() => OnCloseRequested(EventArgs.Empty);

        public WizardViewModel(StepViewModel step)
        {
            step.Wizard = this;
            step.Antecedent = null;
            Step = step;
        }

        public bool? Run()
        {
            IWindowService window = ServiceLocator.Resolve<IWindowService>();
            window.ShowDialog(this);
            StepViewModel step = Step;
            while (step != null)
            {
                step.Dispose();
                step = step.Antecedent;
            }
            return Result;
        }

        public void GoForward(StepViewModel step)
        {
            step.Wizard = this;
            step.Antecedent = Step;
            Step = step;
        }

        public void GoBack()
        {
            Step.Dispose();
            Step = Step.Antecedent;
        }

        public void Close()
        {
            OnCloseRequested();
        }
    }
}
