using ERHMS.Desktop.ViewModels.Collections;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views.Collections
{
    public partial class WorkerCollectionView : UserControl
    {
        public new WorkerCollectionViewModel DataContext
        {
            get { return (WorkerCollectionViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public WorkerCollectionView()
        {
            InitializeComponent();
        }
    }
}
