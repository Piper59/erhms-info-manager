using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels.Integration;
using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Views.Integration
{
    public partial class GetWorkerIdView : Window
    {
        public new GetWorkerIdViewModel DataContext
        {
            get { return (GetWorkerIdViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ICommand CommitCommand { get; }
        public ICommand CancelCommand { get; }

        public GetWorkerIdView()
        {
            CommitCommand = new SyncCommand(Commit, CanCommit);
            CancelCommand = new SyncCommand(Cancel);
            InitializeComponent();
        }

        public bool CanCommit()
        {
            return DataContext?.Workers != null && DataContext.Workers.Items.HasSelection();
        }

        public void Commit()
        {
            DialogResult = true;
            Close();
        }

        public void Cancel()
        {
            DialogResult = false;
            Close();
        }
    }
}
