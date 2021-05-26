using ERHMS.Desktop.ViewModels.Collections;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views.Collections
{
    public partial class AnalysisCollectionView : UserControl
    {
        public new AnalysisCollectionViewModel DataContext
        {
            get { return (AnalysisCollectionViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public AnalysisCollectionView()
        {
            InitializeComponent();
        }
    }
}
