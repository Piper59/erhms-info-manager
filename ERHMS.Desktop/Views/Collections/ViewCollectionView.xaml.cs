using ERHMS.Desktop.ViewModels.Collections;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views.Collections
{
    public partial class ViewCollectionView : UserControl
    {
        public new ViewCollectionViewModel DataContext
        {
            get { return (ViewCollectionViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ViewCollectionView()
        {
            InitializeComponent();
        }
    }
}
