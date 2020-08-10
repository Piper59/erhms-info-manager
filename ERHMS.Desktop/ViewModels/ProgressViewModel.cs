using ERHMS.Common;
using ERHMS.Desktop.Commands;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProgressViewModel : ObservableObject
    {
        public string TaskName { get; }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public bool CanUserCancel { get; }

        private bool isUserCancellationRequested;
        public bool IsUserCancellationRequested
        {
            get { return isUserCancellationRequested; }
            private set { SetProperty(ref isUserCancellationRequested, value); }
        }

        public ICommand CancelCommand { get; }

        public ProgressViewModel(string taskName, bool canUserCancel)
        {
            TaskName = taskName;
            CanUserCancel = canUserCancel;
            CancelCommand = new SyncCommand(Cancel, CanCancel, ErrorBehavior.Raise);
        }

        public bool CanCancel() => CanUserCancel && !IsUserCancellationRequested;

        public void Cancel()
        {
            IsUserCancellationRequested = true;
        }
    }
}
