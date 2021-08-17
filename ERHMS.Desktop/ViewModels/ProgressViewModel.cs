using ERHMS.Common.ComponentModel;
using ERHMS.Desktop.Commands;
using System.Threading;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProgressViewModel : ObservableObject
    {
        private readonly CancellationTokenSource running = new CancellationTokenSource();

        private string lead;
        public string Lead
        {
            get { return lead; }
            set { SetProperty(ref lead, value); }
        }

        private string body;
        public string Body
        {
            get { return body; }
            set { SetProperty(ref body, value); }
        }

        private bool canBeCanceled;
        public bool CanBeCanceled
        {
            get { return canBeCanceled; }
            set { SetProperty(ref canBeCanceled, value); }
        }

        public CancellationToken CancellationToken => CanBeCanceled ? running.Token : CancellationToken.None;

        public ICommand CancelCommand { get; }

        public ProgressViewModel()
        {
            CancelCommand = new SyncCommand(Cancel, CanCancel);
        }

        public bool CanCancel()
        {
            return CanBeCanceled && !running.IsCancellationRequested;
        }

        public void Cancel()
        {
            running.Cancel();
        }
    }
}
