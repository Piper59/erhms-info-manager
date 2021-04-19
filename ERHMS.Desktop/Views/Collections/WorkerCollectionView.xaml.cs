using ERHMS.Desktop.ViewModels.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Views.Collections
{
    public partial class WorkerCollectionView : UserControl
    {
        public static readonly DependencyProperty CommitCommandProperty = DependencyProperty.Register(
            nameof(CommitCommand),
            typeof(ICommand),
            typeof(WorkerCollectionView));

        public new WorkerCollectionViewModel DataContext
        {
            get { return (WorkerCollectionViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ICommand CommitCommand
        {
            get { return (ICommand)GetValue(CommitCommandProperty); }
            set { SetValue(CommitCommandProperty, value); }
        }

        public WorkerCollectionView()
        {
            InitializeComponent();
        }
    }
}
