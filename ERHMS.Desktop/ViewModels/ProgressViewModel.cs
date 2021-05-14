using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using System.Threading;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProgressViewModel : ObservableObject
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        public string Title { get; }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public CancellationToken CancellationToken { get; }
        public bool CanBeCanceled => CancellationToken.CanBeCanceled;

        public ICommand CancelCommand { get; }

        public ProgressViewModel(string title, bool canBeCanceled)
        {
            Title = title;
            if (canBeCanceled)
            {
                cancellationTokenSource = new CancellationTokenSource();
                CancellationToken = cancellationTokenSource.Token;
            }
            CancelCommand = new SyncCommand(Cancel, CanCancel);
        }

        public bool CanCancel()
        {
            return CanBeCanceled && !CancellationToken.IsCancellationRequested;
        }

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
