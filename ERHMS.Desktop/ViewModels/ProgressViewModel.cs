using ERHMS.Common;
using ERHMS.Desktop.Commands;
using System;
using System.Threading;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProgressViewModel : ObservableObject, IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource;

        public string Title { get; }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public CancellationToken CancellationToken => cancellationTokenSource?.Token ?? CancellationToken.None;
        public bool CanBeCanceled => cancellationTokenSource != null;

        public ICommand CancelCommand { get; }

        public ProgressViewModel(string title, bool canBeCanceled)
        {
            Title = title;
            if (canBeCanceled)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }
            CancelCommand = new SyncCommand(Cancel, CanCancel);
        }

        public bool CanCancel()
        {
            return CanBeCanceled && !cancellationTokenSource.IsCancellationRequested;
        }

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
        }
    }
}
