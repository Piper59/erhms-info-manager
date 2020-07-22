using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Utilities;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels
{
    public class UtilityViewModel : ObservableObject
    {
        public IUtility Utility { get; }

        private bool done;
        public bool Done
        {
            get { return done; }
            private set { SetProperty(ref done, value); }
        }

        public Command ExitCommand { get; }

        public UtilityViewModel(IUtility utility)
        {
            Utility = utility;
            ExitCommand = new SyncCommand(Exit, CanExit, ErrorBehavior.Throw);
        }

        public event EventHandler ExitRequested;

        protected virtual void OnExitRequested()
        {
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(Done):
                    Command.OnCanExecuteChanged();
                    break;
            }
        }

        public async Task Run()
        {
            await Utility.RunAsync();
            Done = true;
        }

        private bool CanExit()
        {
            return Done;
        }

        private void Exit()
        {
            OnExitRequested();
        }
    }
}
