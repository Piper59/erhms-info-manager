using ERHMS.Common;
using ERHMS.Desktop.Commands;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProgressViewModel : ObservableObject
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private bool canUserCancel;
        public bool CanUserCancel
        {
            get { return canUserCancel; }
            set { SetProperty(ref canUserCancel, value); }
        }

        private bool isUserCancellationRequested;
        public bool IsUserCancellationRequested
        {
            get { return isUserCancellationRequested; }
            private set { SetProperty(ref isUserCancellationRequested, value); }
        }

        public ICommand CancelCommand { get; }

        public ProgressViewModel()
        {
            CancelCommand = new SyncCommand(Cancel, CanCancel);
        }

        public bool CanCancel()
        {
            return canUserCancel && !isUserCancellationRequested;
        }

        public void Cancel()
        {
            IsUserCancellationRequested = true;
        }
    }
}
